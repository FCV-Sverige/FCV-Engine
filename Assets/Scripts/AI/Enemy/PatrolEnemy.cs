using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Options"), Space(5)]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float stopDistance = 0.1f; // Distance to stop at the point

    [Space(15)] [Header("Chase Options"), Space(5)] 
    [SerializeField] private float chaseSpeed = 5;
    [SerializeField] private float detectionRadius = 5;
    [SerializeField, Range(0, 360)] private float detectionAngle = 360;

    private Rigidbody2D rb;
    private int currentPointIndex = 0;
    private bool movingForward = true;

    private bool chasing = false;

    private Vector3 MostLeft => patrolPoints.OrderBy(v => v.x).First();
    private Vector3 MostRight => patrolPoints.OrderBy(v => v.x).Last();
    private Vector3 CurrentTarget => movingForward ? MostLeft : MostRight;

    private float Direction => Mathf.Sign((chasing ? playerTransform.position.x : CurrentTarget.x) - transform.position.x);

    private List<Vector3> patrolPoints;

    private Transform playerTransform;
    private void Start()
    {
        playerTransform = Movement.PlayerTransform;
        rb = GetComponent<Rigidbody2D>();
        UpdatePatrolPoints();
        InvokeRepeating(nameof(UpdatePatrolPoints), 1f, 1f);
    }

    private void UpdatePatrolPoints()
    {
        patrolPoints = PlatformFinder.Instance.GetClosestPlatform(transform.position).Select(x => PlatformFinder.Instance.TileMap.GetCellCenterWorld(x)).ToList();
    }

    private void Update()
    {
        if (IsWithinFOVAndRange(transform.position, playerTransform.position, Vector3.right * Direction, detectionRadius, detectionAngle))
        {
            chasing = true;
            rb.velocity = Vector3.right * (Direction * chaseSpeed);
            return;
        }

        chasing = false;
        PatrolMovement();
    }

    private void PatrolMovement()
    {
        if (patrolPoints.Count == 0)
            return;
        
        // Calculate the interpolation factor based on the speed
        Vector3 target = CurrentTarget;
        float travelTime = Vector3.Distance(transform.position, target) / patrolSpeed;
        float lerpFactor = Time.deltaTime / travelTime;

        if (Vector3.Distance(rb.position, target) > stopDistance)
        {
            Vector3 end = target;
            transform.position = Vector3.Lerp(transform.position, end, lerpFactor);
        }
        else
        {
            // Reached the patrol point
            if (movingForward)
            {
                currentPointIndex++;
                if (currentPointIndex >= patrolPoints.Count)
                {
                    movingForward = false;
                }
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex < 0)
                {
                    movingForward = true;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints is { Count: > 0 })
            Handles.DrawPolyLine(patrolPoints.ToArray());
        
        if (Application.isPlaying) return;
        
        // Draw the range as a sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw the FOV as a frustum
        Color color;
        Gizmos.color = color = Color.green;
        color.a = .1f;
        Handles.color = color;

        DrawFOV(transform.position, Vector2.right, detectionRadius, detectionAngle);
    }
    
    // Helper function to draw the FOV as a frustum
    private void DrawFOV(Vector3 origin, Vector3 direction, float maxDistance, float fovAngle)
    {
        // Calculate the frustum vertices
        Vector3 right = Quaternion.Euler(0, 0, fovAngle / 2) * direction;
        Vector3 left = Quaternion.Euler(0, 0, -fovAngle / 2) * direction;

        // Draw the frustum edges
        Gizmos.DrawLine(origin, origin + direction * maxDistance); // Forward line
        Gizmos.DrawLine(origin, origin + right * maxDistance);    // Right edge
        Gizmos.DrawLine(origin, origin + left * maxDistance);     // Left edge
        
        Handles.DrawSolidArc(origin, Vector3.back, right.normalized, fovAngle, maxDistance);
        
    }
    
    // Function to check if the target is within FOV and range of the source
    private bool IsWithinFOVAndRange(Vector3 source, Vector3 target, Vector3 direction, float maxDistance, float fovAngle)
    {
        // Calculate the vector from source to target
        Vector3 toTarget = target - source;

        // Calculate the distance to the target
        float distanceToTarget = toTarget.magnitude;

        // Check if the target is within the maximum distance
        if (distanceToTarget > maxDistance)
        {
            return false;
        }

        // Normalize the direction and toTarget vectors
        Vector3 normalizedDirection = direction.normalized;
        Vector3 normalizedToTarget = toTarget.normalized;

        // Calculate the angle between the direction and toTarget vectors
        float angle = Vector3.Angle(normalizedDirection, normalizedToTarget);

        // Check if the target is within the FOV angle
        if (angle <= fovAngle / 2)
        {
            return true;
        }

        return false;
    }
}

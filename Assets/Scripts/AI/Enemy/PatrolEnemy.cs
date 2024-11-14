using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Options"), Space(5)]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float stopDistance = 0.1f; // Distance to stop at the point

    [Space(15)] [Header("Detection Options"), Space(5)] 
    [SerializeField] private float detectionRadius = 5;
    [SerializeField, Range(0, 360)] private float detectionAngle = 360;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int currentPointIndex = 0;
    private bool movingForward = true;

    private bool chasing = false;

    private Vector3 MostLeft => patrolPoints.OrderBy(v => v.x).First();
    private Vector3 MostRight => patrolPoints.OrderBy(v => v.x).Last();
    private Vector3 CurrentTarget => chasing ? patrolPoints[0] : movingForward ? MostLeft : MostRight;

    private float Direction => Mathf.Sign((chasing ? playerTransform.position.x : CurrentTarget.x) - transform.position.x);

    private List<Vector3> patrolPoints;

    private Transform playerTransform;

    private PlatformFinder platformFinder;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformFinder = PlatformFinder.Instance;
        playerTransform = Movement.PlayerTransform;
        rb = GetComponent<Rigidbody2D>();
        UpdatePatrolPoints();
        InvokeRepeating(nameof(UpdatePatrolPoints), 1f, 1f);
    }

    private void UpdatePatrolPoints()
    {
        patrolPoints = platformFinder.GetClosestPlatform(transform.position).Select(x => platformFinder.TileMap.GetCellCenterWorld(x)).ToList();
    }

    private void Update()
    {
        if (patrolPoints is {Count: 0}) return;
        
        // if player is within range: start chase, if not: stop chase
        if (FOVUtility.IsWithinFOVAndRange(transform.position, playerTransform.position, Vector3.right * Direction, detectionRadius, detectionAngle))
        {
            chasing = true;
            if (patrolPoints is {Count : >1})
            {
                patrolPoints.Clear();
                patrolPoints.Add(Vector3.one);
            }

            patrolPoints[0] = platformFinder.TileMap.GetCellCenterWorld(platformFinder.GetPlatformBelow(Movement.PlayerTransform.position) + Vector3Int.up);
        }
        else
        {
            chasing = false;
        }

        PatrolMovement();
        spriteRenderer.flipX = movingForward; // TODO: Change to not moving forward when Isaac delivers new animations
    }
    
    // controls the movement of the enemy by its patrol points, reversing when end is reached
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
#if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints is { Count: > 0 })
            Handles.DrawPolyLine(patrolPoints.ToArray());
        
        
        // Draw the range as a sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw the FOV as a frustum
        Color color;
        Gizmos.color = color = Color.green;
        color.a = .1f;
        Handles.color = color;

        FOVUtility.DrawFOV(transform.position, Application.isPlaying ? Vector2.right * Direction: Vector2.right, detectionRadius, detectionAngle);
    }
#endif
}

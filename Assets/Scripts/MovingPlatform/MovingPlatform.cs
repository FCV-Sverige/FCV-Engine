using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[SelectionBase]
public class MovingPlatform : MonoBehaviour
{
    public enum LoopType
    {
        LOOP,
        PINGPONG
    }
    
    [SerializeField] private LoopType loopType;
    private bool pingPongReversing = false;
    

    [HideInInspector, SerializeField] private Vector3[] worldPoints;

    public Vector3[] WorldPoints => worldPoints;
    
    public float speed = 5f; // Speed in units per second

    private int currentPointIndex = 0; // Index of the current target point
    private Vector3 currentTarget; // Current target point
    private Vector3 previousPosition;
    private float journeyLength; // Distance to the next point
    private float timeSinceStart; // Time when the movement started

    private List<float> totalTravelLength;

    private void Awake()
    {
        if (worldPoints.Length == 0 || worldPoints == null)
        {
            Debug.LogError($"No points defined for movement on {name}");
            EditorApplication.ExitPlaymode();
            return;
        }

        currentTarget = worldPoints[0];
    }

    private void Start()
    {
        // Initialize the first target
        SetNextTarget();
    }



    private void Update()
    {
        if (worldPoints.Length == 0)
            return;

        // Calculate the distance covered based on time
        timeSinceStart += Time.deltaTime;

        float finalSpeed = journeyLength / speed;

        float T = timeSinceStart / finalSpeed;

        T = Mathf.Clamp01(T);

        // Move the object to the current target
        transform.position = Vector3.Lerp(previousPosition, currentTarget, T);

        // Check if the object has reached the target
        if (T > 0.99f)
        {
            // Set the next target
            SetNextTarget();
        }
    }

    private void SetNextTarget()
    {
        currentPointIndex = GetNextIndex(currentPointIndex, worldPoints.Length, loopType); // Loop through the points
        previousPosition = currentTarget;
        currentTarget = worldPoints[currentPointIndex];
        timeSinceStart = 0;
        journeyLength = Vector3.Distance(previousPosition, currentTarget);
    }
    

    private int GetNextIndex(int currentIndex, int arrayLength, LoopType movementType)
    {
        if (movementType == LoopType.LOOP)
        {
            // Loop behavior: Move to the next index, wrapping around to 0 if needed
            return (currentIndex + 1) % arrayLength;
        }
        
        if (movementType == LoopType.PINGPONG)
        {
            // Ping-Pong-behavior: Move to the next index, reversing direction at the ends
            if (!pingPongReversing)
            {
                if (currentIndex + 1 >= arrayLength)
                {
                    // Reverse direction at the end
                    pingPongReversing = true;
                    return currentIndex - 1;
                }

                return currentIndex + 1;
            }

            if (currentIndex - 1 < 0)
            {
                // Reverse direction at the start
                pingPongReversing = false;
                return currentIndex + 1;
            }

            return currentIndex - 1;
        }
        
        

        return currentIndex; // Default case (should not be reached)
    }
}


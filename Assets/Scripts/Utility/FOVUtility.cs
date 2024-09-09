#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class FOVUtility
{
#if UNITY_EDITOR
    
    // Helper function to draw the FOV as a frustum
    /// <summary>
    /// This functions draws an arc based on specified values. SHOULD ONLY BE USED IN OnDrawGizmos/OnDrawGizmosSelected
    /// </summary>
    /// <param name="origin">start point</param>
    /// <param name="direction">start direction of arc</param>
    /// <param name="maxDistance">distance of arc</param>
    /// <param name="fovAngle">max angle of arc</param>
    public static void DrawFOV(Vector3 origin, Vector3 direction, float maxDistance, float fovAngle)
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
#endif
    
    // Function to check if the target is within FOV and range of the source
    public static bool IsWithinFOVAndRange(Vector3 source, Vector3 target, Vector3 direction, float maxDistance, float fovAngle)
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

        return IsWithinFOV(source, target, direction, fovAngle);
    }

    public static bool IsWithinFOV(Vector3 source, Vector3 target, Vector3 direction, float fovAngle)
    {
        // Calculate the vector from source to target
        Vector3 toTarget = target - source;
        
        // Normalize the direction and toTarget vectors
        Vector3 normalizedDirection = direction.normalized;
        Vector3 normalizedToTarget = toTarget.normalized;

        // Calculate the angle between the direction and toTarget vectors
        float angle = Vector3.Angle(normalizedDirection, normalizedToTarget);

        // Check if the target is within the FOV angle
        return angle <= fovAngle / 2;
    }
    
    
}

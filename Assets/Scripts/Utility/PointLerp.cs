using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointLerp
{
    public static Vector3 LerpPoints(Vector3[] points, float t)
    {
        if (points == null || points.Length == 0)
        {
            Debug.Log("Points array is null or empty.");
            return Vector3.zero;
        }
        
        if (points.Length == 1)
        {
            return points[0];
        }

        // Ensure t is clamped between 0 and 1
        t = Mathf.Clamp01(t);

        // Calculate the total number of segments
        int numSegments = points.Length - 1;

        // Calculate which segment t falls into
        float segmentLength = 1.0f / numSegments;
        int currentSegment = Mathf.Min(Mathf.FloorToInt(t / segmentLength), numSegments - 1);

        // Calculate local t within the current segment
        float segmentT = (t - currentSegment * segmentLength) / segmentLength;

        // Interpolate between the start and end points of the current segment
        return Vector3.Lerp(points[currentSegment], points[currentSegment + 1], segmentT);
    }
}
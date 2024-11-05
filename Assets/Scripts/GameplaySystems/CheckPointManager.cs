using UnityEngine;

public static class CheckPointManager
{
    public static readonly float CheckPointAcquiredDistance = 1;

    private static Vector2 lastCheckPoint;

    public static void PlaceAtCheckPoint(Transform playerTransform)
    {
        playerTransform.position = lastCheckPoint;
    }

    public static void CheckPointReached(Vector2 position)
    {
        lastCheckPoint = position;
    }
}

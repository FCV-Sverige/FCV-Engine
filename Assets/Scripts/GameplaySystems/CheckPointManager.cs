using UnityEngine;

public static class CheckPointManager
{
    public const float CheckPointAcquiredDistance = 1;

    private static Vector2 lastCheckPoint;
    
    /// <summary>
    /// places playerTransform at the latest checkpoint stored
    /// </summary>
    /// <param name="playerTransform">transform to place at latest checkpoint</param>
    public static void PlaceAtCheckPoint(Transform playerTransform)
    {
        playerTransform.position = lastCheckPoint;
    }
    
    /// <summary>
    /// Stores the latest checkpoint position reached
    /// </summary>
    /// <param name="position">checkpoint position</param>
    public static void CheckPointReached(Vector2 position)
    {
        lastCheckPoint = position;
    }
}

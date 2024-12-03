using System;
using UnityEngine;

/// <summary>
/// A checkpoint that will constantly check distance to player and if close enough add itself as latest CheckPointReached in CheckPointManager class
/// </summary>
public class CheckPoint : MonoBehaviour
{
    private Transform playerTransform;

    /// <summary>
    /// Gets player transform from static field in Movement class and sets player position to be the latest checkpoint reached
    /// </summary>
    private void Start()
    {
        playerTransform = Movement.PlayerTransform;
        
        CheckPointManager.CheckPointReached(playerTransform.position);
    }

    /// <summary>
    /// Checks if the player is close enough to trigger checkpoint reached; sets it's position to latest and destroys itself  
    /// </summary>
    private void Update()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) > CheckPointManager.CheckPointAcquiredDistance) return;
        
        CheckPointManager.CheckPointReached(transform.position);
        Destroy(this.gameObject);
    }
}

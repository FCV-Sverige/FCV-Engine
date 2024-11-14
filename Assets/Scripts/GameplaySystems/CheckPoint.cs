using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Transform playerTransform;


    private void Start()
    {
        playerTransform = Movement.PlayerTransform;
    }

    // Update is called once per frame and checks if the player is close enough
    private void Update()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) > CheckPointManager.CheckPointAcquiredDistance) return;
        
        CheckPointManager.CheckPointReached(transform.position);
        Destroy(this.gameObject);
    }
}

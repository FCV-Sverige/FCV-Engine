using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Transform playerTransform;


    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) > CheckPointManager.CheckPointAcquiredDistance) return;
        
        CheckPointManager.CheckPointReached(transform.position);
        Destroy(this.gameObject);
    }
}

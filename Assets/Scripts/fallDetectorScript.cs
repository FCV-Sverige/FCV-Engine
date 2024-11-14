using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Class that resets place if it falls below this gameobjects y position
/// </summary>
public class FallDetectorScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    void Update()
    {
        if (!playerTransform) return;
        
        if (playerTransform.transform.position.y > transform.position.y) return;
            
        CheckPointManager.PlaceAtCheckPoint(playerTransform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(new Vector2(-200, transform.position.y), new Vector3(200, transform.position.y));
    }
#endif

    
}

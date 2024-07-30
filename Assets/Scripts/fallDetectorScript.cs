using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fallDetectorScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float yPositionThreshold = -20;

    void Update()
    {
        if (playerTransform != null)
        {
            if (playerTransform.transform.position.y <= yPositionThreshold)
                SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(new Vector2(-200, yPositionThreshold), new Vector3(200, yPositionThreshold));
    }
#endif

    
}

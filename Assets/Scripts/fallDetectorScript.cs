using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fallDetectorScript : MonoBehaviour
{
    [SerializeField] private CheckPointManager checkPointManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float yPositionThreshold = -20;

    private void Awake()
    {
        if (!checkPointManager)
            checkPointManager = FindObjectOfType<CheckPointManager>();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            if (playerTransform.transform.position.y > yPositionThreshold) return;

            if (checkPointManager)
            {
                checkPointManager.PlaceAtCheckPoint();
                return;
            }
            
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

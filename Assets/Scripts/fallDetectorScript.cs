using UnityEngine;

public class fallDetectorScript : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            // Update the position of the fallDetector to follow the player's X-axis only
            transform.position = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
        }
    }

    
}

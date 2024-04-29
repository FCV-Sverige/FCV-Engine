using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float offset;
    public float offsetSmoothing;
    private Vector3 playerPosition;
    private Vector3 lastPlayerPosition; // Added to keep track of the last position

    // Start is called before the first frame update
    void Start()
    {
        lastPlayerPosition = player.transform.position; // Initialize the last position
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerPosition = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 deltaPosition = player.transform.position - lastPlayerPosition; // Calculate the change in position

        if (player.transform.localScale.x > 0f && deltaPosition.x > 0f)
        {
            // Apply the offset only if the player is facing and moving right
            playerPosition = new Vector3(playerPosition.x + offset, playerPosition.y, playerPosition.z);
        }
        else if (player.transform.localScale.x < 0f && deltaPosition.x < 0f)
        {
            // Apply the offset only if the player is facing and moving left
            playerPosition = new Vector3(playerPosition.x - offset, playerPosition.y, playerPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, playerPosition, offsetSmoothing * Time.deltaTime);
        lastPlayerPosition = player.transform.position; // Update the last position for the next frame
    }
}

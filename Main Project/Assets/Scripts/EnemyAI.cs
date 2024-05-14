using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints; // Array to hold the waypoints
    public float speed = 2f;      // Speed at which the enemy moves
    private int waypointIndex = 0; // Current waypoint index

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Transform's position towards the current waypoint at the set speed
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, speed * Time.deltaTime);

        // Check if the enemy has reached the waypoint
        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) < 0.1f)
        {
            // Increment the waypoint index, reset if at the last waypoint
            if (waypointIndex == waypoints.Length - 1)
                waypointIndex = 0;
            else
                waypointIndex++;
        }
    }
}

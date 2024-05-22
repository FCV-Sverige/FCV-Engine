using System;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;
    public int damage = 10; // Damage to the player on collision

    private int currentPatrolIndex;
    private float waitTimer;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public float obstacleCheckDistance = 0.5f;
    private bool isGrounded;

    private bool facingRight = true; // To keep track of the sprite direction

    private Rigidbody2D rb;
    private PlayerController playerController;


    void Start()
    {
        currentPatrolIndex = 0;
        waitTimer = waitTimeAtPoint;
        rb = GetComponent<Rigidbody2D>();
        playerController = FindObjectOfType<PlayerController>(); // Find the player controller in the scene
    }

    void Update()
    {
        CheckGroundStatus();
        Patrol();

        if (playerController != null && playerController.gameObject.activeInHierarchy)
        {
            bool enemyHit = Physics2D.OverlapCircle(playerController._groundCheck.position, playerController.groundCheckRadius, playerController.enemyLayer);
            if (enemyHit)
            {
                Die();
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points set.");
            return;
        }

        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (patrolPoint.position - transform.position).normalized;

        // Apply movement
        rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y);

        // Flip sprite based on movement direction
        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }

        if (Vector2.Distance(transform.position, patrolPoint.position) < 0.2f) // Adjusted distance tolerance
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                waitTimer = waitTimeAtPoint;
            }
        }
        else
        {
            waitTimer = waitTimeAtPoint; // Reset the timer if not at the patrol point
        }

        if (IsObstacleInFront() && isGrounded)
        {
            Jump();
        }
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1 to flip the sprite
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    bool IsObstacleInFront()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, direction * obstacleCheckDistance, Color.red); // Visualize raycast
        return hit.collider != null;
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (patrolPoints == null) return;

        Gizmos.color = Color.green;
        foreach (Transform patrolPoint in patrolPoints)
        {
            Gizmos.DrawWireSphere(patrolPoint.position, 0.3f);
        }
    }
}

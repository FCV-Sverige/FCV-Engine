using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking }
    public EnemyState currentState = EnemyState.Patrolling;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Chase Settings")]
    public Transform target;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int damage = 10;

    private int currentPatrolIndex;
    private float waitTimer;
    private float attackTimer;

    private PlayerController playerController;

    void Start()
    {
        currentPatrolIndex = 0;
        waitTimer = waitTimeAtPoint;
        attackTimer = 0f;
        playerController = target.GetComponent<PlayerController>();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }

        DetectPlayer();
    }

    void Patrol()
    {
        // Patrol logic...
    }

    void Chase()
    {
        if (target == null) return;
        transform.position = Vector2.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
    }

    void Attack()
    {
        if (playerController == null) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            playerController.TakeDamage(damage);
            attackTimer = attackCooldown;
        }

        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            currentState = EnemyState.Chasing;
        }
    }

    void DetectPlayer()
    {
        if (target == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

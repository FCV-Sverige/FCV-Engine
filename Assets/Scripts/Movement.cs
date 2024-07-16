using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    public static Transform PlayerTransform { get; private set; } 
    
    #region Run
    [Header("Run Settings")]
    [Space(10)]
    [SerializeField] public float maxSpeed;
    [SerializeField] public float runAccelRate, runDecelRate;
    [SerializeField] public float runAccelRateInAir, runDecelRateInAir;
    #endregion
    [Space(20)]

    #region Jump
    [Header("Jump Settings")]
    [Space(10)]

    [SerializeField] public float jumpForce;
    [SerializeField] public float jumpCoyoteTime;
    [SerializeField] public float jumpBufferTime;
    #endregion
    [Space(20)]

    #region Gravity scale
    [Header("Gravity Settings")]
    [Space(10)]

    [SerializeField] public float gravityScale;
    [SerializeField] public float fastFallingGravityMult;

    #endregion
    
    [FormerlySerializedAs("ground")] [SerializeField] LayerMask groundLayer;
    [FormerlySerializedAs("overlapBox")] [SerializeField] private Vector2 overlapBoxSize;
    [SerializeField] private Vector2 overlapBoxOffset;
    [SerializeField] private string horizontalAxisName;
    [SerializeField] private KeyCode jumpButton;
    private Vector2 moveInputs = new Vector2();
    private bool isGrounded;
    private bool isJumping;
    private bool jumpPressed;


    private float lastGroundedTime;
    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        PlayerTransform = transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        lastGroundedTime -= Time.deltaTime;

        moveInputs.x = Input.GetAxis(horizontalAxisName);

        isGrounded = Physics2D.OverlapBox((Vector2)transform.position + overlapBoxOffset, overlapBoxSize, 0, groundLayer);
        if (isGrounded) 
        {
            lastGroundedTime = jumpCoyoteTime;
            isJumping = false;
        }

        SetAnimation();
        Flip();
        GravityScaling();
    }

    private void FixedUpdate()
    {
        Run();

        if (canJump() && !isJumping && Input.GetKey(jumpButton))
            Jump();
    }

    private void Run()
    {
        float targetSpeed = moveInputs.x * maxSpeed;

        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelRate : runDecelRate;

        accelRate = (Mathf.Abs(targetSpeed) > 0.01f && isJumping) ? runAccelRateInAir : runDecelRateInAir;

        float movement = speedDiff * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        float force = jumpForce;
        rb.velocity = new Vector2(rb.velocity.x, 0);

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        lastGroundedTime = 0;
        isJumping = true;
    }

    private void GravityScaling()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fastFallingGravityMult * gravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void SetAnimation()
    {
        animator.SetFloat("speed", Mathf.Abs(moveInputs.x));
    }

    private bool canJump()
    {
        return lastGroundedTime > 0;
    }

    private void Flip()
    {
        if (moveInputs.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }

        if (moveInputs.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + overlapBoxOffset, overlapBoxSize);
    }
}
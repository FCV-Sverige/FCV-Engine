using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [Header("Character settings")]
    public int Health;


    [Header("Layer Settings")]
    [Tooltip("Set this to the layer of your player")]
    public LayerMask playerLayer;

    [Header("Input Settings")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keyboard parity.")]
    public bool snapInput = true;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float horizontalDeadZoneThreshold = 0.1f;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float verticalDeadZoneThreshold = 0.3f;

    [Header("Movement Settings")]
    [Tooltip("Radius for checking ground from ground-check object")]
    public float groundCheckRadius = 0.1f;

    [Tooltip("The top horizontal movement speed")]
    public float maxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float groundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float airDeceleration = 30;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float grounderDistance = 0.05f;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float groundingForce = -1.5f;

    [Header("Jump Settings")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float jumpPower = 36;

    [Tooltip("The maximum vertical movement speed")]
    public float maxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float fallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float jumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float coyoteTime = 0.15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float jumpBuffer = 0.2f;

    [SerializeField] private Transform _groundCheck;  // Transform indicating where to check for ground

    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private bool _grounded;
    private float _time;
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private FrameInput _frameInput;

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (snapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < horizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < verticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleJump();
        HandleDirection();
        HandleGravity();
        ApplyMovement();
    }

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        bool groundHit = Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, ~playerLayer);

        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (_jumpToConsume && (_grounded || (_coyoteUsable && !_grounded && _time < _timeJumpWasPressed + coyoteTime)))
        {
            ExecuteJump();
            _jumpToConsume = false;
        }
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = jumpPower;
        Jumped?.Invoke();
    }

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? groundDeceleration : airDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = groundingForce;
        }
        else
        {
            var inAirGravity = fallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= jumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement()
    {
        _rb.velocity = _frameVelocity;
    }
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    Vector2 FrameInput { get; }
    event Action<bool, float> GroundedChanged;
    event Action Jumped;
}

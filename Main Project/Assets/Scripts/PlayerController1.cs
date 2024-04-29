using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MovementController
{

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private GameStats _gameStats;

        DeathCountScript _deathCountScript;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        public Transform GroundCheck;
        public Vector3 RespawnPoint;

        //coyote jump variables
        private float _initalCoyoteTime;
        private float _initalCoyoteTimeFailWindow;
        public  int _attemptedCoyoteTime = 0;
        public int _missedCoyoteTimes = 0;
        public float _coyoteJumpSuccessRate;

        //buffered jump variables
        public int _attemptedBufferedJumps = 0;
        public int _prematureBufferedJumpAttempts = 0;
        public float _bufferedJumpSuccessRate;



        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;
        private float _standingOnGroundTime;

        void UpdateGameStats()
        {
            //if (_gameStats == null) return;

            //var statsMapping = new Dictionary<Action<GameStats>, int>
            //{
            //    { stats => stats.attemptedCoyote_ = _attemptedCoyoteTime, _attemptedCoyoteTime },
            //    { stats => stats.missedCoyoteTimes_ = _missedCoyoteTimes, _missedCoyoteTimes },
            //    { stats => stats.coyoteJumpSuccessRate_ = _coyoteJumpSuccessRate, (int)_coyoteJumpSuccessRate },
            //    { stats => stats.attemptedBufferedJumps_ = _attemptedBufferedJumps, _attemptedBufferedJumps },
            //    { stats => stats.prematureBufferedJumpAttempts_ = _prematureBufferedJumpAttempts, _prematureBufferedJumpAttempts },
            //    { stats => stats.bufferedJumpSuccessRate_ = _bufferedJumpSuccessRate, (int)_bufferedJumpSuccessRate },
            //    { stats => stats.totalDeaths_ = _deathCountScript.totalDeaths, _deathCountScript.totalDeaths },
            //    { stats => stats.meteorDeaths_ = _deathCountScript.meteorDeaths, _deathCountScript.meteorDeaths },
            //    { stats => stats.fallDeaths_ = _deathCountScript.fallDeaths, _deathCountScript.fallDeaths },
            //    { stats => stats.platformDeath_ = _deathCountScript.platformDeath, _deathCountScript.platformDeath }
            //};

            //foreach (var mapping in statsMapping)
            //{
            //    mapping.Key(_gameStats);
            }
        }

        private void OnApplicationQuit()
        {
            ResetCoyoteTime();
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _deathCountScript = GameObject.Find("DeathCountObject").GetComponent<DeathCountScript>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Start()
        {
            RespawnPoint = transform.position;
            _initalCoyoteTime = _stats.CoyoteTime;
            _initalCoyoteTimeFailWindow = _stats.CoyoteTimeFailWindow;
        }
        //void OnEnable()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        //void OnDisable()
        //{

        //    SceneManager.sceneLoaded -= OnSceneLoaded;
        //}

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ResetCoyoteTime();
        }

        private void Update()
        {
            _time += Time.deltaTime;
            UpdateGameStats();
            GatherInput();
            UpdateCoyoteTimeJumpAttempts();
            UpdateStandingTime();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
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

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;
            bool groundCheck = Physics2D.OverlapCircle(GroundCheck.position, _stats.GroundCheckRadius,_stats.GroundLayer);

            // Landed on the Ground
            if (!_grounded && groundCheck )
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundCheck)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }
            else if (_grounded && !groundCheck && _jumpToConsume)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }


            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void UpdateCoyoteTimeJumpAttempts()
        {
            if (!_grounded)
            {
                if (_stats.CoyoteTime > 0)
                {
                    _stats.CoyoteTime -= Time.deltaTime;
                }
                else if (_stats.CoyoteTimeFailWindow > 0)
                {
                    _stats.CoyoteTimeFailWindow -= Time.deltaTime;
                }
            }
            else 
            {
                ResetCoyoteTime();
            }

        }
        private void ResetCoyoteTime()
        {
            _stats.CoyoteTime = _initalCoyoteTime;
            _stats.CoyoteTimeFailWindow = _initalCoyoteTimeFailWindow;
        }

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) // Check jump height
            {
                _endedJumpEarly = true;
            }

            if (_jumpToConsume)
            {
                if (_grounded && CanUseCoyote ) // Normal Jump
                {
                    ExecuteJump();
                    _jumpToConsume = false;
                    return;
                }
                else if (_coyoteUsable && !_grounded && _stats.CoyoteTime > 0) // Coyote Jump
                {
                    ExecuteJump();
                    _attemptedCoyoteTime++;
                    _jumpToConsume = false;
                    return;
                }
                else if (_coyoteUsable && _stats.CoyoteTime <= 0 && _stats.CoyoteTimeFailWindow > 0)
                {
                    LogFailedCoyoteJumpOpportunity();
                    _jumpToConsume = false;
                    return;
                }
                if (_grounded) // Perform buffer jump
                {
                    ExecuteJump();
                    _jumpToConsume = false;
                }
                _jumpToConsume = false;
            }

            if (_grounded && HasBufferedJump && _timeJumpWasPressed > 0) // Perform buffer jump
            {
                ExecuteJump();
                _attemptedBufferedJumps++;
                _timeJumpWasPressed = 0;
                _jumpToConsume = false;

                return;
            }
            else if (_timeJumpWasPressed > 0 && _grounded && !HasBufferedJump && _time < (_timeJumpWasPressed + (_stats.PrematureJumpBufferWindow + _stats.JumpBuffer)))
            {
                LogFailedBufferedJumpOpportunity();
            }
        }

        private void LogFailedCoyoteJumpOpportunity()
        {
            if (_jumpToConsume)
            {
                _attemptedCoyoteTime++;
                _missedCoyoteTimes++;
                _jumpToConsume = false;  // Prevents re-logging the same jump attempt.
            }
        }
        private void LogFailedBufferedJumpOpportunity()
        {
            // Log the premature buffered jump attempt
            _prematureBufferedJumpAttempts++;
            _attemptedBufferedJumps++;
            _timeJumpWasPressed = 0;
            _jumpToConsume = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "FallDetector" )
            {
                ResetCoyoteTime();
                transform.position = RespawnPoint;
                _deathCountScript.AddDeaths();
                _deathCountScript.AddFallDeath();

            }
            if(collision.gameObject.tag == "Meteor")
            {
                ResetCoyoteTime();
                transform.position = RespawnPoint;
                _deathCountScript.AddDeaths();
                _deathCountScript.AddMeteorDeath();
            }
        }

       
        private void UpdateStandingTime()
        {
            // Check if the player is grounded and not moving horizontally
            if (_grounded) 
            {
                _standingOnGroundTime += Time.deltaTime;

                if (_standingOnGroundTime >= _stats.MaxStandStillTime)
                {
                    ResetCoyoteTime();
                    transform.position = RespawnPoint;
                    _standingOnGroundTime = 0;
                    _deathCountScript.AddDeaths();
                    _deathCountScript.AddPlatformDeath();

                }
            }
            else
            {
                _standingOnGroundTime = 0; // Reset if not grounded
            }
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _standingOnGroundTime = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion
        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }

    
}

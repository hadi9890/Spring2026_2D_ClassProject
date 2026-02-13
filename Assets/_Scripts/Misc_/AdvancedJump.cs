using UnityEngine;

namespace _Scripts.Misc_
{
    public class AdvancedJump : MonoBehaviour
    {
        private Rigidbody2D _rb;
        // private Animator _anim;
        private MyFirstScript _playerMovement;
    
        [Space(10), Header("Jump Variables")]
        [SerializeField] private float jumpPower = 11;
    
        [Tooltip("Maximum number of jumps the player can perform before landing again.")]
        [SerializeField] private float maxJumpCount = 2;
    
        [Tooltip("Minimum time between jumps to prevent spam.")]
        [SerializeField] private float jumpCooldown = 0.1f;
    
        private float _jumpCooldownTimer;
        private float _currJump;
    
        [Space(5), Header("Variable Jump Heights")]
        [Tooltip("Multiplier applied to gravity while falling to speed up descent.")]
        [SerializeField] private float fallMultiplier = 2.7f;
    
        [Tooltip("Multiplier for gravity when jump is released early, allowing short hops.")]
        [SerializeField] private float lowJumpMultiplier = 3f;
    
        [Space(5), Header("Coyote Time")]
        [Tooltip("Short window that allows the player to still jump after walking off a ledge.")]
        [SerializeField] private float coyoteTime = 0.2f;
    
        private float _coyoteTimeCounter;
    
        [Space(5), Header("Jump Buffer")]
        [Tooltip("Short window that queues a player's jump if pressed before landing.")]
        [SerializeField] private float jumpBufferTime = 0.2f;
    
        private float _jumpBufferCounter;
    
        [Space(10), Header("Hang Time")]
        [Tooltip("Short window that suspends the player at the apex of the jump.")]
        [SerializeField] private float hangTimeDuration = 0.15f;
    
        [Tooltip("How close to zero Y velocity must be to trigger hang time.")]
        [SerializeField] private float hangTimeVelocityThreshold = 1.0f;
    
        [SerializeField] private float jumpHoldThreshold = 0.15f;
        private float _hangTimeCounter;
        private float _jumpHoldTimer;
        private bool _eligibleForHangTime;
        private bool _isInHangTime;
    
        #region Anim Hashes
    
        private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    
        #endregion
    
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<MyFirstScript>();
        }
    
        private void Update()
        {
            if (_jumpCooldownTimer > 0)
            {
                _jumpCooldownTimer -= Time.deltaTime;
            }
    
            // Coyote Time
            if (_playerMovement.isGrounded && !Input.GetButtonDown("Jump"))
            {
                _coyoteTimeCounter = coyoteTime;
                _currJump = 0;
                _isInHangTime = false;
            }
            else
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }
    
            // Jump Buffer
            if (Input.GetButtonDown("Jump"))
            {
                _jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
    
            // Handle Hang Time trigger
            if (!_isInHangTime && _rb.velocity.y > 0)
            {
                if (Input.GetButton("Jump"))
                {
                    _jumpHoldTimer += Time.deltaTime;
    
                    if (_jumpHoldTimer >= jumpHoldThreshold)
                    {
                        _eligibleForHangTime = true;
                    }
                }
                else
                {
                    _jumpHoldTimer = 0;
                }
            }
    
            // Handle Jump
            if (_jumpBufferCounter > 0f && _jumpCooldownTimer <= 0)
            {
                if (_coyoteTimeCounter > 0f || _currJump < maxJumpCount - 1)
                {
                    Jump();
                    _jumpBufferCounter = 0f;
                    _jumpCooldownTimer = jumpCooldown;
                }
            }
        }
    
        private void FixedUpdate()
        {
            // Hang Time
            if (_eligibleForHangTime && !_playerMovement.isGrounded && !_isInHangTime &&
                Mathf.Abs(_rb.velocity.y) < hangTimeVelocityThreshold && _hangTimeCounter <= 0f)
            {
                _isInHangTime = true;
                _hangTimeCounter = hangTimeDuration;
            }
    
            if (_isInHangTime)
            {
                _hangTimeCounter -= Time.fixedDeltaTime;
    
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * 0.1f * Time.fixedDeltaTime);
    
                if (_hangTimeCounter <= 0f)
                {
                    _isInHangTime = false;
                }
            }
            else
            {
                if (_rb.velocity.y < 0)
                {
                    _rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
                }
                else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    _rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
                }
            }
    
            // if (_playerMovement.isGrounded)
            // {
            //     _anim.SetFloat(YVelocity, 0);
            //     return;
            // }
            //     
            // _anim.SetFloat(YVelocity, _rb.velocity.y);
        }
    
        private void Jump()
        {
            _currJump++;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            _coyoteTimeCounter = 0;
            _jumpHoldTimer = 0f;
            _hangTimeCounter = 0;
            _isInHangTime = false;
            _eligibleForHangTime = false;
        }
    }
}

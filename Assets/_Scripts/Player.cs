using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class Player : MonoBehaviour, IDamageable
    {
        private float _xInput;
        [SerializeField] private float speed;
    
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GameObject groundCheck;
        [SerializeField] private float groundCheckRadius;
        [HideInInspector] public bool isGrounded;
    
        [SerializeField] private List<AudioClip> footstepsSfx;

        [Space(5), Header("Health")]
        [SerializeField] private int maxHealth;
        private int _currHealth;
        
        [Space(5), Header("Combat")]
        [SerializeField] private Transform attackBox;
        public Vector2 attackSize;
        public LayerMask damageableLayer;
        [SerializeField] private int baseAttackDamage = 1;

        #region Components
    
        private Rigidbody2D _rb;
        private Animator _anim;
        private AudioSource _audioSource;

        #endregion

        #region Animation Hashes
    
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int Hit = Animator.StringToHash("Hit");

        #endregion

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _currHealth = maxHealth;
        }

        private void Update()
        {
            _xInput = Input.GetAxisRaw("Horizontal");

            _anim.SetBool(IsMoving, _xInput != 0);

            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);
            CheckCharacterFlip();
        
            if (Input.GetKeyDown(KeyCode.J))
            {
                _anim.SetTrigger(IsAttacking);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.instance.PauseResumeGame();
            }
        }
    
        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_xInput * speed, _rb.velocity.y);
        }

        public void FootstepSFX()
        {
            if (!isGrounded) return;
            var clip = footstepsSfx[Random.Range(0, footstepsSfx.Count)];
            _audioSource.PlayOneShot(clip);
        }

        private void CheckCharacterFlip()
        {
            var characterScale = transform.localScale;

            if (_xInput < 0)
            {
                characterScale.x = -Mathf.Abs(transform.localScale.x);
            } else if (_xInput > 0)
            {
                characterScale.x = Mathf.Abs(transform.localScale.x);
            }
        
            // if "_xInput" is 0, no need to change the scale
            // This way, we keep the player facing the last moving direction
        
            transform.localScale = characterScale;
        }

        // This function is called as an animation event instead of enabling/disabling the hitbox like before
        // Using this method, we can safely remove the BoxCollider component from the Hitbox gameObject
        public void PerformAttack()
        {
            // Create a box and check if there are objects with the damageable Layer assigned
            var hitObjects = Physics2D.OverlapBoxAll(attackBox.position, attackSize, 0, damageableLayer);
            foreach (var hitObject in hitObjects)
            {
                // Check for the IDamageable interface in the hit objects and apply damage
                var damageable = hitObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // here we used Mathf.Abs and added a minus before it to guarantee the object will
                    // not have its health increased if the value of "_baseAttackDamage" was set to positive by mistake
                    damageable.ChangeHealth(-Mathf.Abs(baseAttackDamage));
                }
            }
        }

        // This method will draw a cube with the dimensions of our attack
        // When we select the player in the Unity Editor, you will see a red box that shows the size of the attack
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackBox.transform.position, attackSize);
        }

        public virtual void ChangeHealth(int amount)
        {
            // Restrict the changed health value between 0 and "maxHealth"
            _currHealth = Mathf.Clamp(_currHealth + amount, 0, maxHealth);
            // Debug.Log("Changed the health of " + gameObject.name + " to: " + _currHealth);
            
            // If we're decreasing health, play the hit animation
            if (amount <= 0)
            {
                _anim?.SetTrigger(Hit);
            }
        }
    }
}
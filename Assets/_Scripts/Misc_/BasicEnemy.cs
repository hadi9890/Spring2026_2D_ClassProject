using _Scripts.Interfaces;
using _Scripts.Lecture08;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Misc_
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        [FormerlySerializedAs("_enemyData")] [SerializeField] private EnemyData enemyData;
        private int _currHealth;
        private bool _isDead;
        [SerializeField] private Transform groundWallCheck;
        private bool _isFacingRight;

        #region Components

        private Animator _anim;
        private Rigidbody2D _rb;
        private BoxCollider2D _collider;

        #endregion

        #region Animation Hashes

        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Death = Animator.StringToHash("Death");

        #endregion

        #region Life Cycle Functions

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _currHealth = enemyData.health;
        }

        private void FixedUpdate()
        {
            _rb.velocity =
                new Vector2(_isFacingRight ? Mathf.Abs(enemyData.moveSpeed) : -Mathf.Abs(enemyData.moveSpeed),
                    _rb.velocity.y);

            // Wall Detection Raycast
            var wallDetect = Physics2D.Raycast(groundWallCheck.position, _isFacingRight ? Vector2.right : Vector2.left,
                enemyData.wallDetectionDistance, enemyData.groundLayer);

            // Edge Detection Raycast
            var edgeDetect = Physics2D.Raycast(groundWallCheck.position, Vector2.down,
                enemyData.edgeDetectionDistance, enemyData.groundLayer);

            if (wallDetect || !edgeDetect)
            {
                FlipCharacter();
            }
        }

        #endregion

        private void FlipCharacter()
        {
            _isFacingRight = !_isFacingRight;
            var characterScale = transform.localScale;

            if (!_isFacingRight)
            {
                characterScale.x = -Mathf.Abs(transform.localScale.x);
            }
            else if (_isFacingRight)
            {
                characterScale.x = Mathf.Abs(transform.localScale.x);
            }

            transform.localScale = characterScale;
        }

        public void ChangeHealth(int amount)
        {
            // Restrict the changed health value between 0 and "maxHealth"
            _currHealth = Mathf.Clamp(_currHealth + amount, 0, enemyData.health);
            // Debug.Log("Changed the health of " + gameObject.name + " to: " + _currHealth);

            // If we're decreasing health and enemy health still hasn't reached 0, play the hit animation
            if (amount <= 0 && _currHealth != 0)
            {
                _anim.SetTrigger(Hit);
            }

            // If enemy's health reached 0, trigger the death animation
            if (_currHealth == 0)
            {
                EnemyDeath();
            }
        }

        private void EnemyDeath()
        {
            _isDead = true;
            _anim.SetTrigger(Death);
            _rb.bodyType = RigidbodyType2D.Static; // Set enemy rigidbody to static so it doesn't get pushed by player
            _collider.isTrigger = true; // set enemy's collider to a trigger, allowing player to walk over the dead body
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!_isDead) // Add a boolean to not damage the player when enemy is already dead
                {
                    // Get the IDamageable from the player and deduct their health
                    var damageable = other.gameObject.GetComponent<IDamageable>();
                    damageable.ChangeHealth(-1);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(groundWallCheck.position, Vector2.down * enemyData.edgeDetectionDistance);
            Gizmos.DrawRay(groundWallCheck.position, _isFacingRight? Vector2.right : Vector2.left * enemyData.wallDetectionDistance);
        }
    }
}
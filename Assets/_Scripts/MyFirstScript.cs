using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    private float _xInput;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float groundCheckRadius;
    [HideInInspector] public bool isGrounded;
    private Animator _anim;

    [SerializeField] private GameObject bulletPrefab;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");

        _anim.SetBool(IsMoving, _xInput != 0);

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(bulletPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
    }
    
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(_xInput * speed, rb.velocity.y);
    }
}
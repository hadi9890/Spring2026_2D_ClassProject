using System;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    private float xInput;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int maxJumpCount;
    private int _currJump;

    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            _currJump = 0;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(bulletPrefab, transform.position + new Vector3(1,0, 0), Quaternion.identity);
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && _currJump < maxJumpCount)
        {
            Jump();
        }
    }

    
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Platform"))
    //     {
    //         Destroy(other.gameObject, 1);
    //     }
    // }


    private void Jump()
    {
        _currJump++;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
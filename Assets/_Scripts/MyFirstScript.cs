using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyFirstScript : MonoBehaviour
{
    private float _xInput;
    [SerializeField] private float speed;
    private Rigidbody2D _rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float groundCheckRadius;
    [HideInInspector] public bool isGrounded;
    [SerializeField] private GameObject _attackbox;
    [SerializeField] private List<AudioClip> footstepsSFX;
    private Animator _anim;
    private AudioSource _audioSource;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _attackbox.SetActive(false);
    }

    private void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");

        _anim.SetBool(IsMoving, _xInput != 0);

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.J))
        {
            _anim.SetTrigger("isAttacking");
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

    public void EnableHitbox()
    {
        _attackbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        _attackbox.SetActive(false);
    }

    public void FootstepSFX()
    {
        var clip = footstepsSFX[Random.Range(0, footstepsSFX.Count)];
        _audioSource.PlayOneShot(clip);
    }
    
    
    
    
    
    
    
    
    
    
}
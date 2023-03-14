using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private float inputHorizontal;
    private float moveSpeed = 400f;
    private Vector2 totalVelocity;
    private bool canJump = false;
    private bool facingRight = true;

    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip gemSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Flip();
    }

    void MovePlayer()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        totalVelocity = new Vector2(inputHorizontal * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(inputHorizontal));
        if (inputHorizontal != 0)
        {
            rb.velocity = totalVelocity;
        }
    }

    void Flip()
    {
        if ((inputHorizontal > 0 && !facingRight) || (inputHorizontal < 0 && facingRight))
        {
            sr.flipX = !sr.flipX;
            facingRight = !facingRight;
        }
    }
    
    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (canJump)
            {
                audioSource.clip = jumpSound;
                audioSource.Play();
                // Adding additional force on x-axis if player is moving and then jump key is pressed
                rb.AddForce(new Vector2(2000f * inputHorizontal, 2000f), ForceMode2D.Force);
                animator.SetBool("JumpAnimate", true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Platform"))
        {
            canJump = true;
            animator.SetBool("JumpAnimate", false);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Platform"))
        {
            canJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Gem"))
        {
            audioSource.clip = gemSound;
            audioSource.Play();
            Destroy(col.gameObject);
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

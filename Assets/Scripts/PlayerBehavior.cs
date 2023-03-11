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
    private float moveSpeed = 500f;
    private Vector2 totalVelocity;
    private bool canJump = false;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
                rb.AddForce(new Vector2(0f, 800f));
                animator.SetBool("JumpAnimate", true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            animator.SetBool("JumpAnimate", false);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("LevelEnd"))
        {
            SceneManager.LoadScene("Level 1");
        }
        
        if (col.gameObject.CompareTag("Gem"))
        {
            Destroy(col.gameObject);
        }
    }
}

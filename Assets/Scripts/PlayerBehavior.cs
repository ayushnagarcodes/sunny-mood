using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private float inputHorizontal;
    private float moveSpeed = 500f;
    private Vector2 totalVelocity;
    private bool canJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        totalVelocity = new Vector2(inputHorizontal * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
        if (inputHorizontal != 0)
        {
            rb.velocity = totalVelocity;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                rb.AddForce(new Vector2(0f, 800f));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            canJump = true;
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
    }
}

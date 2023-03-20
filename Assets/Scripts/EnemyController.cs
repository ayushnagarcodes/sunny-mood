using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 pos;
    private float moveSpeed;
    private Bounds platform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //  This code causes stuttering but the one below this does not
        // pos = transform.position;
        // if (pos.x < (platform.max.x - platform.size.x + 1f) || pos.x > platform.max.x - 1f)
        // {
        //     moveSpeed *= -1;
        //     sr.flipX = !sr.flipX;
        // }
        pos = transform.position;
        if (pos.x < (platform.max.x - platform.size.x + .5f))
        {
            moveSpeed = 50;
            sr.flipX = true;
        }
        else if (pos.x > platform.max.x - .5f)
        {
            moveSpeed = -50;
            sr.flipX = false;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            platform = col.gameObject.GetComponent<Collider2D>().bounds;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 pos;
    private float moveSpeed = -200f;
    private Bounds platform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        pos = transform.position;
        if (pos.x < (platform.max.x - platform.size.x + .5f) || pos.x > platform.max.x - .5f)
        {
            moveSpeed *= -1;
            sr.flipX = !sr.flipX;
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

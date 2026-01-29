using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 8;
    public bool grounded;
    void Update()
    {
        grounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundMask);
        if (InputController.Jump)
        {
            Jump();
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        transform.position = transform.position + new Vector3(InputController.MoveVector.x * speed * Time.deltaTime, 0, 0);
    }
    void Jump()
    {
        if (grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpForce);
            grounded = false;
        }
    }
}

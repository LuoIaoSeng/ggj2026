using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float dashDistance = 5;
    public bool grounded;
    public bool enableInput = true;
    public int direction = 1;
    public bool dashed = false;
    void Update()
    {
        if (!enableInput)
            return;
        var moveVector = InputController.MoveVector;
        if (moveVector.x > 0)
        {
            direction = 1;
        }
        else if (moveVector.x < 0)
        {
            direction = -1;
        }
        grounded = Physics2D.OverlapCircle(transform.position, 0.01f, groundMask);
        if (InputController.Jump)
        {
            Jump();
        }
    }
    void FixedUpdate()
    {
        if (!enableInput)
            return;
        Move();
    }
    void Move()
    {
        if (InputController.Dash && !dashed)
        {
            Dash();
        }
        else
        {
            transform.position = transform.position + new Vector3(InputController.MoveVector.x * speed * Time.deltaTime, 0, 0);
        }
    }
    async void Dash()
    {
        if (dashed)
            return;
        transform.DOMoveX(transform.position.x + dashDistance * direction, 0.3f)
        .SetEase(Ease.Linear);
        dashed = true;
        await Task.Delay(1000);
        dashed = false;
    }
    void Jump()
    {
        if (grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }
    }
}

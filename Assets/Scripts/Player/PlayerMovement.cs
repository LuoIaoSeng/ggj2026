using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("移动速度")]
    public float walkSpeed = 5f;
    [Tooltip("跳跃力度")]
    public float jumpForce = 10f;
    [Tooltip("冲刺距离")]
    public float dashDistance = 10f;
    [Tooltip("冲刺CD")]
    public int dashCoolDown = 5;
    [Tooltip("下蹲移动速度百分比")]
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Detection Settings")]
    [Tooltip("地面检测物体")]
    public Transform groundCheck;
    [Tooltip("顶部检测物体")]
    public Transform ceilingCheck;
    public float checkRadius = 0.2f;
    [Tooltip("地面层")]
    public LayerMask groundLayer;

    [Header("State")]
    [SerializeField] public bool enableInput = true;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool IsCrouching { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsDashing {get;set;}
    private Rigidbody2D rb;
    public int direction = 1;
    public bool canDash = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (!enableInput)
        {
            return;
        }
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (!enableInput)
            return;
        Move();
    }

    private async void HandleInput()
    {
        if(InputController.Dash && canDash)
        {
            canDash = false;
            
            // 2. 冲刺开始：标记状态为 true
            IsDashing = true; 

            // 3. 修改这里：使用 OnComplete 在 0.5秒移动结束后把状态改回 false
            rb.DOMoveX(transform.position.x + direction * dashDistance, 0.5f)
              .OnComplete(() => IsDashing = false); 

            await Task.Delay(dashCoolDown * 1000);
            canDash = true;
        }
        if (InputController.Jump && IsGrounded && !IsCrouching)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        bool wantsToCrouch = Input.GetKey(KeyCode.S);

        if (wantsToCrouch)
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
            if (hit)
            {
                var twoWayStructure = hit.GetComponent<TwoWayStructure>();
                if (twoWayStructure)
                {
                    twoWayStructure.ToggleCollider(false);
                }
            }
        }
        if (!wantsToCrouch && IsCrouching)
        {
            Collider2D hit = Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer);
            if (hit != null)
            {
                wantsToCrouch = true;
            }
        }

        IsCrouching = wantsToCrouch;
    }

    private void Move()
    {

        var moveVector = InputController.MoveVector;

        if (moveVector.x > 0)
        {
            spriteRenderer.flipX = false;
            direction = 1;
        }
        else if (moveVector.x < 0)
        {
            spriteRenderer.flipX = true;
            direction = -1;
        }

        float currentSpeed = walkSpeed;
        if (IsCrouching) currentSpeed *= crouchSpeedMultiplier;

        rb.velocity = new Vector2(moveVector.x * currentSpeed, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ceilingCheck.position, checkRadius);
        }
    }
}

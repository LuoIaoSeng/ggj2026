using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float jumpForce = 10f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Detection Settings")]
    public Transform groundCheck;
    public Transform ceilingCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("State")]
    [SerializeField] public bool enableInput = true;

    // 【修改】改为列表，存储所有允许的按键
    public List<KeyCode> inputExceptionKeys = new List<KeyCode>();

    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool IsCrouching { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsDashing { get; set; }
    private Rigidbody2D rb;
    public int direction = 1;
    public bool canDash = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 【新增】辅助方法：判断某个键是否被允许
    public bool IsKeyAllowed(KeyCode key)
    {
        // 如果全局输入开启，允许一切
        if (enableInput) return true;
        // 如果列表为空，禁止一切
        if (inputExceptionKeys == null || inputExceptionKeys.Count == 0) return false;
        // 检查键是否在白名单里
        return inputExceptionKeys.Contains(key);
    }

    private void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 如果禁用输入且没有任何白名单按键，直接返回
        if (!enableInput && (inputExceptionKeys == null || inputExceptionKeys.Count == 0))
        {
            return;
        }
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (!enableInput && (inputExceptionKeys == null || inputExceptionKeys.Count == 0))
            return;
        Move();
    }

    private async void HandleInput()
    {
        // --- 冲刺逻辑 (LeftShift) ---
        // 使用新的 IsKeyAllowed 方法检查
        if (InputController.Dash && canDash && IsKeyAllowed(KeyCode.LeftShift))
        {
            canDash = false;
            IsDashing = true;
            rb.DOMoveX(transform.position.x + direction * 5, 0.5f)
              .OnComplete(() => IsDashing = false);
            await Task.Delay(1 * 1000);
            canDash = true;
        }

        // --- 跳跃逻辑 (W) ---
        if (InputController.Jump && IsGrounded && !IsCrouching && IsKeyAllowed(KeyCode.W))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- 下蹲逻辑 (S) ---
        bool wantsToCrouch = Input.GetKey(KeyCode.S);

        // 权限检查
        if (!IsKeyAllowed(KeyCode.S))
        {
            wantsToCrouch = false;
        }

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
            if (hit != null) wantsToCrouch = true;
        }
        IsCrouching = wantsToCrouch;
    }

    private void Move()
    {
        var moveVector = InputController.MoveVector;

        // 【关键修改】移动权限检查
        if (!enableInput)
        {
            // 检查是否有任意一个方向键在白名单里
            bool isMoveAllowed = IsKeyAllowed(KeyCode.A) || IsKeyAllowed(KeyCode.D) ||
                                 IsKeyAllowed(KeyCode.LeftArrow) || IsKeyAllowed(KeyCode.RightArrow);

            if (!isMoveAllowed)
            {
                moveVector = Vector2.zero;
            }
        }

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
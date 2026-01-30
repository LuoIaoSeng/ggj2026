using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("走路速度")]
    public float walkSpeed = 5f;
    [Tooltip("跳跃力度")]
    public float jumpForce = 10f;
    [Tooltip("下蹲时的速度倍率 (0-1)")]
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Detection Settings")]
    [Tooltip("角色脚底")]
    public Transform groundCheck;
    [Tooltip("角色头顶")]
    public Transform ceilingCheck;
    public float checkRadius = 0.2f;
    [Tooltip("地面/障碍物图层")]
    public LayerMask groundLayer;

    [Header("State")]
    [SerializeField] private bool isInputEnabled = true;

    // 对外公开状态，供 Animation 脚本读取
    public bool IsCrouching { get; private set; }
    public bool IsGrounded { get; private set; }

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // --- 修改点 2: 删除了所有 Collider 变量和获取代码 ---
    }

    private void Update()
    {
        if (!isInputEnabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        HandleInput();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        Move();
    }

    private void HandleInput()
    {
        // 跳跃逻辑
        if (Input.GetKeyDown(KeyCode.W) && IsGrounded && !IsCrouching)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- 修改点 3: 只计算状态，不修改碰撞体 ---
        bool wantsToCrouch = Input.GetKey(KeyCode.S);

        // 头顶检测逻辑：如果松开S但头顶有东西，保持蹲下
        //if (!wantsToCrouch && IsCrouching)
        //{
        //    if (Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer))
        //    {
        //        wantsToCrouch = true;
        //    }
        //}
        if (!wantsToCrouch && IsCrouching)
        {
            // 把原来的 Physics2D.OverlapCircle 改成这样来调试：
            Collider2D hit = Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer);
            if (hit != null)
            {
                Debug.Log("头顶检测到了障碍物：" + hit.name); // <--- 看控制台输出什么名字
                wantsToCrouch = true;
            }
        }

        // 更新状态
        IsCrouching = wantsToCrouch;

        // --- 修改点 4: 删除了 PerformCrouch() 调用 ---
    }

    private void Move()
    {
        if (!isInputEnabled) return;

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        float currentSpeed = walkSpeed;
        if (IsCrouching) currentSpeed *= crouchSpeedMultiplier;

        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void CheckSurroundings()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
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

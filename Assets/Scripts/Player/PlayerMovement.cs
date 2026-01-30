using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("��·�ٶ�")]
    public float walkSpeed = 5f;
    [Tooltip("��Ծ����")]
    public float jumpForce = 10f;
    [Tooltip("�¶�ʱ���ٶȱ��� (0-1)")]
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Detection Settings")]
    [Tooltip("��ɫ�ŵ�")]
    public Transform groundCheck;
    [Tooltip("��ɫͷ��")]
    public Transform ceilingCheck;
    public float checkRadius = 0.2f;
    [Tooltip("����/�ϰ���ͼ��")]
    public LayerMask groundLayer;

    [Header("State")]
    [SerializeField] public bool isInputEnabled = true;

    // ���⹫��״̬���� Animation �ű���ȡ
    public bool IsCrouching { get; private set; }
    public bool IsGrounded { get; private set; }

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // --- �޸ĵ� 2: ɾ�������� Collider �����ͻ�ȡ���� ---
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
        // ��Ծ�߼�
        if (Input.GetKeyDown(KeyCode.W) && IsGrounded && !IsCrouching)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- �޸ĵ� 3: ֻ����״̬�����޸���ײ�� ---
        bool wantsToCrouch = Input.GetKey(KeyCode.S);

        // ͷ������߼�������ɿ�S��ͷ���ж��������ֶ���
        //if (!wantsToCrouch && IsCrouching)
        //{
        //    if (Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer))
        //    {
        //        wantsToCrouch = true;
        //    }
        //}
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
            // ��ԭ���� Physics2D.OverlapCircle �ĳ����������ԣ�
            Collider2D hit = Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer);
            if (hit != null)
            {
                Debug.Log("ͷ����⵽���ϰ��" + hit.name); // <--- ������̨���ʲô����
                wantsToCrouch = true;
            }
        }

        // ����״̬
        IsCrouching = wantsToCrouch;

        // --- �޸ĵ� 4: ɾ���� PerformCrouch() ���� ---
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

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation State Names")]
    public string animIdle = "Stand";
    public string animWalk = "Walk";
    public string animCrouch = "Crouch"; // 确保你在 Animator 里勾选了这个状态的 Speed Multiplier
    public string animCrouchWalk = "CrouchWalk";
    public string animDash = "Dash";

    [Header("Settings")]
    public KeyCode dashKey = KeyCode.LeftShift;
    public float moveThreshold = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerMovement movement;

    private string currentState;
    private bool isRecoveringFromCrouch = false;
    private bool wasCrouchingLastFrame = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();

        // 初始化确保速度参数存在
        animator.speed = 1f; // 强制重置全局速度为正数，消除报错
    }

    void Update()
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > moveThreshold;
        bool isDashing = Input.GetKey(dashKey);

        // 获取逻辑层状态
        bool isCrouching = movement.IsCrouching;
        bool isGrounded = movement.IsGrounded;

        // 检测松开 S 键的瞬间
        bool isCrouchUp = wasCrouchingLastFrame && !isCrouching;
        wasCrouchingLastFrame = isCrouching;

        string targetState = currentState;
        float targetSpeed = 1f; // 默认为 1 (正向)
        float startNormalizedTime = float.NegativeInfinity;

        // --- 状态优先级逻辑 ---

        // 1. 冲刺或在空中 (打断蹲下)
        if (isDashing || !isGrounded)
        {
            isRecoveringFromCrouch = false;
            targetState = isDashing ? animDash : animIdle;
        }
        // 2. 正在蹲下
        else if (isCrouching)
        {
            isRecoveringFromCrouch = false;
            if (isMoving) targetState = animCrouchWalk;
            else targetState = animCrouch;
        }
        // 3. 松开 S 键的瞬间 (触发倒放)
        else if (isCrouchUp && !isMoving)
        {
            isRecoveringFromCrouch = true;
            targetState = animCrouch;
            targetSpeed = -1f; // 设置倒放速度
            startNormalizedTime = 1f; // 从尾部开始

            StopAllCoroutines();
            StartCoroutine(FinishStandUpAnimation());
        }
        // 4. 普通状态
        else
        {
            if (isMoving)
            {
                isRecoveringFromCrouch = false;
                targetState = animWalk;
            }
            else
            {
                if (isRecoveringFromCrouch)
                {
                    targetState = animCrouch;
                    targetSpeed = -1f; // 维持倒放速度
                }
                else
                {
                    targetState = animIdle;
                }
            }
        }

        ChangeAnimationState(targetState, targetSpeed, startNormalizedTime);
    }

    void ChangeAnimationState(string newState, float speed = 1f, float startNormalizedTime = float.NegativeInfinity)
    {
        // --- 核心修改点 ---
        // 我们不再修改 animator.speed，而是修改我们设置的 "PlaySpeed" 参数
        animator.SetFloat("PlaySpeed", speed);
        // 确保全局速度始终为 1，防止报错
        animator.speed = 1f;

        // 只有状态改变，或者重新需要从头(或尾)播放时才调用 Play
        bool isStateChanged = currentState != newState;
        bool isForcedRestart = !float.IsNegativeInfinity(startNormalizedTime);

        if (isStateChanged || isForcedRestart)
        {
            currentState = newState;
            if (isForcedRestart)
            {
                animator.Play(newState, 0, startNormalizedTime);
            }
            else
            {
                animator.Play(newState);
            }
        }
    }

    IEnumerator FinishStandUpAnimation()
    {
        yield return null;
        float length = 0.5f;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.IsName(animCrouch))
        {
            length = info.length;
        }

        // 等待倒放结束
        yield return new WaitForSeconds(length);

        isRecoveringFromCrouch = false;
        ChangeAnimationState(animIdle);
    }
}
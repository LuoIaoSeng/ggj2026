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
    public string animCrouch = "Crouch"; // 确锟斤拷锟斤拷锟斤拷 Animator 锟斤勾选锟斤拷锟斤拷锟阶刺拷锟?Speed Multiplier
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

        // 锟斤拷始锟斤拷确锟斤拷锟劫度诧拷锟斤拷锟斤拷锟斤拷
        animator.speed = 1f; // 强锟斤拷锟斤拷锟斤拷全锟斤拷锟劫讹拷为锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
    }

    void Update()
    {
        if (!movement.enableInput)
            return;
        bool isMoving = Mathf.Abs(rb.velocity.x) > moveThreshold;
        // bool isDashing = InputController.Dash;  <-- 删除这行
        bool isDashing = movement.IsDashing;    // <-- 改成这行
        // 锟斤拷取锟竭硷拷锟斤拷状态
        bool isCrouching = movement.IsCrouching;
        bool isGrounded = movement.IsGrounded;

        // 锟斤拷锟斤拷煽锟?S 锟斤拷锟斤拷瞬锟斤拷
        bool isCrouchUp = wasCrouchingLastFrame && !isCrouching;
        wasCrouchingLastFrame = isCrouching;

        string targetState = currentState;
        float targetSpeed = 1f; // 默锟斤拷为 1 (锟斤拷锟斤拷)
        float startNormalizedTime = float.NegativeInfinity;

        // --- 状态锟斤拷锟饺硷拷锟竭硷拷 ---

        // 1. 锟斤拷袒锟斤拷诳锟斤拷锟?(锟斤拷隙锟斤拷锟?
        if (isDashing || !isGrounded)
        {
            isRecoveringFromCrouch = false;
            targetState = isDashing ? animDash : animIdle;
        }
        // 2. 锟斤拷锟节讹拷锟斤拷
        else if (isCrouching)
        {
            isRecoveringFromCrouch = false;

            if (isMoving)
            {
                targetState = animCrouchWalk;
            }
            else
            {
                targetState = animCrouch;

                // 【关键修改】
                // 如果上一帧是蹲走 (CrouchWalk)，说明我是停下来了
                // 这时不需要重播下蹲动作，而是直接跳到下蹲的最后一帧 (1.0f)
                if (currentState == animCrouchWalk)
                {
                    startNormalizedTime = 1f;
                }
            }
        }
        // 3. 锟缴匡拷 S 锟斤拷锟斤拷瞬锟斤拷 (锟斤拷锟斤拷锟斤拷锟斤拷)
        else if (isCrouchUp && !isMoving)
        {
            isRecoveringFromCrouch = true;
            targetState = animCrouch;
            targetSpeed = -1f; // 锟斤拷锟矫碉拷锟斤拷锟劫讹拷
            startNormalizedTime = 1f; // 锟斤拷尾锟斤拷锟斤拷始

            StopAllCoroutines();
            StartCoroutine(FinishStandUpAnimation());
        }
        // 4. 锟斤拷通状态
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
                    targetSpeed = -1f; // 维锟街碉拷锟斤拷锟劫讹拷
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
        // --- 锟斤拷锟斤拷锟睫改碉拷 ---
        // 锟斤拷锟角诧拷锟斤拷锟睫革拷 animator.speed锟斤拷锟斤拷锟斤拷锟睫革拷锟斤拷锟斤拷锟斤拷锟矫碉拷 "PlaySpeed" 锟斤拷锟斤拷
        animator.SetFloat("PlaySpeed", speed);
        // 确锟斤拷全锟斤拷锟劫讹拷始锟斤拷为 1锟斤拷锟斤拷止锟斤拷锟斤拷
        animator.speed = 1f;

        // 只锟斤拷状态锟侥变，锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷要锟斤拷头(锟斤拷尾)锟斤拷锟斤拷时锟脚碉拷锟斤拷 Play
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

        //Debug.Log("尝试切换到动画: " + newState);
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

        // 锟饺达拷锟斤拷锟脚斤拷锟斤拷
        yield return new WaitForSeconds(length);

        isRecoveringFromCrouch = false;
        ChangeAnimationState(animIdle);
    }
}
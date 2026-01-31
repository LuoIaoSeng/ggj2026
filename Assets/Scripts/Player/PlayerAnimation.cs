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
    public string animCrouch = "Crouch";
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

        animator.speed = 1f;
    }

    void Update()
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > moveThreshold;
<<<<<<< Updated upstream
        bool isDashing = Input.GetKey(dashKey);
=======
        bool isDashing = movement.IsDashing;
>>>>>>> Stashed changes

        bool isCrouching = movement.IsCrouching;
        bool isGrounded = movement.IsGrounded;

        bool isCrouchUp = wasCrouchingLastFrame && !isCrouching;
        wasCrouchingLastFrame = isCrouching;

        string targetState = currentState;
        float targetSpeed = 1f;
        float startNormalizedTime = float.NegativeInfinity;


        if (isDashing || !isGrounded)
        {
            isRecoveringFromCrouch = false;
            targetState = isDashing ? animDash : animIdle;
        }
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
        else if (isCrouchUp && !isMoving)
        {
            isRecoveringFromCrouch = true;
            targetState = animCrouch;
            targetSpeed = -1f;
            startNormalizedTime = 1f;

            StopAllCoroutines();
            StartCoroutine(FinishStandUpAnimation());
        }
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
                    targetSpeed = -1f;
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
        animator.SetFloat("PlaySpeed", speed);
        animator.speed = 1f;

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

        yield return new WaitForSeconds(length);

        isRecoveringFromCrouch = false;
        ChangeAnimationState(animIdle);
    }
}
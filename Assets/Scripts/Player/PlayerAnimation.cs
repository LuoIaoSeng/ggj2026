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
    public string animCrouch = "Crouch"; // ȷ������ Animator �ﹴѡ�����״̬�� Speed Multiplier
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

        // ��ʼ��ȷ���ٶȲ�������
        animator.speed = 1f; // ǿ������ȫ���ٶ�Ϊ��������������
    }

    void Update()
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > moveThreshold;
        bool isDashing = Input.GetKey(dashKey);

        // ��ȡ�߼���״̬
        bool isCrouching = movement.IsCrouching;
        bool isGrounded = movement.IsGrounded;

        // ����ɿ� S ����˲��
        bool isCrouchUp = wasCrouchingLastFrame && !isCrouching;
        wasCrouchingLastFrame = isCrouching;

        string targetState = currentState;
        float targetSpeed = 1f; // Ĭ��Ϊ 1 (����)
        float startNormalizedTime = float.NegativeInfinity;

        // --- ״̬���ȼ��߼� ---

        // 1. ��̻��ڿ��� (��϶���)
        if (isDashing || !isGrounded)
        {
            isRecoveringFromCrouch = false;
            targetState = isDashing ? animDash : animIdle;
        }
        // 2. ���ڶ���
        else if (isCrouching)
        {
            isRecoveringFromCrouch = false;
            if (isMoving) targetState = animCrouchWalk;
            else targetState = animCrouch;
        }
        // 3. �ɿ� S ����˲�� (��������)
        else if (isCrouchUp && !isMoving)
        {
            isRecoveringFromCrouch = true;
            targetState = animCrouch;
            targetSpeed = -1f; // ���õ����ٶ�
            startNormalizedTime = 1f; // ��β����ʼ

            StopAllCoroutines();
            StartCoroutine(FinishStandUpAnimation());
        }
        // 4. ��ͨ״̬
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
                    targetSpeed = -1f; // ά�ֵ����ٶ�
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
        // --- �����޸ĵ� ---
        // ���ǲ����޸� animator.speed�������޸��������õ� "PlaySpeed" ����
        animator.SetFloat("PlaySpeed", speed);
        // ȷ��ȫ���ٶ�ʼ��Ϊ 1����ֹ����
        animator.speed = 1f;

        // ֻ��״̬�ı䣬����������Ҫ��ͷ(��β)����ʱ�ŵ��� Play
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

        // �ȴ����Ž���
        yield return new WaitForSeconds(length);

        isRecoveringFromCrouch = false;
        ChangeAnimationState(animIdle);
    }
}
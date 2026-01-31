using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AnimatedLift : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Animator liftAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip denySound;
    [SerializeField] private Transform playerStandPos; // 电梯内部站位点

    private Lift liftScript;          // 引用原本的电梯脚本
    private ItemRequirement itemRequirement;

    private string animParamIsOpen = "IsOpen";
    private GameObject playerObj;
    private bool isTransitioning = false; // 是否正在进行整套流程

    void Start()
    {
        liftScript = GetComponent<Lift>();
        itemRequirement = GetComponent<ItemRequirement>();

        // 【关键】强制开启 Lift 的手动模式，防止它自己乱动
        if (liftScript != null)
        {
            liftScript.manualControl = true;
        }
    }

    void Update()
    {
        // 获取 Lift 脚本里的 player 引用，因为它和 Trigger 绑定在一起
        // 如果 Lift 检测到了玩家，我们也就认为玩家在范围内
        if (liftScript != null)
        {
            playerObj = liftScript.player != null ? liftScript.player.gameObject : null;
        }

        if (playerObj == null || isTransitioning) return;

        if (InputController.Interact)
        {
            TryUseLift();
        }
    }

    // --- 1. 玩家靠近自动开门 (利用 Animator) ---
    // 由于 Lift.cs 已经有了 Trigger，且在同一物体上，
    // 我们直接监听 Lift 里的 player 变量变化来控制动画稍显麻烦
    // 还是保留 OnTriggerEnter/Exit 来控制“自动开关门”比较直观
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            liftAnimator.SetBool(animParamIsOpen, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            liftAnimator.SetBool(animParamIsOpen, false);
        }
    }

    // --- 2. 交互逻辑 ---
    private void TryUseLift()
    {
        // 检查物品
        string reqId = itemRequirement != null ? itemRequirement.requiredItemId : "";
        var inventory = playerObj.GetComponent<PlayerInventory>();

        if (string.IsNullOrEmpty(reqId) || (inventory != null && inventory.HasItem(reqId)))
        {
            StartCoroutine(ProcessLiftSequence());
        }
        else
        {
            if (audioSource && denySound) audioSource.PlayOneShot(denySound);
        }
    }

    // --- 3. 核心流程：进门 -> 关门 -> 移动 -> 到达 -> 开门 -> 出门 ---
    private IEnumerator ProcessLiftSequence()
    {
        isTransitioning = true;

        // A. 冻结玩家
        var movement = playerObj.GetComponent<PlayerMovement>();
        var rb = playerObj.GetComponent<Rigidbody2D>();
        var sprite = playerObj.GetComponentInChildren<SpriteRenderer>();

        if (movement) movement.enableInput = false;
        if (rb) rb.velocity = Vector2.zero;

        // B. 演出：玩家走到中间并淡出 (模拟走进电梯深处)
        if (playerStandPos)
        {
            playerObj.transform.DOMoveX(playerStandPos.position.x, 0.5f);
        }
        if (sprite)
        {
            sprite.DOFade(0, 0.5f); // 变透明
        }
        yield return new WaitForSeconds(0.5f);

        // C. 关门动画
        liftAnimator.SetBool(animParamIsOpen, false);
        yield return new WaitForSeconds(1.0f); // 等待关门动画播完

        // D. 呼叫 Lift.cs 开始移动
        if (liftScript != null)
        {
            bool moveFinished = false;

            // 调用我们刚才改写的方法，并传入回调
            liftScript.StartLiftRoutine(() => {
                moveFinished = true;
            });

            // 等待移动结束 (Lift.cs 里的 DOMoveY 完成)
            yield return new WaitUntil(() => moveFinished);
        }

        // --- E. 到达新楼层后的逻辑 ---

        // 1. 开门
        liftAnimator.SetBool(animParamIsOpen, true);
        yield return new WaitForSeconds(0.5f); // 等门开了一会儿

        // 2. 玩家淡入显示
        if (sprite)
        {
            sprite.DOFade(1, 0.5f);
        }

        // 3. 恢复控制
        if (movement) movement.enableInput = true;

        isTransitioning = false;
    }
}
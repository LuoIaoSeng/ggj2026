using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class SimpleEscalator : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("终点相对于当前物体中心的位置偏移量")]
    public Vector2 targetOffset = new Vector2(3, 3);
    [Tooltip("移动所需时间")]
    public float duration = 1.5f;
    [Tooltip("移动曲线")]
    public Ease moveEase = Ease.Linear;

    private bool isMoving = false;
    private bool isPlayerInRange = false; // 标记：玩家是否在区域内
    private Transform currentPlayer;      // 记录玩家是谁

    void Update()
    {
        // 改在 Update 中检测按键，绝对不会丢帧
        if (isPlayerInRange && !isMoving && InputController.Interact)
        {
            if (currentPlayer != null)
            {
                StartCoroutine(MoveRoutine(currentPlayer));
            }
        }
    }

    // 只负责标记状态：进来了
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayer = collision.transform;
        }
    }

    // 只负责标记状态：离开了
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            currentPlayer = null;
        }
    }

    IEnumerator MoveRoutine(Transform playerTransform)
    {
        isMoving = true;

        // 获取组件引用
        var playerMove = playerTransform.GetComponent<PlayerMovement>();
        var rb = playerTransform.GetComponent<Rigidbody2D>();

        // 1. 冻结控制
        if (playerMove) playerMove.enableInput = false;
        if (rb) rb.velocity = Vector2.zero;

        // 2. 计算终点 (当前物体中心 + 偏移)
        Vector3 targetPos = transform.position + (Vector3)targetOffset;

        // 3. 移动
        yield return playerTransform.DOMove(targetPos, duration).SetEase(moveEase).WaitForCompletion();

        // 4. 恢复控制
        if (playerMove) playerMove.enableInput = true;
        isMoving = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 endPos = transform.position + (Vector3)targetOffset;
        Gizmos.DrawLine(transform.position, endPos);
        Gizmos.DrawWireSphere(endPos, 0.2f);
    }
}
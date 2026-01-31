using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class Lift : MonoBehaviour
{
    [SerializeField] private List<Transform> floorTransforms;
    [SerializeField] private int floor;
    [SerializeField] private string dir;
    [SerializeField] private int initFloor;
    [SerializeField] private Transform lift;
    //private Collider2D player;
    public Collider2D player { get; private set; }
    private bool isMoving = false;

    [Header("设置")]
    [Tooltip("如果勾选，电梯不会自动响应按键，而是等待 AnimatedLift 调用")]
    public bool manualControl = false;

    private ItemRequirement itemRequirement;

    void Start()
    {
        floor = initFloor;
        itemRequirement = GetComponent<ItemRequirement>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
        }
    }
    void Update()
    {
        if (manualControl) return;
        { 
            StartLiftRoutine(null);
        }
    }

    public void StartLiftRoutine(Action onMoveComplete)
    {
        if (player == null || isMoving) return;

        // 物品检查逻辑 (如果 AnimatedLift 已经查过了，这里其实可以跳过，但保留也无妨)
        if (!manualControl && itemRequirement != null && !itemRequirement.CheckAccess(player.gameObject))
        {
            // 这里可以加一些反馈，比如播放“嘟嘟”的报错音效
            return;
        }

        var playerMovement = player.GetComponent<PlayerMovement>();
        var rb = playerMovement.GetComponent<Rigidbody2D>();

        rb.velocity = Vector2.zero;
        playerMovement.enableInput = false;
        isMoving = true;

        // 计算目标楼层
        var temp = floor + (dir == "up" ? 1 : -1);
        if (temp == floorTransforms.Count)
        {
            dir = "down";
        }
        else if (temp == -1)
        {
            dir = "up";
        }
        floor = floor + (dir == "up" ? 1 : -1);

        // 开始移动
        playerMovement.transform.DOMoveX(floorTransforms[floor].position.x, 0.5f)
        .OnComplete(() =>
        {
            // 纵向移动 (电梯 + 玩家)
            Sequence seq = DOTween.Sequence();
            seq.Join(playerMovement.transform.DOMoveY(floorTransforms[floor].position.y, 2));
            seq.Join(lift.transform.DOMoveY(floorTransforms[floor].position.y, 2));

            seq.OnComplete(() =>
            {
                // 移动结束
                // 如果不是手动控制，这里就直接恢复输入；
                // 如果是手动控制，我们把恢复输入的权力交给 AnimatedLift（因为它还要演开门动画）
                if (!manualControl)
                {
                    playerMovement.enableInput = true;
                }

                isMoving = false;

                // 执行回调（告诉 AnimatedLift：到了！）
                onMoveComplete?.Invoke();
            });
        });
    }
}

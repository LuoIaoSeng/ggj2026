using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("平滑参数（值越小越平滑）")]
    [Range(0, 1)]
    public float smoothSpeed = 0.125f;

    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("开场运镜设置")]
    [Tooltip("是否启用开场运镜")]
    public bool enableIntro = true;

    [Tooltip("开始移动前的等待时间（秒）")] // 【新增】延迟时间设置
    public float startDelay = 1.0f;

    [Tooltip("运镜时长（秒）")]
    public float introDuration = 2.0f;

    [Tooltip("运镜曲线")]
    public Ease introEase = Ease.InOutSine;

    // 内部状态变量
    private bool _isInIntro = false;
    private Vector3 _introStartPos;
    private float _introProgress = 0f;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (enableIntro)
        {
            StartIntro();
        }
        else
        {
            if (target != null) transform.position = target.position + offset;
        }
    }

    void StartIntro()
    {
        _isInIntro = true;
        _introStartPos = transform.position;
        _introProgress = 0f;

        // 1. 即使是等待期间，也立刻禁用玩家输入，防止玩家在镜头没动时乱跑
        if (target != null)
        {
            var playerMovement = target.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.enableInput = false;

            var rb = target.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        // 2. 使用 DOTween，并添加 SetDelay
        // SetDelay(startDelay) 会让数值在 startDelay 秒内保持为 0，然后才开始变化
        DOTween.To(() => _introProgress, x => _introProgress = x, 1f, introDuration)
            .SetEase(introEase)
            .SetDelay(startDelay) // 【关键修改】添加延迟
            .OnComplete(OnIntroComplete);
    }

    void OnIntroComplete()
    {
        _isInIntro = false;

        // 3. 动画结束（延迟+运镜全部完成后），才恢复玩家控制
        if (target != null)
        {
            var playerMovement = target.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enableInput = true;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (_isInIntro)
        {
            // 在延迟期间 _introProgress 保持为 0，所以相机静止在 _introStartPos
            // 延迟结束后，_introProgress 从 0 变到 1，相机平滑飞向目标
            transform.position = Vector3.Lerp(_introStartPos, desiredPosition, _introProgress);
        }
        else
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
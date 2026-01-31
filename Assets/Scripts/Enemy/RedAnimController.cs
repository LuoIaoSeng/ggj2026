using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedAnimController : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private Enemy _EnemyScript;

    private float _stopSignalTimer = 0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // 改用 LateUpdate！确保在敌人所有的逻辑（包括重置和设置IsTurn）都跑完之后，我们再来看结果
    void LateUpdate()
    {
        if (_EnemyScript != null)
        {
            // 1. 捕捉信号
            if (_EnemyScript.IsTurn)
            {
                // 如果敌人说要停，我们给个0.1秒的“宽限期”
                // 这样即使下一帧 IsTurn 变回 false，我们的动画参数还能坚持一小会儿
                _stopSignalTimer = 0.1f;
            }

            // 2. 倒计时
            if (_stopSignalTimer > 0)
            {
                _stopSignalTimer -= Time.deltaTime;
            }

            // 3. 只要还在宽限期内，就告诉动画机：现在是停止状态！
            bool shouldStop = _stopSignalTimer > 0;
            _animator.SetBool("isStopping", shouldStop);
        }
    }
}

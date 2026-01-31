using UnityEngine;

public class EnemyB : Enemy
{
    [SerializeField] protected Animator animator;
    [SerializeField] public float speed = 2f;

    // 动画状态名称
    private const string ANIM_WALK_RIGHT = "WalkRight";
    private const string ANIM_WALK_LEFT = "WalkLeft";

    protected override void Start()
    {
        base.Start();
        mask = "blue";
        direction = 1;

        // 确保没有旋转干扰
        enemyObject.rotation = Quaternion.identity;

        // 初始动画
        PlayMoveAnimation();
    }

    void Update()
    {
        // 1. 纯粹的移动逻辑
        enemyObject.position += Vector3.right * direction * Time.deltaTime * speed;

        // 2. 纯粹的转向判断
        if (enemyObject.position.x > EndPoint2.position.x && direction == 1)
        {
            TurnAround(-1);
        }
        else if (enemyObject.position.x < EndPoint1.position.x && direction == -1)
        {
            TurnAround(1);
        }
    }

    private void TurnAround(int newDirection)
    {
        direction = newDirection;
        // 3. 状态改变时，直接切换动画，简单明了
        PlayMoveAnimation();
    }

    private void PlayMoveAnimation()
    {
        if (animator == null) return;

        if (direction == 1)
            animator.Play(ANIM_WALK_RIGHT);
        else
        {
            animator.Play(ANIM_WALK_LEFT);
        }
    }
}
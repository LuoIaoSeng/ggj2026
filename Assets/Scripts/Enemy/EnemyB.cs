using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyB : Enemy
{
    [SerializeField] protected Animator animator;
    [SerializeField] public float speed = 2f;

    // ����״̬����
    private const string ANIM_WALK_RIGHT = "WalkRight";
    private const string ANIM_WALK_LEFT = "WalkLeft";

    [SerializeField] private List<SpriteRenderer> coverEnemySprite;

    protected override void Start()
    {
        base.Start();
        mask = "blue";
        direction = 1;

        // ȷ��û����ת����
        enemyObject.rotation = Quaternion.identity;
        enemyObject.DOMoveX(EndPoint1.position.x, 0);

        // ��ʼ����
        PlayMoveAnimation();
    }

    void Update()
    {
        // 1. ������ƶ��߼�
        enemyObject.position += Vector3.right * direction * Time.deltaTime * speed;

        if (isIgnore(playerAbility))
        {
            foreach (var sprite in coverEnemySprite)
            {
                sprite.DOFade(1, 0);
            }
        }
        else
        {
            foreach (var sprite in coverEnemySprite)
            {
                sprite.DOFade(0, 0);
            }
        }

        // 2. �����ת���ж�
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
        // 3. ״̬�ı�ʱ��ֱ���л�������������
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
using DG.Tweening;
using UnityEngine;
public class EnemyR : Enemy
{
    protected override void Start()
    {
        base.Start();
        mask = "red";

        sprite
            .DOMove(EndPoint2.position, 2)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(() =>
            {
                sprite.DORotate(new Vector3(0, sprite.rotation.eulerAngles.y + 180, 0), 0);
                direction *= -1;
                IsTurn = true;
            });
    }

    protected override void Update()
    {
        base.Update();
        hit = Physics2D.Raycast(raycastStartPoint.position, Vector3.right * direction, 10f);
    }
}
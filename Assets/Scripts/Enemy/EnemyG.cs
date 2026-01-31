using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyG : Enemy
{
    protected override void Start()
    {
        base.Start();
        mask = "green"; 
    }
}
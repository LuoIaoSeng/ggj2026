using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private ItemRequirement itemRequirement;
    [SerializeField] private EdgeCollider2D edge;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && itemRequirement.CheckAccess(collision.gameObject))
        {
            edge.enabled = false;
            spriteRenderer.DOFade(0.2f, 1);
        }
    }
}

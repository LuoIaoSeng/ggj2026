using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CollectibleItem : MonoBehaviour
{ 
    [Header("Settings")]
    [Tooltip("该物品的唯一ID，例如 'LiftCard_Level1'")]
    public string uniqueId;

    void Start()
    {
        transform.DOMove(Vector3.up + transform.position, 2).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 参考你 Lift.cs 中的写法，通过 Tag 判断玩家
        if (collision.CompareTag("Player"))
        {
            // 获取玩家身上的背包组件
            var inventory = collision.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                // 1. 将ID加入列表
                inventory.AddItem(uniqueId);

                // 2. 收集物消失
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("玩家物体上缺少 PlayerInventory 组件！");
            }
        }
    }
}
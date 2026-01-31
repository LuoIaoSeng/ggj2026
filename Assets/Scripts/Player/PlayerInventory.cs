using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Collected Items")]
    // 存储所有已收集物品的唯一ID
    [SerializeField]
    private List<string> collectedItemIds = new List<string>();

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="id">物品的唯一ID</param>
    public void AddItem(string id)
    {
        if (!collectedItemIds.Contains(id))
        {
            collectedItemIds.Add(id);
            Debug.Log($"[PlayerInventory] 成功收集物品: {id}");
        }
    }

    /// <summary>
    /// 检查是否拥有某物品
    /// </summary>
    /// <param name="id">物品ID</param>
    /// <returns></returns>
    public bool HasItem(string id)
    {
        return collectedItemIds.Contains(id);
    }

    // 如果你将来需要整合进 GameData 存档系统，可以添加如下方法：
    // public List<string> GetCollectedItems() => collectedItemIds;
    // public void LoadCollectedItems(List<string> items) => collectedItemIds = items;
}
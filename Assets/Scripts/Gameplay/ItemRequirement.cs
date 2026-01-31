using UnityEngine;

public class ItemRequirement : MonoBehaviour
{
    [Header("Requirement Settings")]
    [Tooltip("需要匹配的收集物ID，留空则代表无需物品即可使用")]
    public string requiredItemId;

    /// <summary>
    /// 检查玩家是否满足条件
    /// </summary>
    /// <param name="player">玩家物体</param>
    /// <returns>如果满足条件（或没有设置ID）返回 true，否则返回 false</returns>
    public bool CheckAccess(GameObject player)
    {
        // 1. 如果ID为空，说明不需要锁，直接通过
        if (string.IsNullOrEmpty(requiredItemId))
        {
            return true;
        }

        // 2. 获取玩家的背包
        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning($"[ItemRequirement] 物体 {player.name} 身上没有 PlayerInventory 组件");
            return false;
        }

        // 3. 检查背包里是否有该ID
        if (inventory.HasItem(requiredItemId))
        {
            return true;
        }
        else
        {
            Debug.Log($"[ItemRequirement] 拒绝访问：玩家缺少收集物： {requiredItemId}");
            return false;
        }
    }
}
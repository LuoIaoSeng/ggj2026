using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        ShowPrompt,
        StartDialogue
    }

    public TriggerType type;
    public bool isOneTimeOnly = true;
    private bool hasTriggered = false;

    [Header("提示图标设置 (ShowPrompt)")]
    // 改成数组，支持动画！
    public Sprite[] promptIcons;
    [Tooltip("每帧播放间隔时间，越小越快")]
    public float animSpeed = 0.5f;

    [Tooltip("图标相对于触发器中心的位置偏移")]
    public Vector3 iconOffset = new Vector3(0, 1.5f, 0);

    [Header("对话设置 (StartDialogue)")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && isOneTimeOnly) return;

        // 过滤：只有挂着 PlayerMovement 的物体（主角本体）才能触发
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (type == TriggerType.ShowPrompt)
            {
                // 传入 this 作为权限凭证
                TutorialManager.Instance.ShowPrompt(promptIcons, animSpeed, transform.position + iconOffset, this);
            }
            else if (type == TriggerType.StartDialogue)
            {
                TutorialManager.Instance.StartDialogue(dialogueLines);
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 过滤：同样只有主角本体离开才算离开
        // 并且如果 other 是 trigger 碰撞体（比如攻击范围检测框），也忽略
        if (other.GetComponent<PlayerMovement>() != null && !other.isTrigger)
        {
            if (type == TriggerType.ShowPrompt)
            {
                // 申请隐藏，传入 this 进行核对
                TutorialManager.Instance.HidePrompt(this);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (type == TriggerType.ShowPrompt)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + iconOffset, 0.2f);
        }
    }
}
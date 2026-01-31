using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        ShowPrompt,   // 显示按键提示（如：按W跳跃）
        StartDialogue // 弹出对话框（强制剧情）
    }

    public TriggerType type;
    public bool isOneTimeOnly = true; // 是否是一次性的（通常对话是一次性的，提示可能不是）
    private bool hasTriggered = false;

    [Header("如果是提示图标 (ShowPrompt)")]
    public Sprite promptIcon; // 这里拖入你的 W/A/S/D 或 鼠标 图片

    [Header("如果是对话框 (StartDialogue)")]
    [TextArea(3, 10)]
    public string[] dialogueLines; // 在Inspector里直接写对话内容

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && isOneTimeOnly) return;

        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.ShowPrompt)
            {
                TutorialManager.Instance.ShowPrompt(promptIcon);
            }
            else if (type == TriggerType.StartDialogue)
            {
                TutorialManager.Instance.StartDialogue(dialogueLines);
                hasTriggered = true; // 对话通常只触发一次
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 当玩家离开触发区域时，如果是“提示图标”类型，我们应该把图标关掉
        // 这样可以实现“走到坑前提示跳跃，跳过去后提示消失”的效果
        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.ShowPrompt)
            {
                TutorialManager.Instance.HidePrompt();
            }
        }
    }
}
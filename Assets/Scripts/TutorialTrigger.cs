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

    [Header("如果是提示图标 (ShowPrompt)")]
    public Sprite promptIcon;

    // --- 新增：位置偏移 ---
    [Tooltip("图标相对于触发器中心的位置偏移，例如 (0, 2, 0) 表示在上方2个单位")]
    public Vector3 iconOffset = new Vector3(0, 1.5f, 0);

    [Header("如果是对话框 (StartDialogue)")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && isOneTimeOnly) return;

        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.ShowPrompt)
            {
                // --- 修改：传入当前触发器的位置 + 偏移量 ---
                TutorialManager.Instance.ShowPrompt(promptIcon, transform.position + iconOffset);
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
        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.ShowPrompt)
            {
                TutorialManager.Instance.HidePrompt();
            }
        }
    }

    // 可选：在编辑器里画个小圈圈，方便看偏移量在哪
    void OnDrawGizmosSelected()
    {
        if (type == TriggerType.ShowPrompt)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + iconOffset, 0.2f);
        }
    }
}
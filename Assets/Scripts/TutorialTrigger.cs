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
    public Sprite[] promptIcons;
    public float animSpeed = 0.5f;
    public Vector3 iconOffset = new Vector3(0, 1.5f, 0);

    [Header("对话设置 (StartDialogue)")]
    // 这里使用了 TutorialManager 里定义的结构体
    public DialogueData[] dialogueLines;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && isOneTimeOnly) return;

        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (type == TriggerType.ShowPrompt)
            {
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
        if (other.GetComponent<PlayerMovement>() != null && !other.isTrigger)
        {
            if (type == TriggerType.ShowPrompt)
            {
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
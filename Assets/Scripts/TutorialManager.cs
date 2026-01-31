using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("角色引用")]
    public PlayerMovement playerMovement;

    [Header("UI引用：提示图标 (Prompt)")]
    public GameObject promptPanel; // 包含图标的父物体
    public Image promptImage;      // 显示按键图标的Image组件

    [Header("UI引用：对话框 (Dialogue)")]
    public GameObject dialoguePanel; // 包含背景和文字的父物体
    public TextMeshProUGUI dialogueText;        // 显示文字的组件 (如果是TMP需更换类型)

    // 内部状态
    private bool isInDialogue = false;
    private Queue<string> currentDialogueLines = new Queue<string>();

    void Awake()
    {
        // 单例模式，方便触发器调用
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 初始化UI状态
        if (promptPanel) promptPanel.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // 如果没有手动拖拽玩家，自动查找
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        // --- 需求2：对话模式下的逻辑 ---
        if (isInDialogue)
        {
            // 在对话模式下，只响应空格键（Space）进入下一句
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
        }
    }

    // ================== 需求1：动态提示图标 ==================

    /// <summary>
    /// 显示操作提示（不打断玩家）
    /// </summary>
    /// <param name="icon">要显示的按键图标</param>
    public void ShowPrompt(Sprite icon)
    {
        if (promptPanel != null && promptImage != null)
        {
            promptImage.sprite = icon;
            promptImage.SetNativeSize(); // 保持图片原比例
            promptPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏操作提示
    /// </summary>
    public void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    // ================== 需求2：剧情对话 ==================

    /// <summary>
    /// 开始一段对话（禁用玩家输入）
    /// </summary>
    /// <param name="lines">对话内容的数组</param>
    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        isInDialogue = true;

        // 1. 禁用玩家操作
        if (playerMovement != null)
        {
            playerMovement.enableInput = false;

            // 关键：将玩家速度归零，防止带着惯性滑行
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            // 如果有动画组件，这里最好也设为 Idle，例如：
            // playerMovement.GetComponent<Animator>().Play("Stand");
        }

        // 2. 初始化对话队列
        currentDialogueLines.Clear();
        foreach (string line in lines)
        {
            currentDialogueLines.Enqueue(line);
        }

        // 3. 打开UI并显示第一句
        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (currentDialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = currentDialogueLines.Dequeue();
        dialogueText.text = line;
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);

        // 恢复玩家操作
        if (playerMovement != null)
        {
            playerMovement.enableInput = true;
        }
    }
}
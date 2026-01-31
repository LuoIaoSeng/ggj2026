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

    // 用来记录图标应该钉在世界里的哪个位置
    private Vector3 targetWorldPos;
    // 开关：是否需要更新图标位置
    private bool isTrackingPos = false;
    // 缓存摄像机，提升性能
    private Camera mainCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (promptPanel) promptPanel.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();

        // 获取主摄像机
        mainCamera = Camera.main;
        if (mainCamera == null) mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        // --- 1. 图标位置跟随逻辑 (新增) ---
        if (isTrackingPos && promptPanel.activeSelf && mainCamera != null)
        {
            // 核心魔法：将 3D/2D 世界坐标转换为屏幕上的 2D 像素坐标
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);
            promptPanel.transform.position = screenPos;
        }

        // --- 2. 对话逻辑 ---
        if (isInDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
        }
    }

    // --- 修改了这里：增加了 worldPos 参数 ---
    public void ShowPrompt(Sprite icon, Vector3 worldPos)
    {
        if (promptPanel != null && promptImage != null)
        {
            promptImage.sprite = icon;
            promptImage.SetNativeSize();

            // 记录目标位置，并开启跟随
            targetWorldPos = worldPos;
            isTrackingPos = true;

            promptPanel.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
        {
            isTrackingPos = false; // 停止跟随计算
            promptPanel.SetActive(false);
        }
    }

    // ... 下面是原本的对话代码，保持不变 ...
    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        isInDialogue = true;

        if (playerMovement != null)
        {
            playerMovement.enableInput = false;
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        currentDialogueLines.Clear();
        foreach (string line in lines)
        {
            currentDialogueLines.Enqueue(line);
        }

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
        if (playerMovement != null)
        {
            playerMovement.enableInput = true;
        }
    }
}
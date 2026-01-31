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

    [Header("UI引用")]
    public GameObject promptPanel;
    public Image promptImage;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    // --- 状态变量 ---
    private Vector3 targetWorldPos;
    private bool isTrackingPos = false;
    private Camera mainCamera;

    // 动画相关
    private Coroutine animCoroutine;

    // 权限锁：记录当前正在显示提示的“触发器物体”
    private Object currentRequester;

    // 对话相关
    private bool isInDialogue = false;
    private Queue<string> currentDialogueLines = new Queue<string>();

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

        mainCamera = Camera.main;
        if (mainCamera == null) mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        // 1. 图标位置跟随
        if (isTrackingPos && promptPanel.activeSelf && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);
            promptPanel.transform.position = screenPos;
        }

        // 2. 对话输入
        if (isInDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
        }
    }

    public void ShowPrompt(Sprite[] frames, float animSpeed, Vector3 worldPos, Object requester)
    {
        if (promptPanel != null && promptImage != null)
        {
            currentRequester = requester;
            targetWorldPos = worldPos;
            isTrackingPos = true;

            if (animCoroutine != null) StopCoroutine(animCoroutine);

            if (frames != null && frames.Length > 0)
            {
                // 1. 先设置图片
                promptImage.sprite = frames[0];

                // 2. 【关键修改】让图片恢复素材本身的像素尺寸
                // 这样 16px 的图就是 16 大小，32px 的图就是 32 大小，比例永远是 1:1
                promptImage.SetNativeSize();

                // 3. 【关键修改】取消 Preserve Aspect，因为 NativeSize 不需要它
                promptImage.preserveAspect = false;

                // 4. 【关键修改】统一放大倍数
                // 因为像素图原尺寸通常很小，我们统一放大 2 倍或 3 倍
                // 这样无论你是长条还是正方形，里面的像素颗粒看起来都是一样大的
                promptImage.rectTransform.localScale = Vector3.one * 0.6f; // 这里改倍数，比如 2f, 3f

                if (frames.Length > 1)
                {
                    animCoroutine = StartCoroutine(PlaySpriteAnim(frames, animSpeed));
                }
            }

            promptPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏提示（带权限检查）
    /// </summary>
    /// <param name="requester">请求者</param>
    public void HidePrompt(Object requester)
    {
        // 关键逻辑：如果来请求隐藏的人，不是当前正在显示的人，就无视它！
        // 这能完美解决玩家进入Trigger A时，还没完全离开Trigger A导致频繁闪烁的问题
        if (currentRequester != requester) return;

        if (promptPanel != null)
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            isTrackingPos = false;
            promptPanel.SetActive(false);
            currentRequester = null; // 释放锁
        }
    }

    // 简单的帧动画播放器
    IEnumerator PlaySpriteAnim(Sprite[] frames, float speed)
    {
        int index = 0;
        while (true)
        {
            promptImage.sprite = frames[index];
            index = (index + 1) % frames.Length; // 循环索引
            yield return new WaitForSeconds(speed);
        }
    }

    // ================== 对话部分保持不变 ==================
    public void StartDialogue(string[] lines)
    {
        isInDialogue = true;
        if (playerMovement != null)
        {
            playerMovement.enableInput = false;
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        currentDialogueLines.Clear();
        foreach (string line in lines) currentDialogueLines.Enqueue(line);

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
        dialogueText.text = currentDialogueLines.Dequeue();
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        if (playerMovement != null) playerMovement.enableInput = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 10)] public string text;
    // 【修改】改为列表，支持多选
    public List<KeyCode> requiredKeys;
}

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

    private Vector3 targetWorldPos;
    private bool isTrackingPos = false;
    private Camera mainCamera;
    private Coroutine animCoroutine;
    private Object currentRequester;

    public bool isInDialogue = false;
    private Queue<DialogueData> currentDialogueQueue = new Queue<DialogueData>();
    private DialogueData currentLineData;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (promptPanel) promptPanel.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>();
        mainCamera = Camera.main;
        if (mainCamera == null) mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (isTrackingPos && promptPanel.activeSelf && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);
            promptPanel.transform.position = screenPos;
        }
    }

    void LateUpdate()
    {
        if (isInDialogue)
        {
            bool keyPressed = false;

            // 如果列表为空，默认按 Space 继续
            if (currentLineData.requiredKeys == null || currentLineData.requiredKeys.Count == 0)
            {
                if (Input.GetKeyDown(KeyCode.Space)) keyPressed = true;
            }
            else
            {
                // 遍历列表，只要按下了其中任意一个键，就通过
                foreach (var key in currentLineData.requiredKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        keyPressed = true;
                        break;
                    }
                }
            }

            if (keyPressed)
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
                promptImage.sprite = frames[0];
                promptImage.SetNativeSize();
                promptImage.preserveAspect = false;
                promptImage.rectTransform.localScale = Vector3.one * 0.6f;

                if (frames.Length > 1) animCoroutine = StartCoroutine(PlaySpriteAnim(frames, animSpeed));
            }
            promptPanel.SetActive(true);
        }
    }

    public void HidePrompt(Object requester)
    {
        if (currentRequester != requester) return;
        if (promptPanel != null)
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            isTrackingPos = false;
            promptPanel.SetActive(false);
            currentRequester = null;
        }
    }

    IEnumerator PlaySpriteAnim(Sprite[] frames, float speed)
    {
        int index = 0;
        while (true)
        {
            promptImage.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(speed);
        }
    }

    public void StartDialogue(DialogueData[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        isInDialogue = true;

        if (playerMovement != null)
        {
            playerMovement.enableInput = false;
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        currentDialogueQueue.Clear();
        foreach (var line in lines) currentDialogueQueue.Enqueue(line);

        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (currentDialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLineData = currentDialogueQueue.Dequeue();
        dialogueText.text = currentLineData.text;

        // 【修改】传递按键列表给 PlayerMovement
        if (playerMovement != null)
        {
            playerMovement.inputExceptionKeys = currentLineData.requiredKeys;
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        if (playerMovement != null)
        {
            playerMovement.enableInput = true;
            playerMovement.inputExceptionKeys = null; // 清空白名单
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private OpenAIClient openAIClient;

    [Header("UI References")]
    public ScrollRect chatScrollView;
    public Transform chatContent;
    public TMP_InputField inputField;
    public Button sendButton;

    [Header("Message Prefabs")]
    public GameObject userMessagePrefab;
    public GameObject aiMessagePrefab;

    [Header("Colors")]
    public Color userMessageColor = Color.blue;
    public Color aiMessageColor = Color.green;

    void Start()
    {
        //openAIClient = GetComponent<OpenAIClient>();

        // Setup button and input field events
        sendButton.onClick.AddListener(SendMessage);
        inputField.onSubmit.AddListener(OnInputSubmit);

        // Focus input field
        inputField.ActivateInputField();
    }

    void Update()
    {
        // Send message with Enter key
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(inputField.text))
        {
            SendMessage();
        }
    }

    private void OnInputSubmit(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        // Display user message
        AddMessage(message, true);

        // Send to OpenAI
        openAIClient.SendMessage(message);

        // Clear input and refocus
        inputField.text = "";
        inputField.ActivateInputField();

        // Show "thinking" indicator
        AddMessage("Thinking...", false, true);
    }

    public void OnAIResponse(string response)
    {
        // Remove "thinking" indicator
        RemoveLastMessage();

        // Add AI response
        AddMessage(response, false);
    }

    public void AddMessage(string text, bool isUser, bool isTemporary = false)
    {
        // Choose prefab based on sender
        GameObject prefab = isUser ? userMessagePrefab : aiMessagePrefab;
        GameObject messageObj = Instantiate(prefab, chatContent);

        // Set message text
        TextMeshProUGUI textComponent = messageObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = isUser ? userMessageColor : aiMessageColor;
        }

        // Mark as temporary if needed (for "thinking" messages)
        if (isTemporary)
        {
            messageObj.tag = "TempMessage";
        }

        // Auto-scroll to bottom
        StartCoroutine(ScrollToBottom());
    }

    private void RemoveLastMessage()
    {
        // Remove temporary messages (like "thinking...")
        GameObject tempMessage = GameObject.FindWithTag("TempMessage");
        if (tempMessage != null)
        {
            Destroy(tempMessage);
        }
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        chatScrollView.verticalNormalizedPosition = 0f;
    }
}
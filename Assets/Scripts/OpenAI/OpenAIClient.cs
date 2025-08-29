using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class OpenAIClient : MonoBehaviour
{
    [SerializeField] private ChatUI chatUI;
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    private string apiKey;

    void Awake()
    {
        LoadAPIKey();
    }

    private void LoadAPIKey()
    {

        // Fallback to resources file (for builds)
        apiKey = ObfuscatedAPIKey.GetAPIKey();

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("No OpenAI API key found! Add to environment variable or Resources/apikey.txt");
        }
        else
        {
            Debug.Log("API key loaded successfully!");
        }
    }

    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model = "gpt-3.5-turbo";
        public ChatMessage[] messages;
        public int max_tokens = 150;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public ChatMessage message;
    }

    public void SendMessage(string userMessage)
    {
        StartCoroutine(SendChatRequest(userMessage));
    }

    private IEnumerator SendChatRequest(string message)
    {
        ChatRequest request = new ChatRequest
        {
            messages = new ChatMessage[]
            {
                new ChatMessage { role = "user", content = message }
            }
        };

        string jsonData = JsonConvert.SerializeObject(request);

        using (UnityWebRequest webRequest = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseText = webRequest.downloadHandler.text;
                ChatResponse response = JsonConvert.DeserializeObject<ChatResponse>(responseText);

                string aiReply = response.choices[0].message.content;
                Debug.Log("AI Response: " + aiReply);

                // Handle the response in your game
                OnAIResponseReceived(aiReply);
                chatUI.OnAIResponse(aiReply);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    private void OnAIResponseReceived(string response)
    {
        // Implement your game logic here
        // For example, display the response in UI
        
    }
}

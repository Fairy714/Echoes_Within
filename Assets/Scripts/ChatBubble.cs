using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    [Header("References")]
    public RectTransform bubbleBackground;
    public TextMeshProUGUI messageText;

    [Header("Settings")]
    public Vector2 padding = new Vector2(30f, 20f);
    public float maxWidth = 400f;
    public float minWidth = 80f;
    public float minHeight = 40f;

    public void SetMessage(string text)
    {
        messageText.text = text;
        ResizeBubble();
    }

    public void ResizeBubble()
    {
        // Force text mesh to update
        messageText.ForceMeshUpdate();

        // Get text bounds
        Vector2 textSize = messageText.GetRenderedValues(false);

        // Apply constraints
        textSize.x = Mathf.Min(textSize.x, maxWidth);
        textSize.x = Mathf.Max(textSize.x, minWidth - padding.x);
        textSize.y = Mathf.Max(textSize.y, minHeight - padding.y);
       

        // Set text rect size
        messageText.rectTransform.sizeDelta = textSize;

        // Set bubble size with padding
        Vector2 bubbleSize = textSize + padding;
        bubbleBackground.sizeDelta = bubbleSize;

        // Also set root transform size
        GetComponent<RectTransform>().sizeDelta = bubbleSize;
        bubbleBackground.offsetMin = Vector2.zero;  // Sets Left and Bottom to 0
        bubbleBackground.offsetMax = Vector2.zero;  // Sets Right and Top to 0
    }
}
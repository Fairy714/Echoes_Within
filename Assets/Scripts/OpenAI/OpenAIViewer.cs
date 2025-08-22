using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OpenAIViewer : MonoBehaviour
{
    [SerializeField] private TMP_InputField TMP_InputField;
    [SerializeField] private OpenAIClient OpenAI_Client;

    public void SubmitResponse()
    {
        OpenAI_Client.SendMessage(TMP_InputField.text);
    }
}

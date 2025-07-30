using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VNEngine;

public class AltarUIManager : MonoBehaviour
{
    [SerializeField] private GameObject altar_obj;
    [SerializeField] private TMP_Text name_;
    [SerializeField] private TMP_Text personality_;
    [SerializeField] private TMP_Text weaknesses_;
    [SerializeField] private TMP_Text strengths_;
    [SerializeField] private Button next;
    


    public void InitializeAltarUI(Altar altar)
    {
        altar_obj.SetActive(false);
        name_.text = altar.altar_name;
        personality_.text = altar.altar_personality;
        weaknesses_.text = string.Empty;
        foreach( var weakness in altar.altar_weaknesses)
        {
            weaknesses_.text += "- " + weakness + "\n";
        }
        strengths_.text = string.Empty;
        foreach (var strength in altar.altar_strengths)
        {
            strengths_.text += "- " + strength + "\n";
        }
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(() => altar_obj.SetActive(false));

        altar_obj.SetActive(true);
    }

    

    public void SetNextConversation(ConversationManager nextConversation)
    {
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(nextConversation.Start_Conversation);
        next.onClick.AddListener(() => altar_obj.SetActive(false));

    }
}


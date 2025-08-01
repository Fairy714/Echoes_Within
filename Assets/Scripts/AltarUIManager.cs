using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VNEngine;

public class AltarUIManager : MonoBehaviour
{
    private const string TRIGGER_SLIDE_IN = "In";
    private const string TRIGGER_SLIDE_OUT = "Out";
    [SerializeField] private GameObject altar_obj;
    [SerializeField] private TMP_Text name_;
    [SerializeField] private TMP_Text personality_;
    [SerializeField] private TMP_Text weaknesses_;
    [SerializeField] private TMP_Text strengths_;
    [SerializeField] private Button next;
    [SerializeField] private Animator animator_;
    


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
        next.onClick.AddListener(() => animator_.SetTrigger(TRIGGER_SLIDE_OUT));

        altar_obj.SetActive(true);
        animator_.SetTrigger(TRIGGER_SLIDE_IN);
    }

    

    public void SetNextConversation(ConversationManager nextConversation)
    {
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(nextConversation.Start_Conversation);
        next.onClick.AddListener(() => animator_.SetTrigger(TRIGGER_SLIDE_OUT));

    }
}


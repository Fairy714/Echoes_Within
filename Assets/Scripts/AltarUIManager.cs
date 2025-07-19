using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AltarUIManager : MonoBehaviour
{
    [SerializeField] private GameObject altar_obj;
    [SerializeField] private TMP_Text name_;
    [SerializeField] private TMP_Text personality_;
    [SerializeField] private TMP_Text belief_;
    [SerializeField] private TMP_Text weaknesses_;
    [SerializeField] private TMP_Text strengths_;


    public void InitializeAltarUI(Altar altar)
    {
        altar_obj.SetActive(false);
        name_.text = altar.altar_name;
        personality_.text = altar.altar_personality;
        belief_.text = altar.altar_belief;
        foreach( var weakness in altar.altar_weaknesses)
        {
            weaknesses_.text += weakness;
        }
        foreach (var strength in altar.altar_strengths)
        {
            strengths_.text += strength;
        }
        altar_obj.SetActive(true);
    }
}

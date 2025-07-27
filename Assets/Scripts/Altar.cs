using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Altar", menuName = "VN/Create Altar")]
public class Altar : ScriptableObject
{
    public string altar_name;
    public string altar_personality;
    public List<string> altar_weaknesses;
    public List<string> altar_strengths;
}

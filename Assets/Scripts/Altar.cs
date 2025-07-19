using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : ScriptableObject
{
    public string altar_name { get; private set; }
    public string altar_personality { get; private set; }
    public string altar_belief { get; private set; }
    public List<string> altar_weaknesses { get; private set; }
    public List<string> altar_strengths { get; private set; }
}

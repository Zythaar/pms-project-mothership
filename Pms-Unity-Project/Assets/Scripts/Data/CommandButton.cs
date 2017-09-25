using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandButton 
{
    public string name; // tooltip
    [Header("Functionality")]
    public CommandButtonName gridButton;
    public string invokeKey; // axis
    //public KeyCode invokeKey;
    public CommandActionResponse commandActionResponse;
    [Header("UI")]
    //public string uiText;
    public Sprite icon;
}

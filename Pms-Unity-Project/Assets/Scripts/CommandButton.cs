using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandButton 
{
    [Header("Functionality")]
    public CommandButtonName gridButton;
    public KeyCode invokeKey;
    public CommandAction invokeCommandAction;
    [Header("UI")]
    public string name;
    public Sprite icon;
}

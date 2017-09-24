using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CommandButton/Layout")]
public class CommandButtonLayout : ScriptableObject 
{
    public CommandButton[] commandButtons;
    new public string name;
    public EntityType entityType;
}

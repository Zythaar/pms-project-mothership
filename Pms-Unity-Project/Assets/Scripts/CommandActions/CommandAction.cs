using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandAction : ScriptableObject 
{
    new public string name;
    public abstract bool DoCommandAction(CommandController controller);
}

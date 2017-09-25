using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CommandActionResponse")]
public class CommandActionResponse : ScriptableObject 
{
    //new public string name;
    public string key;
    public virtual bool DoCommandAction(CommandController controller, string key)
    {
        return controller.AttemptToInvokeCommand(key);
    }
}

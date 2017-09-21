using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandAction : ScriptableObject 
{


    public abstract bool DoCommandAction(CommandController controller);
}

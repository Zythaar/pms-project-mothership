using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour 
{
    public CommandButtonLayout[] commandButtonLayouts;

    int commandButtonCount = System.Enum.GetValues(typeof(CommandButtonName)).Length;

    CommandButton[] commandButtons = new CommandButton[System.Enum.GetValues(typeof(CommandButtonName)).Length];
    //CommandAction[] commandActions = new CommandAction[System.Enum.GetValues(typeof(CommandButtonName)).Length];
    //KeyCode[] keyCodes = new KeyCode[System.Enum.GetValues(typeof(CommandButtonName)).Length];
    bool[] commandButtonActive = new bool[System.Enum.GetValues(typeof(CommandButtonName)).Length];

    public void SetCommandButton(int buttonIndex, CommandButton buttonToSet)
    {
        commandButtons[buttonIndex] = buttonToSet;
        commandButtonActive[buttonIndex] = true;
    }

    public void ResetAllCommandButtonActions()
    {
        for (int i = 0; i < commandButtonCount; i++)
        {
            commandButtons[i] = null;
            commandButtonActive[i] = false;
        }
    }

    public bool InvokeCommandButton(int buttonIndex)
    {
        if (commandButtons[buttonIndex] != null)
        {
            commandButtons[buttonIndex].invokeCommandAction.DoCommandAction(this);
            return true;
        }
        return false;
    }

    #region Ship Commands
    public void Move()
    {
        Debug.Log("Move command invoked");
    }

    public void Stop()
    {
        Debug.Log("Stop command invoked");
    }

    public void Hold()
    {
        Debug.Log("Hold command invoked");
    }

    public void Patrol()
    {
        Debug.Log("Patrol command invoked");
    }

    public void Attack()
    {
        Debug.Log("Attack command invoked");
    }
    #endregion


    private void Update()
    {
        for (int i = 0; i < commandButtonCount; i++)
        {
            if (commandButtonActive[i])
            {
                if (Input.GetKeyDown(commandButtons[i].invokeKey))
                {
                    InvokeCommandButton(i);
                }
            }
        }
    }

}

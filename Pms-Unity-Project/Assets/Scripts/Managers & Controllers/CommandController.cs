using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour 
{
    public CommandButtonLayout[] commandButtonLayouts;

    int commandButtonCount = System.Enum.GetValues(typeof(CommandButtonName)).Length;

    CommandButton[] commandButtons = new CommandButton[System.Enum.GetValues(typeof(CommandButtonName)).Length];
    bool[] commandButtonActive = new bool[System.Enum.GetValues(typeof(CommandButtonName)).Length];

    delegate bool CommandAction();
    CommandAction commandAction;

    Dictionary<string, CommandAction> commandDictionary = new Dictionary<string, CommandAction>();

    private void Start()
    {
        AddCommandsToDicionary();
    }

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

    public void SetCommandButton(CommandButton buttonToSet)
    {
        int buttonIndex = (int)buttonToSet.gridButton;

        commandButtons[buttonIndex] = buttonToSet;
        commandButtonActive[buttonIndex] = true;
    }

    //public void SetCommandButton(int buttonIndex, CommandButton buttonToSet)
    //{
    //    commandButtons[buttonIndex] = buttonToSet;
    //    commandButtonActive[buttonIndex] = true;
    //}

    public void ResetAllCommandButtonActions()
    {
        for (int i = 0; i < commandButtonCount; i++)
        {
            commandButtons[i] = null;
            commandButtonActive[i] = false;
        }
    }

    public bool InvokeCommandButton(int buttonIndex) // UI Button or Key pressed
    {
        CommandButton button = commandButtons[buttonIndex];
        if (button != null)
        {
            return button.commandActionResponse.DoCommandAction(this, button.commandActionResponse.key); ;
        }
        return false;
    }

    public bool AttemptToInvokeCommand(string key) // Command Action Response invokation in Scriptable object
    {
        if (commandDictionary.ContainsKey(key))
        {
            CommandAction action = commandDictionary[key];
            if (action != null)
            {
                action.Invoke();
                return true;
            }
            else
            {
                Debug.LogWarning("No action assigned to " + key);
            }
        }
        return false;
    }

    #region Command Actions
    // foreach command exits one CommandActionResponse Asset
    // the key string on the asset and the dictionary MUST be indentical to work
    void AddCommandsToDicionary() // Add commands to the dictionary
    {
        commandDictionary.Clear();

        commandDictionary.Add("move", Move);
        commandDictionary.Add("stop", Stop);
        commandDictionary.Add("hold", Hold);
        commandDictionary.Add("patrol", Patrol);
        commandDictionary.Add("attack", Attack);

        commandDictionary.Add("cancel", Cancel);

        commandDictionary.Add("gather", Gather);
        commandDictionary.Add("build", Build);
        commandDictionary.Add("repair", Repair);

        commandDictionary.Add("load_pos", LoadPosition);

        commandDictionary.Add("craft_ship", CraftShip);
        commandDictionary.Add("open_inventory", OpenInventory);
    }

    #region Ship Command Actions
    public bool Move()
    {
        Debug.Log("Move command invoked");
        return false;
    }

    public bool Stop()
    {
        Debug.Log("Stop command invoked");
        return false;
    }

    public bool Hold()
    {
        Debug.Log("Hold command invoked");
        return false;
    }

    public bool Patrol()
    {
        Debug.Log("Patrol command invoked");
        return false;
    }
    #endregion

    public bool Attack()
    {
        Debug.Log("Attack command invoked");
        return false;
    }

    public bool Cancel()
    {
        Debug.Log("Cancel command invoked");
        return false;
    }

    #region Ship - Worker
    public bool Gather()
    {
        Debug.Log("Gather command invoked");
        return false;
    }

    public bool Build()
    {
        Debug.Log("Build command invoked");
        return false;
    }

    public bool Repair()
    {
        Debug.Log("Repair command invoked");
        return false;
    }
    #endregion

    #region Ship - Transporter
    public bool LoadPosition()
    {
        Debug.Log("LoadPosition command invoked");
        return false;
    } 
    #endregion

    #region Station

    public bool CraftShip()
    {
        Debug.Log("CraftShip command invoked");
        return false;
    }

    public bool OpenInventory()
    {
        Debug.Log("OpenInventory command invoked");
        return false;
    }

    #endregion

    #endregion

}

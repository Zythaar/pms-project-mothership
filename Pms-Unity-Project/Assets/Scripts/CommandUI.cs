using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour 
{
    public enum CommandButtonName { Q, W, E, R, T, A, S, D, F, G, Z, X, C, V, B };
    
    public Button[] commandButtons;
    public UnityEngine.Events.UnityAction action;

    private void Awake()
    {
        //for (int i = 0; i < commandButtons.Length; i++)
        //{
        //    commandButtons[i].onClick.AddListener(action);
        //}
    }

    public void OnClickCommandButton(CommandButtonName buttonName)
    {

    }

    public void OnClickCommandButton(int buttonIndex)
    {
        Debug.Log(((CommandButtonName)buttonIndex).ToString());
    }

    public class SubcribedCommandUI
    {
        public CommandButtonName commandButton;
        public string buttonName;

        public void Command()
        {

        }
    }


    // Use this for initialization
    private void Start () 
	{
		
	}
	
	// Update is called once per frame
	private void Update () 
	{
		
	}
}

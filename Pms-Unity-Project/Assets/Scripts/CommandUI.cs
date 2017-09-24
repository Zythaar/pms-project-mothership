using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour 
{

    public CommandButtonLayout layout;
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

        CommandController controller = GameManager.singleton.CommandController;

        commandController.InvokeCommandButton(buttonIndex);

        //switch ((CommandButtonName)buttonIndex)
        //{
        //    case CommandButtonName.Q:
        //        commandActions[buttonIndex].DoCommandAction(controller);
        //        break;
        //    case CommandButtonName.W:
        //        break;
        //    case CommandButtonName.E:
        //        break;
        //    case CommandButtonName.R:
        //        break;
        //    case CommandButtonName.T:
        //        break;
        //    case CommandButtonName.A:
        //        break;
        //    case CommandButtonName.S:
        //        break;
        //    case CommandButtonName.D:
        //        break;
        //    case CommandButtonName.F:
        //        break;
        //    case CommandButtonName.G:
        //        break;
        //    case CommandButtonName.Z:
        //        break;
        //    case CommandButtonName.X:
        //        break;
        //    case CommandButtonName.C:
        //        break;
        //    case CommandButtonName.V:
        //        break;
        //    case CommandButtonName.B:
        //        break;
        //    default:
        //        break;
        //}
    }
    
    public class SubcribedCommandUI
    {
        public CommandButtonName commandButton;
        public string buttonName;

        public void Command()
        {

        }
    }
    SelectionController selectionController;
    CommandController commandController;

    PMS.UI.Selectable[] selectedShips, selectedStations;

    void OnSelectionChanged()
    {
        Debug.Log("Onselection changed Commandui");
        selectionController.GetCurrentSelected(out selectedShips, out selectedStations);
        Debug.Log("selectedShips: " + selectedShips.Length + ", selectedStations: " + selectedStations.Length);
        if (selectedShips.Length > 0 && selectedStations.Length > 0) // both
        {

        }
        else if (selectedShips.Length > 0 && selectedStations.Length == 0) // only ships
        {
            Debug.Log("only ships selected");
            ChangeGridButton(0);
        }
        else if (selectedShips.Length == 0 && selectedStations.Length > 0) // only stations
        {

        }
        else if (selectedShips.Length == 0 && selectedStations.Length == 0) // nothing
        {
            DeactivateAllButtons();
        }

    }

    void ChangeGridButton(CommandButtonName buttonName)
    {
        ChangeGridButton((int)buttonName);
    }
    

    void ChangeGridButton(int buttonIndex)
    {
        CommandButton button = layout.commandButtons[buttonIndex];

        commandButtons[buttonIndex].interactable = true;
        commandButtons[buttonIndex].GetComponent<Image>().sprite = button.icon;
        commandButtons[buttonIndex].GetComponentInChildren<Text>().text = button.name;
        commandController.SetCommandButton(buttonIndex, button);
    }

    void DeactivateAllButtons()
    {
        for (int i = 0; i < commandButtons.Length; i++)
        {
            commandButtons[i].interactable = false;
            commandButtons[i].GetComponent<Image>().sprite = null;
            commandButtons[i].GetComponentInChildren<Text>().text = "";
        }

    }

    // Use this for initialization
    private void Start () 
	{
        selectionController = GameManager.singleton.SelectionController;
        commandController = GameManager.singleton.CommandController;

        selectionController.onSelectionChanged += OnSelectionChanged;

        DeactivateAllButtons();
    }
	
	// Update is called once per frame
	private void Update () 
	{
		
	}
}

public enum CommandButtonName { Q, W, E, R, T, A, S, D, F, G, Z, X, C, V, B };
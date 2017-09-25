using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour 
{

    public CommandButtonLayout[] layouts;

    public Button[] commandButtons;

    PMS.UI.Selectable[] selectedShips, selectedStations;

    EntityType focusedEntityType;
    EntityID focusedEntityID;

    SelectionController selectionController;
    CommandController commandController;



    // Use this for initialization
    private void Start()
    {
        selectionController = GameManager.singleton.SelectionController;
        commandController = GameManager.singleton.CommandController;

        selectionController.onSelectionChanged += OnSelectionChanged;

        DeactivateAllButtons();
    }

    public void OnClickCommandButton(int buttonIndex)
    {
        Debug.Log(((CommandButtonName)buttonIndex).ToString());

        commandController.InvokeCommandButton(buttonIndex);
    }
    
    void OnSelectionChanged()
    {
        Debug.Log("Onselection changed Commandui");
        selectionController.GetCurrentSelected(out selectedShips, out selectedStations);
        Debug.Log("selectedShips: " + selectedShips.Length + ", selectedStations: " + selectedStations.Length);
        if (selectedShips.Length > 0 && selectedStations.Length > 0) // both
        {
            // load 
            Debug.Log("Both selected");
            DeactivateAllButtons();
        }
        else if (selectedShips.Length > 0 && selectedStations.Length == 0) // only ships
        {
            Debug.Log("only ships selected");
            ChangeCommandButtonLayout(CommandButtonLayoutName.BasicShip);
        }
        else if (selectedShips.Length == 0 && selectedStations.Length > 0) // only stations
        {
            // load 
            Debug.Log("only stations selected");
            ChangeCommandButtonLayout(CommandButtonLayoutName.BasicStation);
        }
        else if (selectedShips.Length == 0 && selectedStations.Length == 0) // nothing
        {
            DeactivateAllButtons();
        }
    }

    void ChangeCommandButtonLayout(CommandButtonLayoutName layoutName)
    {
        ChangeCommandButtonLayout((int)layoutName);
    }

    void ChangeCommandButtonLayout(int index)
    {
        if (index < 0 || index >= layouts.Length)
        {
            Debug.LogError("index out of Range");
        }
        CommandButtonLayout currentLayout = layouts[index];

        if (currentLayout)
        {
            foreach (CommandButton button in currentLayout.commandButtons)
            {
                ChangeGridButton(button);
            }
        }
    }

    void ChangeGridButton(CommandButton button)
    {
        int buttonIndex = (int)button.gridButton;
        commandButtons[buttonIndex].interactable = true;
        commandButtons[buttonIndex].GetComponent<Image>().sprite = button.icon;
        commandButtons[buttonIndex].GetComponentInChildren<Text>().text = button.invokeKey.ToUpper();
        commandController.SetCommandButton(button);
    }

    //void ChangeGridButton(CommandButtonName buttonName, CommandButton button)
    //{
    //    ChangeGridButton((int)buttonName, button);
    //}
    

    //void ChangeGridButton(int buttonIndex, CommandButton button)
    //{
    //    //CommandButton button = layout.commandButtons[buttonIndex];

    //    commandButtons[buttonIndex].interactable = true;
    //    commandButtons[buttonIndex].GetComponent<Image>().sprite = button.icon;
    //    commandButtons[buttonIndex].GetComponentInChildren<Text>().text = button.name;
    //    commandController.SetCommandButton(buttonIndex, button);
    //}

    void DeactivateAllButtons()
    {
        for (int i = 0; i < commandButtons.Length; i++)
        {
            commandButtons[i].interactable = false;
            commandButtons[i].GetComponent<Image>().sprite = null;
            commandButtons[i].GetComponentInChildren<Text>().text = "";
        }
    }

}

public enum CommandButtonName { Q, W, E, R, T, A, S, D, F, G, Z, X, C, V, B };

public enum CommandButtonLayoutName { BasicShip, BasicStation, WorkerShip, TransporterShip, ShipYardStation, StorageStation  };
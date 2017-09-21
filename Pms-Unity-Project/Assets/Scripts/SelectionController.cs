using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using PMS.UI;

public class SelectionController : MonoBehaviour 
{
    public static int cameraOffset = -11;
    public LayerMask selectableMask;

    public delegate void OnPointed();
    public OnPointed onPointed;

    public delegate void OnDePointed();
    public OnDePointed onDePointed;

    public delegate void OnSelected();
    public OnSelected onSelected;

    public delegate void OnDeselected();
    public OnDeselected onDeselected;

    public delegate void OnSelectionChanged();
    public OnSelectionChanged onSelectionChanged;


    public enum PointerEntityState { Void, Ship, Station, Minable, GUI };
    public PointerEntityState pointerEntityState;
    public Selectable pointedSelectable;
    public List<Selectable> currentSelectedList;

    public List<Selectable> currentPointedList;

    public List<Selectable> currentSelectedShipList;
    public List<Selectable> currentSelectedStationList;

    [SerializeField]
    int currentSelectedShipCount;
    [SerializeField]
    int currentSelectedStationCount;

    public List<Selectable> ownedSelectableList;

    public bool isBoxSelecting;
    public bool isDoubleClick;
    public Vector3 mousePositionOld;


    Camera cam;
    public Color boxColor;
    public Color boxBorderColor;
    float thickness = 2f;
        /*
         * void OnPointed()
        {

        }

        void OnDePointed()
        {

        }

        void OnSelected()
        {

        }

        void OnDeselected()
        {

        }
         * */

    // Use this for initialization
    private void Start () 
	{
        cam = Camera.main;
        currentSelectedList = new List<Selectable>();

    }
    private void OnGUI()
    {
        if (isBoxSelecting)
        {
            var rect = Utils.GetScreenRect(mousePositionOld, Input.mousePosition);
            Utils.DrawScreenRect(rect, boxColor);
            Utils.DrawScreenRectBorder(rect, thickness, boxBorderColor);
        }
    }
    // Update is called once per frame
    private void Update () 
	{

        #region PointerEntityStateBehaviour
        if (EventSystem.current.IsPointerOverGameObject())
        {
            pointerEntityState = PointerEntityState.GUI;
        }
        else
        {
            // MULTIPLE HITS NEEDED ?????
            // Shoot ray to detect selectables
            RaycastHit2D[] hits = Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset), Vector2.zero, 100, selectableMask);    // Collision Layer

            foreach (RaycastHit2D hit in hits)
            {
                if (hit) // ray has detected anything
                {
                    Selectable newSelectable = hit.collider.GetComponentInParent<Selectable>(); // Check for selectable

                    if (newSelectable) // has hit a selectable
                    {
                        // Check if the entity is owned by the player


                        // Set the pointer type of the controller
                        if (newSelectable.entityType == EntityType.Ship)
                        {
                            pointerEntityState = PointerEntityState.Ship;
                        }
                        else if (newSelectable.entityType == EntityType.Station)
                        {
                            pointerEntityState = PointerEntityState.Station;
                        }
                        else if (newSelectable.entityType == EntityType.Minable)
                        {
                            pointerEntityState = PointerEntityState.Minable;
                        }


                        SetOnPointed(newSelectable);   // Set the pointed state of the entity
                    }

                }
            }

            if (hits.Length == 0) // nothing hit
            {
                pointerEntityState = PointerEntityState.Void;// Void State

                SetOnDepointed(pointedSelectable);
            } 
        }
        #endregion

        #region Pointer Begin

        if (Input.GetMouseButtonDown(0))    //Start boxselecting
        {
            if (pointerEntityState != PointerEntityState.GUI)
            {
                mousePositionOld = Input.mousePosition;
                isBoxSelecting = true;
            }
        }
        #endregion

        #region Pointer End
        if (Input.GetMouseButtonUp(0)) // Mouse up behaviour
        {
            //selectedInteractableList.Clear();

            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (pointerEntityState == PointerEntityState.Void || !shift) // Deselect all in the void or not shift
            {
                DeselectAll();
            }

            if (pointerEntityState != PointerEntityState.Void && pointerEntityState != PointerEntityState.GUI)
            {
                if (shift)    // Up Shift
                {
                    bool isSelected = pointedSelectable.IsSelected;
                    isSelected = !isSelected;
                    Debug.Log("is Selected " + isSelected);
                    if (isSelected == true)
                    {
                        Select(pointedSelectable);
                    }
                    else
                    {
                        Deselect(pointedSelectable);
                    }
                }
                else
                {
                    Select(pointedSelectable);
                }

                if (isDoubleClick || control)
                {
                    //SelectEqualEntitiesInCameraView();
                    foreach (Selectable selectable in ownedSelectableList)
                    {
                        if (selectable.entityID == pointedSelectable.entityID)
                        {
                            if (cam.pixelRect.Contains(cam.WorldToViewportPoint(selectable.transform.position))) // clamp in cam view
                            {
                                Select(selectable);
                            }
                        }
                    }
                }
            }

            #region Box Selecting

            if (isBoxSelecting && mousePositionOld != Input.mousePosition) // Box selecting
            {
                int currentBoxedShipCount = 0, currentBoxedStationCount = 0;
                foreach (Selectable selectable in ownedSelectableList)
                {
                    if (Utils.GetViewportBounds(cam, mousePositionOld, Input.mousePosition).Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                    {
                        if (!currentPointedList.Contains(selectable))
                        {
                            currentPointedList.Add(selectable);
                            if (selectable.entityType == EntityType.Ship)
                            {
                                currentSelectedShipList.Add(selectable);
                                currentBoxedShipCount++;
                            }

                            if (selectable.entityType == EntityType.Station)
                            {
                                currentSelectedStationList.Add(selectable);
                                currentBoxedStationCount++;
                            }
                        }
                    }
                    else
                    {
                        if (currentPointedList.Contains(selectable))
                        {
                            currentPointedList.Remove(selectable);
                        }
                        Deselect(selectable);
                    }

                    if (selectable != pointedSelectable)
                    {
                        SetOnDepointed(selectable);
                    }
                }

                Debug.Log("Boxed Ships: " + currentBoxedShipCount + "\nBoxed Stations: " + currentBoxedStationCount);

                if (currentSelectedShipList.Count > 0 && currentSelectedStationList.Count > 0)
                {
                    //foreach (Selectable selectable in currentSelectedStationList) // 
                    //{

                    //    selectable.OnPointed();
                    //}
                    currentSelectedStationList.Clear();
                    foreach (Selectable selectable in currentSelectedShipList)
                    {
                        Select(selectable);
                        //selectable.OnDePointed();
                    }
                }
                else
                {
                    foreach (Selectable selectable in currentPointedList)
                    {
                        if (currentPointedList.Contains(selectable))
                        {
                            Select(selectable);
                            //selectable.OnDePointed();
                        }
                        //else
                        //{
                        //    Deselect(selectable);
                        //}
                    }
                }
            }


            isBoxSelecting = false;

            #endregion

            currentPointedList.Clear();
        }

        #endregion

        #region Box Pointing
        //Mouse HOLD
        if (isBoxSelecting && Input.GetMouseButton(0)) // Hold Rect
        {
            foreach (Selectable selectable in ownedSelectableList)
            {
                if (Utils.GetViewportBounds(cam, mousePositionOld, Input.mousePosition).Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                {
                    SetOnPointed(selectable, isBoxSelecting);
                    
                    //if (!currentPointedList.Contains(selectable))
                    //{
                    //    currentPointedList.Add(selectable);
                    //}
                }
                else
                {
                    if (selectable != pointedSelectable)
                    {
                        SetOnDepointed(selectable);
                        //if (currentPointedList.Contains(selectable))
                        //{
                        //    currentPointedList.Remove(selectable);
                        //}
                    }
                }
            }
        }
        #endregion

        #region Pointer Null
        // Mouse Released
        if (pointerEntityState == PointerEntityState.Void)
        {
            if (pointedSelectable != null)
                pointedSelectable.OnDePointed();

            pointedSelectable = null;
        } 
        #endregion
    }


    #region Interfaces

    public void SelectControlGroup(Selectable[] selectables)
    {
        // deleselect all
        // loop through selectables
        // select them
    }

    public void SelectAllArmie()
    {
        // deleselect all
        // loop through owned army list
    }

    public void SelectIdleWorker()
    {
        // get closest Idle worker
        // deselect all
        // select woker
    }

    #endregion


    void Select(Selectable newSelectable)
    {
        // don't pass in null values

        // select the entity

        // check the entity type

        // Check if the entity is allready in the appropriate List
         //// add the entity to the List if NOT in it

        // Invoke onSelectionChangedonSelectionChanged if NOT null

        ///////////////////
        // to do: make shure is not already selected???

        //if (currentSelectedShipCount > 0 && newSelectable.entityType == EntityType.Station)
        //{
        //    Debug.Log("Station was not selected because ships are already selected!");
        //    return;
        //}
        if (newSelectable != null)
            newSelectable.OnSelected();
        if (!currentSelectedList.Contains(newSelectable))
        {
            currentSelectedList.Add(newSelectable);

            IncrementEntityType(newSelectable);
        }
    }

    void SelectMultiple(Selectable[] newSelectables)
    {
        // don't pass in null values

        // loop through array

        // select the entity

        // check the entity type

        // Check if the entity is allready in the appropriate List
        //// add the entity to the List if NOT in it

        // Invoke onSelectionChanged if NOT null
    }

    void Deselect(Selectable newSelectable)
    {
        // don't pass in null values

        // deselect the entity

        // check the entity type

        // Check if the entity is allready in the appropriate List
        // remove the entity to the List if so

        // Invoke onSelectionChanged if NOT null
        
        ////////////
        newSelectable.OnDeselected();

        if (currentSelectedList.Contains(newSelectable))
        {
            currentSelectedList.Remove(newSelectable);

            DecrementEntityType(newSelectable);
        }
       
    }

    void DeselectAll()
    {
        // don't pass in null values

        // convert both List to (optional one??) Array

        // loop through both Arrays

        //// deselect the entity

        //// check the entity type

        //// Check if the entity is allready in the appropriate List
        //// remove the entity to the List if so

        // Invoke onSelectionChanged if NOT null

        // Clear both Lists

        ////////////


        //if (onDeselected != null)
        //{
        //    onDeselected();



        //}
        foreach (Selectable selectable in currentSelectedList.ToArray())
        {
            Deselect(selectable);
        }
        currentSelectedStationList.Clear();
        currentSelectedShipList.Clear();
        //currentSelectedList.Clear();
    }

    void SetOnPointed (Selectable newSelectable, bool isBoxSelecting) // Called inside box pointing
    {
        // don't pass over null selectables !!

        if (!isBoxSelecting) // pass bool inside the box pointing
        {
            SetOnDepointed(pointedSelectable); // de point the old pointedSelectable

            pointedSelectable = newSelectable; // overwrite the reference only when not Box Selecting
        }

        if (!newSelectable.IsPointed)   // don't point already pointed entities
        {
            newSelectable.OnPointed(); // set the given newSelectable to be pointed
        }
    }

    void SetOnPointed(Selectable newSelectable)
    {
        // don't pass over null selectables !!
        SetOnPointed(newSelectable, false); // called inside pointer Enity state
    }

    void SetOnDepointed (Selectable oldSelectable)
    {
        // null values can savely passed in

        if (oldSelectable != null && oldSelectable.IsPointed)   // do not depoint enttities that aren't pointed in the first place
        {
            oldSelectable.OnDePointed();
        }

        if (oldSelectable == pointedSelectable) // do not overwrite the currerntly pointed entity
        {
            pointedSelectable = null;
        }
    }

    void IncrementEntityType(Selectable newSelectable)
    {
        if (newSelectable.entityType == EntityType.Ship)
            currentSelectedShipCount++;

        if (newSelectable.entityType == EntityType.Station)
            currentSelectedStationCount++;
    }

    void DecrementEntityType(Selectable newSelectable)
    {
        if (newSelectable.entityType == EntityType.Ship)
            currentSelectedShipCount--;

        if (newSelectable.entityType == EntityType.Station)
            currentSelectedStationCount--;
    }

}

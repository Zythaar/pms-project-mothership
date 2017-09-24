using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using PMS.UI;

public class SelectionController : MonoBehaviour 
{
    public static int cameraOffset = -11;   //TEMPORARY

    public LayerMask selectableMask;

    public delegate void OnSelectionChanged();
    public OnSelectionChanged onSelectionChanged;

    public enum PointerEntityState { Void, Ship, Station, Minable, GUI };
    public PointerEntityState pointerEntityState;
    
    public List<Selectable> ownedSelectableList; // kept here and updated // or moved and get when needed

    private Selectable pointedSelectable;
    private List<Selectable> currentSelectedShipList;
    private List<Selectable> currentSelectedStationList;

    private bool isBoxSelecting; // make public if graphics get moved
    private bool isDoubleClick;
    private Vector3 mousePositionOld;

    private Camera cam;

    /* moved or kept ? */
    // scriptable object ????
    public Color boxColor;
    public Color boxBorderColor;
    float thickness = 2f;

    public void GetCurrentSelected(out Selectable[] selectedShips, out Selectable[] selectedStations)
    {
        selectedShips = currentSelectedShipList.ToArray();
        selectedStations = currentSelectedStationList.ToArray();
    }

    /* -------------------- */
    // Use this for initialization
    private void Start () 
	{
        cam = Camera.main;
        currentSelectedShipList = new List<Selectable>();
        currentSelectedStationList = new List<Selectable>();
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
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset), Vector2.zero, 100, selectableMask);    // Collision Layer

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
            else   // nothing hit
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
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || isDoubleClick;

            if (pointerEntityState == PointerEntityState.Void || !shift && pointerEntityState != PointerEntityState.GUI) // Deselect all in the void or not shift
            {
                DeselectAll();
            }

            if (pointerEntityState != PointerEntityState.Void && pointerEntityState != PointerEntityState.GUI)
            {
                List<Selectable> pointedSelectables = new List<Selectable>();


                if (!shift && !control) // Simple select
                {
                    Debug.Log("Simple select du simbel");
                    Select(pointedSelectable);
                }

                if (shift && !control)  // Shift select
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

                if (control)    // Controll Select (including control shift select)
                {
                    List<Selectable> selectables = new List<Selectable>();
                    foreach (Selectable selectable in ownedSelectableList)
                    {
                        if (selectable.entityID == pointedSelectable.entityID)
                        {
                            if (cam.pixelRect.Contains(cam.WorldToViewportPoint(selectable.transform.position))) // clamp in cam view
                            {
                                selectables.Add(selectable);
                            }
                        }
                    }

                    SelectMultiple(selectables.ToArray());
                }
            }

            #region Box Selecting

            if (isBoxSelecting && mousePositionOld != Input.mousePosition) // Box selecting
            {
                List<Selectable> boxedShipList = new List<Selectable>();
                List<Selectable> boxedStationList = new List<Selectable>();

                foreach (Selectable selectable in ownedSelectableList)
                {
                    if (Utils.GetViewportBounds(cam, mousePositionOld, Input.mousePosition).Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                    {
                        if (selectable.entityType == EntityType.Ship)   // check if ship
                        {
                            boxedShipList.Add(selectable); // collect boxed ships
                        }

                        if (selectable.entityType == EntityType.Station)    // check if station
                        {
                            boxedStationList.Add(selectable);   // collect boxed stations
                        }
                    }
                    else
                    {
                        Deselect(selectable); // Deselect entities not inside the box
                    }

                    if (selectable != pointedSelectable) // depoint all but the pointed entity
                    {
                        SetOnDepointed(selectable);
                    }
                }

                Debug.Log("Boxed Ships: " + boxedShipList.Count + "\nBoxed Stations: " + boxedStationList.Count);

                SelectMultiple(boxedShipList.ToArray());

                if (boxedShipList.Count == 0 && boxedStationList.Count > 0)
                {
                    SelectMultiple(boxedStationList.ToArray());
                } 
            }
            isBoxSelecting = false;
            #endregion
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
                }
                else
                {
                    if (selectable != pointedSelectable)
                    {
                        SetOnDepointed(selectable);
                    }
                }
            }
        }
        #endregion
    }

    private void OnGUI() // moved or kept
    {
        if (isBoxSelecting)
        {
            var rect = Utils.GetScreenRect(mousePositionOld, Input.mousePosition);
            Utils.DrawScreenRect(rect, boxColor);
            Utils.DrawScreenRectBorder(rect, thickness, boxBorderColor);
        }
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

        newSelectable.OnSelected(); // select the entity

        // check the entity type
        // Check if the entity is allready in the appropriate List
        //// add the entity to the List if NOT in it
        AddEntityToList(newSelectable);

        // Invoke onSelectionChangedonSelectionChanged if NOT null
        if (onSelectionChanged != null)
        {
            onSelectionChanged.Invoke();
        }
    }

    void SelectMultiple(Selectable[] newSelectables)
    {
        // don't pass in null values

        // loop through array
        foreach (Selectable newSelectable in newSelectables)
        {
            newSelectable.OnSelected(); // select the entity

            // check the entity type
            // Check if the entity is allready in the appropriate List
            //// add the entity to the List if NOT in it
            AddEntityToList(newSelectable);
        }

        // Invoke onSelectionChanged if NOT null
        if (onSelectionChanged != null)
        {
            onSelectionChanged.Invoke();
        }
    }

    void Deselect(Selectable newSelectable)
    {
        // don't pass in null values

        newSelectable.OnDeselected();   // deselect the entity

        // check the entity type
        // Check if the entity is allready in the appropriate List
        // remove the entity to the List if so
        RemoveEntityFromList(newSelectable);

        // Invoke onSelectionChanged if NOT null
        if (onSelectionChanged != null)
        {
            onSelectionChanged.Invoke();
        }
    }
    void DeselectAll()
    {
        // don't pass in null values

        // loop through both Arrays and deselect all
        foreach (Selectable selectable in currentSelectedShipList)
        {
            //if (notToBeDeselected != nu notToBeDeselected.entityType == EntityType.Ship && notToBeDeselected == selectable)
            //    continue;
            selectable.OnDeselected();
        }

        foreach (Selectable selectable in currentSelectedStationList)
        {
            //if (notToBeDeselected.entityType == EntityType.Station && notToBeDeselected == selectable)
            //    continue;
            selectable.OnDeselected();
        }

        // Clear both Lists
        currentSelectedStationList.Clear();
        currentSelectedShipList.Clear();

        // Invoke onSelectionChanged if NOT null
        if (onSelectionChanged != null)
        {
            onSelectionChanged.Invoke();
        }
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

    void AddEntityToList(Selectable newSelectable)
    {
        if (newSelectable.entityType == EntityType.Ship)
        {
            if (!currentSelectedShipList.Contains(newSelectable))
            {
                currentSelectedShipList.Add(newSelectable);
            }
        }

        if (newSelectable.entityType == EntityType.Station)
        {
            if (!currentSelectedStationList.Contains(newSelectable))
            {
                currentSelectedStationList.Add(newSelectable);
            }
        }
    }

    void RemoveEntityFromList(Selectable newSelectable)
    {
        if (newSelectable.entityType == EntityType.Ship)
        {
            if (currentSelectedShipList.Contains(newSelectable))
            {
                currentSelectedShipList.Remove(newSelectable);
            }
        }

        if (newSelectable.entityType == EntityType.Station)
        {
            if (currentSelectedStationList.Contains(newSelectable))
            {
                currentSelectedStationList.Remove(newSelectable);
            }
        }
    }

}

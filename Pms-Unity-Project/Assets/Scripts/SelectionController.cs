using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using PMS.UI;

public class SelectionController : MonoBehaviour 
{
    public static int cameraOffset = -11;
    public LayerMask movementMask;

    public delegate void OnPointed();
    public OnPointed onPointed;

    public delegate void OnDePointed();
    public OnDePointed onDePointed;

    public delegate void OnSelected();
    public OnSelected onSelected;

    public delegate void OnDeselected();
    public OnDeselected onDeselected;


    public enum PointerEntityState { Void, Ship, Station, Minable, GUI };
    public PointerEntityState pointerEntityState;
    public Selectable pointedSelectable;
    public List<Selectable> selectedInteractableList;

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
        selectedInteractableList = new List<Selectable>();

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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            pointerEntityState = PointerEntityState.GUI;
            
        }
            

        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset),Vector2.zero, 100, movementMask);    // Collision Layer

        if (hit)
        {
            Selectable selectable = hit.collider.GetComponentInParent<Selectable>();

            if (selectable)
            {
                // Check if the entity is owned by the player

                SetPointerEntityType(selectable); // Set the pointer type of the controller

                SetOnPointed(selectable);   // Set the pointed state of the entity
            }

        }
        else
        {
            SetPointerEntityType(null); // Void State
        }

        if (Input.GetMouseButtonDown(0))    //Start boxselecting
        {
            mousePositionOld = Input.mousePosition;
            isBoxSelecting = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //selectedInteractableList.Clear();

            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (pointerEntityState == PointerEntityState.Void)
            {
                DeselectAll();
            }

            if (!shift)
            {
                DeselectAll();
            }



            if (pointerEntityState != PointerEntityState.Void)
            {
                if (shift)    // Up Shift
                {
                    bool isSelected = pointedSelectable.GetSelectedState();
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
                    if (pointerEntityState != PointerEntityState.Void)
                        //SelectEqualEntitiesInCameraView();
                        foreach (Selectable selectable in ownedSelectableList)
                        {
                            if (selectable.entityID == pointedSelectable.entityID)
                            {
                                if (cam.pixelRect.Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                                {
                                    Select(selectable);
                                }
                            }
                        }
                } 
            }


            if (isBoxSelecting && mousePositionOld != Input.mousePosition)
            {
                foreach (Selectable selectable in ownedSelectableList)
                {
                    if (Utils.GetViewportBounds(cam, mousePositionOld, Input.mousePosition).Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                    {
                        Select(selectable);
                        selectable.OnDePointed();
                    }
                    else
                    {
                        Deselect(selectable);
                    }
                }
            }

            isBoxSelecting = false;
        }

        //Mouse HOLD
        if (isBoxSelecting && Input.GetMouseButton(0)) // Hold Rect
        {
            foreach (Selectable selectable in ownedSelectableList)
            {
                if (Utils.GetViewportBounds(cam, mousePositionOld, Input.mousePosition).Contains(cam.WorldToViewportPoint(selectable.transform.position)))
                {
                    selectable.OnPointed();
                }
                else
                {
                    if (selectable != pointedSelectable)
                        selectable.OnDePointed();
                }
            }
        }

        // Mouse Released
        if (pointerEntityState == PointerEntityState.Void)
        {
            if (pointedSelectable != null)
                pointedSelectable.OnDePointed();

            pointedSelectable = null;
        }
    }

    void SetPointerEntityType(Selectable newSelectable)
    {
        if (newSelectable == null)
        {
            pointerEntityState = PointerEntityState.Void;
            return;
        }


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
    }
    
    void Select(Selectable newSelectable)
    {
        newSelectable.OnSelected();
        if (!selectedInteractableList.Contains(newSelectable))
            selectedInteractableList.Add(newSelectable);
    }

    void Deselect(Selectable newSelectable)
    {
        newSelectable.OnDeselected();
        selectedInteractableList.Remove(newSelectable);
    }

    void DeselectAll()
    {
        if (onDeselected != null)
        {
            onDeselected();
            selectedInteractableList.Clear();
        }
    }
    void SetOnPointed(Selectable newSelectable)
    {
        if (newSelectable != null)
        {
            //if (onPointed != null)
            //{
            //    onPointed.Invoke();

            //}

            pointedSelectable = newSelectable;
        }

        newSelectable.OnPointed();

    }

}

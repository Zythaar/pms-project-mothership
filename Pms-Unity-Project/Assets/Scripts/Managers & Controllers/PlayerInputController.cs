using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PMS.UI;

public class PlayerInputController : MonoBehaviour 
{



    //   public Interactable focus;
    public static int cameraOffset = -11;// TEST
    public LayerMask movementMask;

    bool active;
    Motor[] selectedMotor;

    SelectionController selectionController;

    Camera cam;
    //   Motor motor;
    //   Selectable selectable;

    // Use this for initialization
    private void Start()
    {
        cam = Camera.main;
        //motor = GetComponent<Motor>();
        //selectable = GetComponent<Selectable>();
        selectionController = GetComponent<SelectionController>();
    }

    //   // Update is called once per frame
    private void Update()
    {
        if (!active)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset), Vector2.zero, 100, movementMask);    // Collision Layer


        if (Input.GetMouseButtonDown(1))
        {
            if (hit)
            {

            }

            //if (selectable.IsSelected)
            //{
            //    RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset), Vector2.zero, 100, movementMask);    // Collision Layer

            //    Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset);
            //    motor.MoveToPoint(target);

            //    if (hit)
            //    {
            //        Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

            //        if (interactable != null)
            //        {
            //            SetFocus(interactable);
            //        }
            //        //RemoveFocus();
            //    }
            //}
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    RemoveFocus();
        //}
    }

    void OnSelectionChanged()
    {
        Selectable[] selectedShips;
        selectionController.GetCurrentSelected(out selectedShips);

        if (selectedShips.Length > 0)
        {
            active = true;
        }
        else
        {
            active = false;
            return;
        }

        selectedMotor = new Motor[selectedShips.Length];
        for (int i = 0; i < selectedShips.Length; i++)
        {
            selectedMotor[i] = selectedShips[i].GetComponent<Motor>();
        }
    }

    //   void SetFocus(Interactable newFocus)
    //   {
    //       if (newFocus != focus)
    //       {
    //           if (focus != null)
    //               focus.OnDefocused();

    //           focus = newFocus;
    //           motor.FollowTarget(newFocus);
    //       }

    //       newFocus.OnFocused(transform);

    //   }

    //   void RemoveFocus()
    //   {
    //       if (focus != null)
    //           focus.OnDefocused();

    //       focus = null;
    //       motor.StopFollowingTarget();
    //   }


}

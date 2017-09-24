using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PMS.UI;

public class PlayerController : MonoBehaviour 
{
    public Interactable focus;

    public LayerMask movementMask;

    Camera cam;
    Motor motor;
    Selectable selectable;

    // Use this for initialization
    private void Start () 
	{
        cam = Camera.main;
        motor = GetComponent<Motor>();
        selectable = GetComponent<Selectable>();
    }
    public static int cameraOffset = -11;// TEST
    // Update is called once per frame
    private void Update () 
	{
        if (Input.GetMouseButtonDown(1))
        {
            if (selectable.IsSelected)
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset), Vector2.zero, 100, movementMask);    // Collision Layer

                Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition + Vector3.back * cameraOffset);
                motor.MoveToPoint(target);

                if (hit)
                {
                    Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

                    if (interactable != null)
                    {
                        SetFocus(interactable);
                    }
                    //RemoveFocus();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RemoveFocus();
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDefocused();

            focus = newFocus;
            motor.FollowTarget(newFocus);
        }

        newFocus.OnFocused(transform);

    }

    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();

        focus = null;
        motor.StopFollowingTarget();
    }


}

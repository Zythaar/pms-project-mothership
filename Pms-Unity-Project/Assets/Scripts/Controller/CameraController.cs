using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool scrollingEnabled = true;
    public float keyBoardScrollSpeed = 2;

    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public float pitch = 2f;

    private float currentZoom = 10f;

    Vector2 mousePos;
    float screenWidth;
    float screenheight;
    int pixelBorder;
    Vector3 scrollVelocity;

    bool isScrolling;

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        screenWidth = cam.pixelWidth;
        screenheight = cam.pixelHeight;

        currentZoom = cam.orthographicSize;

    }

    private void Update()
    {
        #region DEBUG
        if (Input.GetButtonDown("Jump"))
        {
            scrollingEnabled = !scrollingEnabled;
        }
        #endregion

        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (!scrollingEnabled)
            return;
        ScrollInput();
    }

    private void LateUpdate()
    {
        cam.orthographicSize = currentZoom * pitch;

        if (!scrollingEnabled)
            return;

        // KeayBoard Scrolling
        transform.Translate(scrollVelocity * keyBoardScrollSpeed * cam.orthographicSize * Time.deltaTime); // Move Camera scrollSpeed rel. to size

        //transform.position = new Vector3    // Begrenzt Transform innerhalb der Karte entsprechend der orthografischen Größe
        //(
        //    Mathf.Clamp(transform.position.x, mapBounds.min.x + targetOrtho * camRectRatio, mapBounds.max.x - targetOrtho * camRectRatio),    // Begrenzt x
        //    Mathf.Clamp(transform.position.y, mapBounds.min.y + targetOrtho, mapBounds.max.y - targetOrtho),
        //    -11
        //);
    }

    void ScrollInput()
    {
        mousePos = Input.mousePosition;

        isScrolling = Input.GetButton("Horizontal") || Input.GetButton("Vertical");

        if (!isScrolling)
        {
            if ((mousePos.x < pixelBorder) && (mousePos.y > (screenheight - pixelBorder)))    // topleft
            {

                scrollVelocity = new Vector3(-1, 1, 0);
            }
            else if ((mousePos.x > (screenWidth - pixelBorder)) && (mousePos.y > (screenheight - pixelBorder)))    // topright
            {

                scrollVelocity = new Vector3(1, 1, 0);
            }
            else if ((mousePos.x < pixelBorder) && (mousePos.y < pixelBorder))    // bottomleft
            {

                scrollVelocity = new Vector3(-1, -1, 0);
            }
            else if ((mousePos.x > (screenWidth - pixelBorder)) && (mousePos.y < pixelBorder))    // bottomright  
            {

                scrollVelocity = new Vector3(1, -1, 0);
            }
            else if (mousePos.x < pixelBorder)    // left
            {

                scrollVelocity = -Vector3.right;
            }
            else if (mousePos.x > (screenWidth - pixelBorder))    // right
            {

                scrollVelocity = Vector3.right;
            }
            else if (mousePos.y < pixelBorder)    // bottom
            {

                scrollVelocity = -Vector3.up;
            }
            else if (mousePos.y > (screenheight - pixelBorder))    // top  
            {

                scrollVelocity = Vector3.up;
            }
            else
            {
                scrollVelocity = Vector3.zero;    // Stop
            }
        }
        else
        {
            scrollVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        }

    }
}

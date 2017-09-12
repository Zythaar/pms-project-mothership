using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{

    static Texture2D whiteTexture;

    public static Texture2D WhiteTexture
    {
        get
        {
            if (whiteTexture == null)
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            return whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition0, Vector3 screenPosition1)
    {
        // Move origin from bottom left to top left
        screenPosition0.y = Screen.height - screenPosition0.y;
        screenPosition1.y = Screen.height - screenPosition1.y;
        // Calculate corners
        Vector2 topLeft = Vector3.Min(screenPosition0, screenPosition1);
        Vector2 bottomRight = Vector3.Max(screenPosition0, screenPosition1);
        //Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition0, Vector3 screenPosition1)
    {
        Vector3 v0 = Camera.main.ScreenToViewportPoint(screenPosition0);
        Vector3 v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        Vector3 min = Vector3.Min(v0, v1);
        Vector3 max = Vector3.Max(v0, v1);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}

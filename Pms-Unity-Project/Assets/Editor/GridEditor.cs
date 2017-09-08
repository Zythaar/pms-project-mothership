using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

    public override void OnInspectorGUI()
    {
        Grid grid = (Grid)target;

        if (DrawDefaultInspector())
        {
            if (grid.autoUpdate)
            {
                grid.GenerateGrid();
            }
        }

        if (GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();
        }

        //GUILayout.Label()

    }
}

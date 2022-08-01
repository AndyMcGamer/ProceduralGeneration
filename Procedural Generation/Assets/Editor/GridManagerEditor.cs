using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
[CanEditMultipleObjects]
public class GridManagerEditor : Editor
{
    GridManager gridManager;

    private SerializedProperty numOfSides;
    private SerializedProperty radius;


    private void OnEnable()
    {
        gridManager = (GridManager)target;
        numOfSides = serializedObject.FindProperty("numOfSides");
        radius = serializedObject.FindProperty("radius");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.IntSlider(numOfSides, 3, 30);
        EditorGUILayout.IntSlider(radius, 1, 15);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        if(GUILayout.Button("Draw Vertices"))
        {
            gridManager.DrawVertices();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Triangulate"))
        {
            gridManager.Triangulate();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Quadrilaterate"))
        {
            gridManager.Quadrilaterate();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Subdivide"))
        {
            gridManager.Subdivide();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear Gizmos"))
        {
            gridManager.Clear();
        }
    }
}

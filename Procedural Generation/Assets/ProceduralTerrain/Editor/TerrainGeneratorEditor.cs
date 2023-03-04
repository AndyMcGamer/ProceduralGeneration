using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            terrainGenerator.ClearDictionary();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.Generate(false);
        }
        if (GUILayout.Button("Generate All"))
        {
            terrainGenerator.Generate(true);
        }
        if (GUILayout.Button("Clear"))
        {
            terrainGenerator.ClearDictionary();
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            var logEntries = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
            while (terrainGenerator.terrainContainer.childCount > 0)
            {
                DestroyImmediate(terrainGenerator.terrainContainer.GetChild(0).gameObject);
            }
        }
    }
}

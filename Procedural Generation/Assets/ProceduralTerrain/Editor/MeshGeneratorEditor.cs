using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshDisplayer))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshDisplayer meshDisplayer= (MeshDisplayer)target;

        if (DrawDefaultInspector())
        {
            if(meshDisplayer.autoUpdate)
            {
                meshDisplayer.Generate();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            meshDisplayer.Generate();
        }
    }
}

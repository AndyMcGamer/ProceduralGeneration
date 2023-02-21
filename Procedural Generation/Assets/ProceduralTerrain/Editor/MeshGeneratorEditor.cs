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
                meshDisplayer.GenerateMesh();
            }
        }

        //base.OnInspectorGUI();
        if(GUILayout.Button("Calc Height Map"))
        {
            meshDisplayer.CalculateHeightMap();
        }

        if (GUILayout.Button("Generate"))
        {
            meshDisplayer.GenerateMesh();
        }
    }
}

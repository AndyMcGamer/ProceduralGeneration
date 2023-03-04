using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mesh Settings", menuName = "New Mesh Settings Object")]
public class MeshSettings : UpdateableData
{
    public float scale;
    public int chunkSize;
    public float worldSize
    {
        get
        {
            return scale * chunkSize;
        }
    }
}

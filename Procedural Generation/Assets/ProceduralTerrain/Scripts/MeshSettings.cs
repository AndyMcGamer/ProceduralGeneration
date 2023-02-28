using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

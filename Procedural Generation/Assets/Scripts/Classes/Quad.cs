using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;

public class Quad : IShapeObject
{
    public Edge[] edges;
    public Vector3[] vertices;
    public Quad(Triangle t1, Triangle t2)
    {
        vertices = new Vector3[4];

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] verts;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        triangles = new int[meshWidth * meshHeight * 6];
        triangleIndex = 0;
        //meshWidth++;
        //meshHeight++;
        verts = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];

    }
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = verts,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}

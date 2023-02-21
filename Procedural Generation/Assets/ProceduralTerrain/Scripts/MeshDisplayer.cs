using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDisplayer : MonoBehaviour
{
    public bool autoUpdate;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int octaves;
    [SerializeField, Range(0,1)] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float scale;
    [SerializeField] private Vector2 offset;
    [SerializeField] private int seed;

    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve meshHeightCurve;

    private float[][] heightMap;

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        if (scale <= 0) scale = 0.0001f;
        if(heightMultiplier < 1) heightMultiplier = 1;
    }

    public void GenerateMesh()
    {
        size.x = size.x <= 0 ? 1 : size.x;
        size.y = size.y <= 0 ? 1 : size.y;
        Mesh mesh = new Mesh
        {
            vertices = GenerateVertices(),
            triangles = GenerateTriangles()
        };
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
    }

    public void CalculateHeightMap()
    {
        heightMap = PerlinNoise.GenerateNoiseMap(size.x+1, size.y+1, seed, scale, octaves, persistance, lacunarity, offset);
    }

    private void GenerateUV()
    {
        Vector2[] uv = new Vector2[size.x * size.y];
        for (int y = 0, i = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                uv[i] = new Vector2(x/(float)size.x, y/(float)size.y);
            }
        }
    }

    private Vector3[] GenerateVertices()
    {
        if (heightMap == null || heightMap.Length != size.y || heightMap[0].Length != size.x) CalculateHeightMap();
        Vector3[] verts = new Vector3[(size.x+1) * (size.y+1)];
        for (int z = -Mathf.FloorToInt(size.y/2), i = 0, j = 0; z <= size.y-Mathf.FloorToInt(size.y / 2); z++)
        {
            for (int x = -Mathf.FloorToInt(size.x / 2), k = 0; x <= size.x - Mathf.FloorToInt(size.x / 2); x++)
            {
                //print(heightMap[j][k]);
                verts[i] = new Vector3(x, meshHeightCurve.Evaluate(heightMap[j][k]) * heightMultiplier, z);
                k++;
                i++;
            }
            j++;
        }

        return verts;
    }

    private int[] GenerateTriangles()
    {
        int[] triangles = new int[6 * size.x * size.y];
        for (int z = 0, vert = 0, tris = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + size.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size.x + 1;
                triangles[tris + 5] = vert + size.x + 2;

                vert++;

                tris += 6;
            }
            vert++;
        }
        return triangles;
    }
}

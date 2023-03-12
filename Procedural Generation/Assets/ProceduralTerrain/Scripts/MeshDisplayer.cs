using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DrawMode
{
    DrawMesh,
    DrawPerlin,
    DrawFalloff,
    DrawMidpoint,
    DrawAll
}

public class MeshDisplayer : MonoBehaviour
{
    public bool autoUpdate;
    [SerializeField] private DrawMode drawMode;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int chunkSize;
    [SerializeField] private int octaves;
    [SerializeField, Range(0,1)] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private float scale;
    [SerializeField] private Vector2 offset;
    [SerializeField] private int seed;

    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve meshHeightCurve;


    [SerializeField, Range(0, 6)] private int levelOfDetail = 1;

    [SerializeField] private bool useMidpoint;
    [SerializeField, Range(0,1)] private float midptInfluence;
    [SerializeField] private int midpointSeed;
    [SerializeField] private Vector2 midpointBounds;
    [SerializeField, Range(0.01f, 1.5f)] float midpointRoughness = 1f;
    private HeightMapData heightMap;
    private HeightMapData midpointMap;
    private HeightMapData falloffMap;

    [SerializeField] private bool useFalloff;
    [SerializeField] private Vector2 falloffParams = new(3f,2.2f);

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        if (scale <= 0) scale = 0.0001f;
        if(heightMultiplier < 1) heightMultiplier = 1;
    }

    public void Generate()
    {
        switch (drawMode)
        {
            case DrawMode.DrawMesh:
                //GenerateMeshData();
                GenerateMesh(false);
                GenerateTexture();
                break;
            case DrawMode.DrawPerlin:
                GenerateMesh(true);
                GenerateTexture();
                break;
            case DrawMode.DrawFalloff:
                GenerateMesh(true);
                GenerateTexture();
                break;
            case DrawMode.DrawMidpoint:
                GenerateMesh(true);
                GenerateTexture();
                break;
            case DrawMode.DrawAll:
                GenerateMeshData();
                GenerateTexture(chunkSize);
                break;
            default:
                break;
        }
    }

    private void GenerateMeshData()
    {
        CalculateHeightMap(chunkSize);

        
        int meshIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;

        int vertsPerLine = (chunkSize) / meshIncrement + 1;
        MeshData meshData = new(vertsPerLine, vertsPerLine);

        //float[][] fallOff = FalloffGenerator.GenerateFalloffMap(chunkSize, falloffParams.x, falloffParams.y);
        float[][] fallOff = FalloffGenerator.GenerateFalloffMap(chunkSize, falloffParams);

        for (int z = -Mathf.FloorToInt(chunkSize / 2), vertIndex = 0, j = 0; z <= chunkSize - Mathf.FloorToInt(chunkSize / 2); z+=meshIncrement)
        {
            for (int x = -Mathf.FloorToInt(chunkSize / 2), k = 0; x <= chunkSize - Mathf.FloorToInt(chunkSize / 2); x += meshIncrement)
            {
                float height = heightMap.heightMap[j][k];
                if (useMidpoint)
                {
                    height = Mathf.Lerp(heightMap.heightMap[j][k], midpointMap.heightMap[j][k], midptInfluence);
                }
                if (useFalloff)
                {
                    height = Mathf.Clamp(0,height - fallOff[j][k], int.MaxValue);
                    //height = Mathf.Clamp01(height - fallOff[j][k]);
                }
                //heightMap.heightMap[j][k] = meshHeightCurve.Evaluate(height) * heightMultiplier;
                meshData.verts[vertIndex] = new Vector3(x, meshHeightCurve.Evaluate(height) * heightMultiplier, z);
                meshData.uvs[vertIndex] = new Vector2(k / (chunkSize + 1f), j / (chunkSize + 1f));

                k += meshIncrement;
                vertIndex++;
            }
            j += meshIncrement;
        }
        
        for (int y = 0, vert = 0; y < vertsPerLine - 1; y++)
        {
            for (int x = 0; x < vertsPerLine - 1; x++)
            {
                meshData.AddTriangle(vert, vert + vertsPerLine + 1, vert + 1);
                meshData.AddTriangle(vert, vert + vertsPerLine, vert + vertsPerLine + 1);
                vert++;
            }
            vert++;
        }

        meshFilter.mesh = meshData.CreateMesh();
    }



    private void GenerateTexture()
    {
        Texture2D texture = TextureGenerator.GenerateTextureFromHeightmap(heightMap);
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
    }

    private void GenerateTexture(int cSize)
    {
        Texture2D texture = new(cSize, cSize)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        Color32[] colorMap = new Color32[cSize * cSize];
        for (int y = 0; y < cSize; y++)
        {
            for (int x = 0; x < cSize; x++)
            {
                colorMap[y * cSize + x] = Color32.Lerp((Color32)Color.black, (Color32)Color.white, Mathf.InverseLerp(heightMap.minHeight, heightMap.maxHeight, heightMap.heightMap[y][x]));
            }
        }
        texture.SetPixels32(colorMap);
        texture.Apply();
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);
    }

    private void GenerateMesh(bool isFlat)
    {
        size.x = size.x <= 0 ? 1 : size.x;
        size.y = size.y <= 0 ? 1 : size.y;
        Mesh mesh = new()
        {
            vertices = GenerateVertices(isFlat),
            triangles = GenerateTriangles(),
            uv = GenerateUV()
        };
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
    }

    public void CalculateHeightMap()
    {
        if(drawMode == DrawMode.DrawMidpoint)
        {
            float[][] midHeightmap = MidpointDisplacement.GenerateMidpointDisplacement(chunkSize, midpointSeed, midpointRoughness, midpointBounds.x, midpointBounds.y);
            heightMap = new()
            {
                heightMap = midHeightmap,
                minHeight = midpointBounds.x,
                maxHeight = midpointBounds.y
            };
            return;
        }
        if(drawMode == DrawMode.DrawFalloff)
        {
            //float[][] falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize, falloffParams.x, falloffParams.y);
            float[][] falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize, falloffParams);
            heightMap = new()
            {
                heightMap = falloffMap,
                minHeight = 0,
                maxHeight = 1
            };
            return;
        }
        float[][] map = PerlinNoise.GenerateNoiseMap(size.x+1, size.y+1, seed, scale, octaves, persistence, lacunarity, offset);
        float[][] hmap = new float[map.Length][];
        for (int i = 0; i < hmap.Length; i++)
        {
            hmap[i] = (float[])map[i].Clone();
        }
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                map[y][x] *= meshHeightCurve.Evaluate(map[y][x]) * heightMultiplier;

                if (map[y][x] > maxVal) { maxVal= map[y][x];}
                if (map[y][x] < minVal) { minVal= map[y][x];}
            }
        }
        heightMap = new()
        {
            heightMap = map,
            //minHeight = minVal / heightMultiplier,
            //maxHeight = maxVal / heightMultiplier
            minHeight = minVal,
            maxHeight = maxVal
        };
    }

    public void CalculateHeightMap(int cSize)
    {
        float[][] midHeightmap = MidpointDisplacement.GenerateMidpointDisplacement(chunkSize, midpointSeed, midpointRoughness, midpointBounds.x, midpointBounds.y);
        midpointMap = new()
        {
            heightMap = midHeightmap,
            minHeight = midpointBounds.x,
            maxHeight = midpointBounds.y
        };

        float[][] map = PerlinNoise.GenerateNoiseMap(cSize + 1, cSize + 1, seed, scale, octaves, persistence, lacunarity, offset);
        float[][] hmap = new float[map.Length][];
        for (int i = 0; i < hmap.Length; i++)
        {
            hmap[i] = (float[])map[i].Clone();
        }
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        for (int x = 0; x <= cSize; x++)
        {
            for (int y = 0; y <= cSize; y++)
            {
                map[y][x] *= meshHeightCurve.Evaluate(map[y][x]) * heightMultiplier;

                if (map[y][x] > maxVal) { maxVal = map[y][x]; }
                if (map[y][x] < minVal) { minVal = map[y][x]; }
            }
        }
        heightMap = new()
        {
            heightMap = hmap,
            minHeight = minVal / heightMultiplier,
            maxHeight = maxVal / heightMultiplier
        };
    }

    private Vector2[] GenerateUV()
    {
        Vector2[] uv = new Vector2[(size.x+1) * (size.y+1)];
        for (int y = 0, i = 0; y <= size.y; y++)
        {
            for (int x = 0; x <= size.x; x++)
            {
                uv[i] = new Vector2(x/(size.x+1f), y/(size.y+1f));
                i++;
            }
        }
        return uv;
    }

    private Vector3[] GenerateVertices(bool isFlat)
    {
        if (heightMap.heightMap == null || heightMap.heightMap.Length != size.y || heightMap.heightMap[0].Length != size.x) CalculateHeightMap();
        Vector3[] verts = new Vector3[(size.x+1) * (size.y+1)];
        for (int z = -Mathf.FloorToInt(size.y/2), i = 0, j = 0; z <= size.y-Mathf.FloorToInt(size.y / 2); z++)
        {
            for (int x = -Mathf.FloorToInt(size.x / 2), k = 0; x <= size.x - Mathf.FloorToInt(size.x / 2); x++)
            {
                float height = isFlat ? 0 : meshHeightCurve.Evaluate(heightMap.heightMap[j][k]) * heightMultiplier;
                verts[i] = new Vector3(x, height, z);
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

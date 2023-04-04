using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public Vector2 position;

    private GameObject meshObject;
    private Vector2 sampleCenter;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private HeightMapSettings heightmapSettings;
    private MeshSettings meshSettings;

    private HeightMapData heightMap;

    private LODMesh[] lodMeshes;

    public TerrainChunk(Vector2 position, HeightMapSettings heightmapSettings, MeshSettings meshSettings, Transform parent, Material material, LODInfo[] detailLevels)
    {
        this.position = position;
        this.heightmapSettings = heightmapSettings;
        this.meshSettings = meshSettings;
        
        meshObject = new GameObject($"Terrain Chunk {position.x}, {position.y}");
        meshObject.transform.localScale = Vector3.one * meshSettings.scale;
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        sampleCenter = position * meshSettings.chunkSize;

        Vector2 worldPos = meshSettings.worldSize * position;
        meshObject.transform.parent = parent;
        meshObject.transform.position = new Vector3(worldPos.x, 0, worldPos.y);

        meshRenderer.material = material;

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
        }
    }

    public void UpdateChunk(HeightMapSettings heightmapSettings, MeshSettings meshSettings, Material material)
    {
        this.heightmapSettings = heightmapSettings;
        this.meshSettings = meshSettings;
        sampleCenter = position * meshSettings.chunkSize;
        Vector2 worldPos = meshSettings.worldSize * position;
        meshObject.transform.position = new Vector3(worldPos.x, 0, worldPos.y);
        meshRenderer.material = material;
    }


    public void LoadChunk(float[][] falloffMap, float[][] midpointMap, int lod)
    {
        heightMap = HeightmapGenerator.GenerateHeightmap(meshSettings.chunkSize, heightmapSettings, sampleCenter, falloffMap, midpointMap);
        
        lodMeshes[lod].GetMesh(heightMap.heightMap, meshSettings);
        
        meshFilter.mesh = lodMeshes[lod].mesh;
        meshCollider.sharedMesh = lodMeshes[lod].mesh;
    }
}

public class LODMesh
{
    public Mesh mesh;
    int lod;
    public bool hasMesh;

    public LODMesh(int lod)
    {
        this.lod = lod;
        hasMesh = false;
        mesh = null;
    }

    public void GetMesh(float[][] heightmap, MeshSettings meshSettings)
    {
        
        mesh = MeshGenerator.GenerateMesh(heightmap, meshSettings, lod).CreateMesh();
        hasMesh = true;
    }
}

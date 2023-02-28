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

    private HeightMapSettings heightmapSettings;
    private MeshSettings meshSettings;

    private HeightMapData heightMap;

    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;

    public TerrainChunk(Vector2 position, HeightMapSettings heightmapSettings, MeshSettings meshSettings, Transform parent, Material material, LODInfo[] detailLevels)
    {
        this.position = position;
        this.heightmapSettings = heightmapSettings;
        this.meshSettings = meshSettings;
        
        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

        sampleCenter = position * meshSettings.chunkSize;

        Vector2 worldPos = meshSettings.worldSize * position;
        meshObject.transform.parent = parent;
        meshObject.transform.position = new Vector3(worldPos.x, 0, worldPos.y);

        meshRenderer.material = material;

        this.detailLevels = detailLevels;
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


    public void LoadChunk(ref float[][] falloffMap, ref float[][] midpointMap, int lod)
    {
        heightMap = HeightmapGenerator.GenerateHeightmap(meshSettings.chunkSize, heightmapSettings, sampleCenter, ref falloffMap, ref midpointMap);
        if (!lodMeshes[lod].hasMesh)
        {
            lodMeshes[lod].GetMesh(ref heightMap.heightMap, meshSettings);
        }
        meshFilter.mesh = lodMeshes[lod].mesh;
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
    }

    public void GetMesh(ref float[][] heightmap, MeshSettings meshSettings)
    {
        mesh = MeshGenerator.GenerateMesh(ref heightmap, meshSettings, lod).CreateMesh();
        hasMesh = true;
    }
}

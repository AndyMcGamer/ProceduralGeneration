using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int mapSize;
    public HeightMapSettings heightmapSettings;
    public MeshSettings meshSettings;

    public Material terrainMaterial;

    public Transform terrainContainer;
    Dictionary<Vector2, TerrainChunk> chunkDictionary = new();

    private float[][] falloffMap;
    private float[][] midpointMap;

    public LODInfo[] detailLevels;
    public int lod;

    public void ClearDictionary()
    {
        chunkDictionary.Clear();
    }

    public void Generate()
    {
        if (heightmapSettings.useFalloff)
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.falloffBounds.x, heightmapSettings.falloffBounds.y);
        }
        else
        {
            falloffMap = null;
        }
        if (heightmapSettings.useMidpoint)
        {
            midpointMap = MidpointDisplacement.GenerateMidpointDisplacement((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.midpointSettings);
        }
        else
        {
            midpointMap = null;
        }
        for (int y = -mapSize/2; y < mapSize - mapSize /2; y++)
        {
            for (int x = -mapSize/2; x < mapSize - mapSize/2; x++)
            {
                Vector2 coord = new(x, y);
                if (chunkDictionary.ContainsKey(coord))
                {
                    chunkDictionary[coord].UpdateChunk(heightmapSettings, meshSettings, terrainMaterial);
                }
                else
                {
                    TerrainChunk chunk = new(coord, heightmapSettings, meshSettings, terrainContainer, terrainMaterial, detailLevels);
                    chunkDictionary.Add(coord, chunk);
                    chunk.LoadChunk(ref falloffMap, ref midpointMap, lod);
                }
            }
        }
    }
}

[System.Serializable]
public class LODInfo
{
    public int lod;
}

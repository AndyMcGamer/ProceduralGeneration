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

    public void Generate(bool recalculate)
    {
        if (recalculate)
        {
            if (heightmapSettings.useFalloff)
            {
                switch (heightmapSettings.falloffSettings.falloffMode)
                {
                    case FalloffMode.Values:
                        falloffMap = FalloffGenerator.GenerateFalloffMap((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.falloffSettings.falloffValues.x, heightmapSettings.falloffSettings.falloffValues.y, meshSettings.scale);
                        break;
                    case FalloffMode.Bounds:
                        falloffMap = FalloffGenerator.GenerateFalloffMap((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.falloffSettings.falloffBounds);
                        break;
                    case FalloffMode.Curve:
                        falloffMap = FalloffGenerator.GenerateFalloffMap((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.falloffSettings.falloffCurve);
                        break;
                    default:
                        break;
                }

            }
            else
            {
                falloffMap = null;
            }
            if (heightmapSettings.useMidpoint)
            {
                midpointMap = MidpointDisplacement.GenerateMidpointDisplacement((meshSettings.chunkSize + 1) * mapSize, heightmapSettings.midpointSettings, mapSize);
            }
            else
            {
                midpointMap = null;
            }
        }
        for (int y = -mapSize/2; y < mapSize - mapSize /2; y++)
        {
            for (int x = -mapSize/2; x < mapSize - mapSize/2; x++)
            {
                Vector2 coord = new(x, y);
                if (chunkDictionary.ContainsKey(coord))
                {
                    chunkDictionary[coord].UpdateChunk(heightmapSettings, meshSettings, terrainMaterial);
                    chunkDictionary[coord].LoadChunk(falloffMap, midpointMap, lod);
                }
                else
                {
                    TerrainChunk chunk = new(coord, heightmapSettings, meshSettings, terrainContainer, terrainMaterial, detailLevels);
                    chunkDictionary.Add(coord, chunk);
                    chunk.LoadChunk(falloffMap, midpointMap, lod);
                }
            }
        }
    }
}

[System.Serializable]
public struct LODInfo
{
    public int lod;
}

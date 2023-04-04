using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightmapGenerator 
{
    public static HeightMapData GenerateHeightmap(int chunkSize, HeightMapSettings settings, Vector2 sampleCenter, float[][] falloffMap, float[][] midpointMap)
    {
        float[][] heightmap = PerlinNoise.GenerateNoiseMap(chunkSize+1, chunkSize+1, settings.perlinSettings, sampleCenter);
        Vector2 position = sampleCenter / chunkSize;

        AnimationCurve curve = new(settings.heightCurve.keys);

        for (int y = 0; y <= chunkSize; y++)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                
                if (settings.useMidpoint)
                {
                    int mapSize = midpointMap.Length / (chunkSize+1);
                    int offset = mapSize / 2;
                    int xCoord = ((int)(position.x) + offset);
                    int yCoord = ((int)(position.y) + offset);
                    xCoord *= chunkSize;
                    yCoord *= chunkSize;
                    heightmap[y][x] = Mathf.Lerp(heightmap[y][x], midpointMap[yCoord + y][xCoord + x], settings.midpointInfluence);
                    //heightmap[y][x] = heightmap[y][x] + midpointMap[yCoord + y][xCoord + x];
                }
                if (settings.useFalloff)
                {
                    int mapSize = falloffMap.Length / (chunkSize+1);
                    int offset = mapSize / 2;
                    int xCoord = ((int)(position.x) + offset);
                    int yCoord = ((int)(position.y) + offset);
                    xCoord *= chunkSize;
                    yCoord *= chunkSize;
                    heightmap[y][x] = Mathf.Clamp(heightmap[y][x] - falloffMap[yCoord + y][xCoord + x], 0, int.MaxValue);
                    //heightmap[y][x] = Mathf.Clamp01(heightmap[y][x] - falloffMap[yCoord + y][xCoord + x]);
                }
                heightmap[y][x] = curve.Evaluate(heightmap[y][x]) * settings.heightScale;
                //heightmap[y][x] *= settings.heightScale;
            }
        }
        
        return new HeightMapData()
        {
            heightMap = heightmap,
        };
    }
}

public struct HeightMapData
{
    public float[][] heightMap;
    public float minHeight;
    public float maxHeight;
}
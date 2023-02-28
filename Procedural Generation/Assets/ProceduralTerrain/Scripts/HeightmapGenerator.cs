using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightmapGenerator 
{
    public static HeightMapData GenerateHeightmap(int chunkSize, HeightMapSettings settings, Vector2 sampleCenter, ref float[][] falloffMap, ref float[][] midpointMap)
    {
        float[][] heightmap = PerlinNoise.GenerateNoiseMap(chunkSize, chunkSize, settings.perlinSettings, sampleCenter);
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        Vector2 position = sampleCenter / chunkSize;

        for (int y = 0; y <= chunkSize; y++)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                if (settings.useMidpoint)
                {
                    heightmap[y][x] = Mathf.Lerp(heightmap[y][x], midpointMap[(int)position.y + y][(int)position.x + x], settings.midpointInfluence);
                }
                if (settings.useFalloff)
                {
                    heightmap[y][x] = Mathf.Clamp(heightmap[y][x] - falloffMap[(int)position.y + y][(int)position.x + x], 0, int.MaxValue);
                }
                heightmap[y][x] *= settings.heightCurve.Evaluate(heightmap[y][x]) * settings.heightScale;

                if(heightmap[y][x] > maxVal)
                {
                    maxVal = heightmap[y][x];
                }
                if(heightmap[y][x] < minVal)
                {
                    minVal = heightmap[y][x];
                }
            }
        }

        return new HeightMapData()
        {
            heightMap = heightmap,
            maxHeight = maxVal,
            minHeight = minVal
        };
    }
}

public struct HeightMapData
{
    public float[][] heightMap;
    public float minHeight;
    public float maxHeight;
}
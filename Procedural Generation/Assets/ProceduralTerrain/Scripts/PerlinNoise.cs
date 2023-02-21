using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise 
{
    public static float[][] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        System.Random r = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(r.Next(-100000, 1000000) + offset.x, r.Next(-100000, 1000000) + offset.y);
        }

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        float[][] noiseMap = new float[mapHeight][];

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int i = 0; i < mapHeight; i++)
        {
            noiseMap[i] = new float[mapWidth];
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amp = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * freq + octaveOffsets[i].x * freq;
                    float sampleY = (y - halfHeight) / scale * freq + octaveOffsets[i].y * freq;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amp;

                    amp *= persistance;
                    freq *= lacunarity;

                }

                if(noiseHeight > maxHeight)
                {
                    maxHeight = noiseHeight;
                }else if (noiseHeight < minHeight)
                {
                    minHeight = noiseHeight;
                }
                
                noiseMap[y][x] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[y][x] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[y][x]);
            }
        }

        return noiseMap;
    }
}

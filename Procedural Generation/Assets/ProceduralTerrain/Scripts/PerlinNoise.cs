using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public enum NormalizeMode
{
    Local,
    Global
}
public static class PerlinNoise 
{
    public static float[][] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        System.Random r = new(seed);
        float amp = 1, freq;
        float maxPossibleHeight = 0;
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(r.Next(-100000, 1000000) + offset.x, r.Next(-100000, 1000000) + offset.y);
            maxPossibleHeight += amp;
            amp *= persistence;
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
                amp = 1;
                freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * freq + octaveOffsets[i].x * freq;
                    float sampleY = (y - halfHeight) / scale * freq + octaveOffsets[i].y * freq;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amp;

                    amp *= persistence;
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

                float normalizedHeight = (noiseMap[y][x]+1) / (maxPossibleHeight/1.1f);
                noiseMap[y][x] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //noiseMap[y][x] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[y][x]);
                Debug.Log(noiseMap[y][x]);
            }
        }

        return noiseMap;
    }

    public static float[][] GenerateNoiseMap(int mapWidth, int mapHeight, PerlinSettings settings, Vector2 sampleCenter)
    {
        System.Random r = new(settings.seed);
        float amp = 1;
        float maxPossibleHeight = 0;
        //Vector2[] octaveOffsets = new Vector2[settings.octaves];
        NativeArray<Vector2> nativeOffsets = new NativeArray<Vector2>(settings.octaves, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < settings.octaves; i++)
        {
            //octaveOffsets[i] = new Vector2(r.Next(-100000, 1000000) + sampleCenter.x + settings.offset.x, r.Next(-100000, 1000000) + sampleCenter.y + settings.offset.y);
            nativeOffsets[i] = new Vector2(r.Next(-100000, 1000000) + sampleCenter.x + settings.offset.x, r.Next(-100000, 1000000) + sampleCenter.y + settings.offset.y);
            maxPossibleHeight += amp;
            amp *= settings.persistence;
        }

        float[][] noiseMap = new float[mapHeight][];
        for (int i = 0; i < mapHeight; i++)
        {
            noiseMap[i] = new float[mapWidth];
        }

        NativeArray<float> nativeNoisemap = new NativeArray<float>(mapHeight * mapWidth, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        PerlinJob perlinJob = new PerlinJob() 
        {
            mapWidth = mapWidth,
            mapHeight = mapHeight,
            octaveOffsets = nativeOffsets,
            octaves = settings.octaves,
            scale = settings.scale,
            maxPossibleHeight = maxPossibleHeight,
            maxHeight = float.MinValue,
            minHeight = float.MaxValue,
            persistence = settings.persistence,
            lacunarity = settings.lacunarity,
            noiseMap = nativeNoisemap,
            global = settings.mode == NormalizeMode.Global,
        };

        var dependency = new JobHandle();
        var scheduleDependency = perlinJob.Schedule(mapWidth * mapHeight, dependency);
        var jobHandle = perlinJob.ScheduleParallel(mapWidth * mapHeight, 1, scheduleDependency);

        jobHandle.Complete();

        for (int i = 0; i < perlinJob.noiseMap.Length; i++)
        {
            int y = i / mapWidth;
            int x = i % mapHeight;
            noiseMap[y][x] = perlinJob.noiseMap[i];
        }

        nativeOffsets.Dispose();
        nativeNoisemap.Dispose();

        float minHeight = perlinJob.minHeight;
        float maxHeight = perlinJob.maxHeight;

        //float minHeight = float.MaxValue;
        //float maxHeight = float.MinValue;

        //float halfWidth = mapWidth / 2f;
        //float halfHeight = mapHeight / 2f;



        //for (int y = 0; y < mapHeight; y++)
        //{
        //    for (int x = 0; x < mapWidth; x++)
        //    {
        //        amp = 1;
        //        float freq = 1;
        //        float noiseHeight = 0;

        //        for (int i = 0; i < settings.octaves; i++)
        //        {
        //            float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * freq;
        //            float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * freq;

        //            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
        //            noiseHeight += perlinValue * amp;

        //            amp *= settings.persistence;
        //            freq *= settings.lacunarity;

        //        }

        //        if (noiseHeight > maxHeight)
        //        {
        //            maxHeight = noiseHeight;
        //        }
        //        else if (noiseHeight < minHeight)
        //        {
        //            minHeight = noiseHeight;
        //        }

        //        noiseMap[y][x] = noiseHeight;

        //        if(settings.mode == NormalizeMode.Global)
        //        {
        //            float normalizedHeight = (noiseMap[y][x]+1) / (maxPossibleHeight / 1.2f);
        //            noiseMap[y][x] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
        //        }

        //    }
        //}





        if (settings.mode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[y][x] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[y][x]);
                }
            }
        }

        return noiseMap;
    }
}



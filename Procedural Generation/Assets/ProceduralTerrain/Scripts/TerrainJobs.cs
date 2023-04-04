using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile(DisableSafetyChecks = true, CompileSynchronously = false)]
public struct PerlinJob : IJobFor
{
    public NativeArray<float> noiseMap;
    [ReadOnly] public float maxPossibleHeight;
    [ReadOnly] public NativeArray<Vector2> octaveOffsets;
    public float minHeight;
    public float maxHeight;
    [ReadOnly] public int mapWidth;
    [ReadOnly] public int mapHeight;
    [ReadOnly] public float persistence;
    [ReadOnly] public float lacunarity;
    [ReadOnly] public int octaves;
    [ReadOnly] public float scale;
    [ReadOnly] public bool global;
    
    public void Execute(int i)
    {
        float amp = 1f, freq = 1f;
        float noiseHeight = 0f;
        int x = i % mapHeight;
        int y = i / mapWidth;
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        for (int j = 0; j < octaves; j++)
        {
            float sampleX = (x - halfWidth + octaveOffsets[j].x) / scale * freq;
            float sampleY = (y - halfHeight + octaveOffsets[j].y) / scale * freq;

            float perlinValue = noise.snoise(new float2(sampleX, sampleY));
            //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
            noiseHeight += perlinValue * amp;

            amp *= persistence;
            freq *= lacunarity;
        }

        if (noiseHeight > maxHeight)
        {
            maxHeight = noiseHeight;
        }
        else if (noiseHeight < minHeight)
        {
            minHeight = noiseHeight;
        }

        noiseMap[i] = noiseHeight;

        if (global)
        {
            float normalizedHeight = (noiseMap[i]+1)/(maxPossibleHeight);
            noiseMap[i] = normalizedHeight;
            //noiseMap[i] = math.clamp(normalizedHeight, 0, int.MaxValue);
        }
    }
}

[BurstCompile]
public struct MeshVerticesJob : IJobFor
{
    [ReadOnly] public NativeArray<float> heightmap;
    public int meshIncrement;
    public int chunkSize;
    public NativeArray<Vector3> verts;
    public NativeArray<Vector2> uvs;
    public void Execute(int i)
    {
        if (i % meshIncrement != 0) return;
        int shiftedIndex = i - ( ( ((chunkSize+1) * (chunkSize+1)) - 1)/chunkSize * (chunkSize/2) );
        int z = (i / (chunkSize + 1)) - ((chunkSize+1)/2);
        int x = shiftedIndex - z * (chunkSize + 1);
        int j = i / (chunkSize + 1);
        int k = i % (chunkSize + 1);
        int vertIdx = i / meshIncrement;
        verts[vertIdx] = new Vector3(x, heightmap[j * (chunkSize+1) + k], z);
        uvs[vertIdx] = new Vector2(k / (chunkSize + 1f), j / (chunkSize + 1f));

    }
}

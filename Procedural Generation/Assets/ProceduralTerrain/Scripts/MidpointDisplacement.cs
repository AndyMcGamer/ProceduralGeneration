using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class MidpointDisplacement
{
    public static float[][] GenerateMidpointDisplacement(int chunksize, int seed, float roughness, float minHeight = 0f, float maxHeight = 1f)
    {
        int rectSize = NextPowerOfTwo(chunksize);
        float[][] heightmap = new float[rectSize+1][];
        for (int i = 0; i <= rectSize; i++)
        {
            heightmap[i] = new float[rectSize + 1];
        }
        System.Random r = new(seed);
        Displace(ref heightmap, chunksize, roughness, r);
        
        Normalize(ref heightmap, minHeight, maxHeight);
        
        return heightmap;
    }

    public static float[][] GenerateMidpointDisplacement(int chunksize, MidpointSettings settings, int mapSize)
    {
        int rectSize = NextPowerOfTwo(chunksize);
        float[][] heightmap = new float[rectSize + 1][];
        for (int i = 0; i <= rectSize; i++)
        {
            heightmap[i] = new float[rectSize + 1];
        }
        System.Random r = new(settings.seed);
        Displace(ref heightmap, chunksize, settings.roughness, r);
        
        Normalize(ref heightmap, settings.heightBounds.x, settings.heightBounds.y, mapSize);
        
        return heightmap;
    }


    private static void Displace(ref float[][] heightmap, int chunksize, float roughness, System.Random r)
    {
        int rectSize = NextPowerOfTwo(chunksize) + 1;
        float curHeight = rectSize / 2f;
        float heightReduction = Mathf.Pow(-roughness, 2f);

        while(rectSize > 0)
        {
            DiamondStep(ref heightmap, rectSize, curHeight, r);
            SquareStep(ref heightmap, rectSize, curHeight, r);
            rectSize /= 2;
            curHeight *= heightReduction;
        }
    }

    private static void DiamondStep(ref float[][] heightmap, int rectSize, float curHeight, System.Random r)
    {
        int halfSize = rectSize / 2;
        int chunkSize = heightmap.Length-1;
        for (int y = 0; y <= chunkSize; y+=rectSize)
        {
            for (int x = 0; x <= chunkSize; x+=rectSize)
            {
                int next_x = (x + rectSize) % chunkSize;
                int next_y = (y + rectSize) % chunkSize;
                if (next_x < x)
                {
                    next_x = chunkSize;
                }
                if (next_y < y)
                {
                    next_y= chunkSize;
                }

                float topLeft = heightmap[y][x];
                float topRight = heightmap[y][next_x];
                float botLeft = heightmap[next_y][x];
                float botRight = heightmap[next_y][next_x];

                int midx = (x + halfSize) % chunkSize;
                int midy = (y+ halfSize) % chunkSize;

                float randVal = ((float)r.NextDouble() * 2 - 1) * curHeight;
                float midpoint = (topLeft + topRight + botLeft + botRight) / 4f;
                heightmap[midy][midx] = midpoint + randVal;
            }
        }
    }

    private static void SquareStep(ref float[][] heightmap, int rectSize, float curHeight, System.Random r)
    {
        int halfSize = rectSize / 2;
        int chunkSize = heightmap.Length - 1;
        for (int y = 0; y <= chunkSize; y += rectSize)
        {
            for (int x = 0; x <= chunkSize; x += rectSize)
            {
                int next_x = (x + rectSize) % chunkSize;
                int next_y = (y + rectSize) % chunkSize;
                if (next_x < x)
                {
                    next_x = chunkSize;
                }
                if (next_y < y)
                {
                    next_y = chunkSize;
                }

                int midX = (x+halfSize) % chunkSize;
                int midY = (y+halfSize) % chunkSize;

                int prevMidX = (x-halfSize+chunkSize) % chunkSize;
                int prevMidY = (y-halfSize+chunkSize) % chunkSize;

                float curTopL = heightmap[y][x];
                float curTopR = heightmap[y][next_x];
                float curCenter = heightmap[midY][midX];
                float curBotL = heightmap[next_y][x];   

                float prevYCenter = heightmap[prevMidY][midX];
                float prevXCenter = heightmap[midY][prevMidX];

                float curLeftMid = (curTopL + curCenter + curBotL + prevXCenter) / 4f + ((float)r.NextDouble() * 2 - 1) * curHeight;
                float curTopMid = (curTopL + curCenter + curTopR + prevYCenter) / 4f + ((float)r.NextDouble() * 2 - 1) * curHeight;

                heightmap[y][midX] = curTopMid;
                heightmap[midY][x] = curLeftMid;

            }
        }
    }

    private static int NextPowerOfTwo(int n)
    {
        int res = 1;
        if (n == 1)
        {
            return 2;
        }
        while(res < n)
        {
            res *= 2;
        }
        return res;
    }

    private static void Normalize(ref float[][] map, float minHeight, float maxHeight, int mapSize = 1)
    {
        float min = float.MaxValue;
        float max = float.MinValue;
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] < min)
                {
                    min = map[i][j];
                }
                else if (map[i][j] > max)
                {
                    max = map[i][j];
                }
            }
        }

        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                map[i][j] = Mathf.InverseLerp(min, max, map[i][j]);
            }
        }
    }
}

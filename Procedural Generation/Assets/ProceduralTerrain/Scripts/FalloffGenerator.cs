using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[][] GenerateFalloffMap(int chunkSize, float a, float b)
    {
        float[][] map = new float[chunkSize+1][];
        for (int i = 0; i <= chunkSize; i++)
        {
            map[i] = new float[chunkSize + 1];
        }
        for (int i = 0; i <= chunkSize; i++)
        {
            for (int j = 0; j <= chunkSize; j++)
            {
                float val = Mathf.Max(Mathf.Abs(i / (float)chunkSize * 2f - 1f), Mathf.Abs(j / (float)chunkSize * 2f - 1f));
                map[i][j] = Evaluate(val,a,b);
            }
        }
        return map;
    }

    private static float Evaluate(float value, float a, float b)
    {
        return Mathf.Pow(value,a)/(Mathf.Pow(value,a) + Mathf.Pow(b-b*value,a));
    }
}

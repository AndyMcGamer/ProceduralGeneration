using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[][] GenerateFalloffMap(int chunkSize, float a, float b, float scale = 1f)
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
                map[i][j] = Evaluate(val,a,b, scale);
            }
        }
        return map;
    }

    public static float[][] GenerateFalloffMap(int chunkSize, AnimationCurve curve)
    {
        float[][] map = new float[chunkSize + 1][];
        for (int i = 0; i <= chunkSize; i++)
        {
            map[i] = new float[chunkSize + 1];
        }
        for (int i = 0; i <= chunkSize; i++)
        {
            for (int j = 0; j <= chunkSize; j++)
            {
                float val = Mathf.Max(Mathf.Abs(i / (float)chunkSize) * 2 - 1, Mathf.Abs(j / (float)chunkSize) * 2 - 1);
                map[i][j] = curve.Evaluate(val);
            }
        }
        return map;
    }

    public static float[][] GenerateFalloffMap(int chunkSize, Vector2 falloffBounds)
    {

        float[][] map = new float[chunkSize + 1][];
        for (int i = 0; i <= chunkSize; i++)
        {
            map[i] = new float[chunkSize + 1];
        }
        for (int i = 0; i <= chunkSize; i++)
        {
            for (int j = 0; j <= chunkSize; j++)
            {

                Vector2 pos = new(
                    (float)j / (chunkSize + 1) * 2 - 1,
                    (float)i / (chunkSize + 1) * 2 - 1
                    );
                float val = Mathf.Max(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
                if(val < falloffBounds.x)
                {
                    map[i][j] = 0;
                }
                else if(val > falloffBounds.y)
                {
                    map[i][j] = 1;
                }
                else
                {
                    map[i][j] = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(falloffBounds.x, falloffBounds.y, val));
                }
            }
        }
        return map;
    }

    private static float Evaluate(float value, float a, float b, float scale = 1f)
    {
        return Mathf.Pow(value,a)/(Mathf.Pow(value,a) + Mathf.Pow(b-b*value,a));
    }
}

public enum FalloffMode
{
    Values,
    Bounds,
    Curve
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class TextureGenerator
{
    public static Texture2D GenerateTextureFromHeightmap(HeightMapData heightmap)
    {
        int height = heightmap.heightMap.Length;
        int width = heightmap.heightMap[0].Length;

        Texture2D texture = new(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        Color32[] colorMap = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color32.Lerp((Color32)Color.black, (Color32)Color.white, Mathf.InverseLerp(heightmap.minHeight, heightmap.maxHeight, heightmap.heightMap[y][x]));
            }
        }
        texture.SetPixels32(colorMap);
        texture.Apply();
        return texture;
    }
}

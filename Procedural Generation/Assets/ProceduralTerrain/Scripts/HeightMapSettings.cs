using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heightmap Settings", menuName = "New Heightmap Settings Object")]
public class HeightMapSettings : UpdateableData
{
    public bool useFalloff;
    public bool useMidpoint;

    public AnimationCurve heightCurve;
    public float heightScale;

    public PerlinSettings perlinSettings;

    public MidpointSettings midpointSettings;
    [Range(0f, 1f)]
    public float midpointInfluence;

    public Vector2 falloffBounds;

    public float MinHeight
    {
        get
        {
            return heightScale * heightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return heightScale * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        perlinSettings.ValidateValues();
        midpointSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}

[System.Serializable]
public class PerlinSettings
{
    public int seed;
    public int octaves = 5;
    [Range(0f, 1f)] public float persistence = 0.5f;
    public float lacunarity = 2;
    public float scale = 65;
    public Vector2 offset;
    public NormalizeMode mode;
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.001f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }


}

[System.Serializable]
public class MidpointSettings
{
    public int seed;
    [Range(0,1f)]
    public float roughness;
    public Vector2 heightBounds;

    public void ValidateValues()
    {
        roughness = Mathf.Max(roughness, 0.001f);
    }
}
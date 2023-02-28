using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(ref float[][] heightmap, MeshSettings meshSettings, int levelOfDetail)
    {
        int meshIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int vertsPerLine = (meshSettings.chunkSize) / meshIncrement + 1;

        MeshData meshData = new(vertsPerLine, vertsPerLine);

        for (int z = -Mathf.FloorToInt(meshSettings.chunkSize / 2), vertIndex = 0, j = 0; z <= meshSettings.chunkSize - Mathf.FloorToInt(meshSettings.chunkSize / 2); z += meshIncrement)
        {
            for (int x = -Mathf.FloorToInt(meshSettings.chunkSize / 2), k = 0; x <= meshSettings.chunkSize - Mathf.FloorToInt(meshSettings.chunkSize / 2); x += meshIncrement)
            {
                meshData.verts[vertIndex] = new Vector3(x, heightmap[j][k], z);
                meshData.uvs[vertIndex] = new Vector2(k / (meshSettings.chunkSize + 1f), j / (meshSettings.chunkSize + 1f));

                k += meshIncrement;
                vertIndex++;
            }
            j += meshIncrement;
        }

        for (int y = 0, vert = 0; y < vertsPerLine - 1; y++)
        {
            for (int x = 0; x < vertsPerLine - 1; x++)
            {
                meshData.AddTriangle(vert, vert + vertsPerLine + 1, vert + 1);
                meshData.AddTriangle(vert, vert + vertsPerLine, vert + vertsPerLine + 1);
                vert++;
            }
            vert++;
        }

        return meshData;
    }
}

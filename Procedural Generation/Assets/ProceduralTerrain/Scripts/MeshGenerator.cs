using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[][] heightmap, MeshSettings meshSettings, int levelOfDetail)
    {
        int meshIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int vertsPerLine = (meshSettings.chunkSize) / meshIncrement + 1;

        MeshData meshData = new(vertsPerLine, vertsPerLine);
        //var nativeHmap = new NativeArray<float>(heightmap.Length * heightmap.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        //var nativeVerts = new NativeArray<Vector3>(meshData.verts.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        //var nativeUvs = new NativeArray<Vector2>(meshData.uvs.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        //for (int i = 0; i < heightmap.Length; i++)
        //{
        //    for (int j = 0; j < heightmap.Length; j++)
        //    {
        //        nativeHmap[i * heightmap.Length + j] = heightmap[i][j];
        //    }
        //}


        //MeshVerticesJob meshVerticesJob = new MeshVerticesJob()
        //{
        //    meshIncrement = meshIncrement,
        //    chunkSize = meshSettings.chunkSize,
        //    heightmap = nativeHmap,
        //    verts = nativeVerts,
        //    uvs = nativeUvs,
        //};

        //var scheduleDependency = meshVerticesJob.Schedule((meshSettings.chunkSize + 1) * (meshSettings.chunkSize + 1), new JobHandle());

        //var jobHandle = meshVerticesJob.ScheduleParallel((meshSettings.chunkSize + 1) * (meshSettings.chunkSize + 1), 32, scheduleDependency);

        ////var jobHandle = meshVerticesJob.Schedule((meshSettings.chunkSize + 1) * (meshSettings.chunkSize + 1), 64);

        //jobHandle.Complete();

        //meshVerticesJob.verts.CopyTo(meshData.verts);
        //meshVerticesJob.uvs.CopyTo(meshData.uvs);
        //nativeHmap.Dispose();
        //nativeVerts.Dispose();
        //nativeUvs.Dispose();

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

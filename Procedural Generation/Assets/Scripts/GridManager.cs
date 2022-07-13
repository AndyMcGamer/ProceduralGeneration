using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField] private int numOfSides = 3;
    [SerializeField] private int radius = 1;

    private List<Vector3> vertices;

    private void OnDrawGizmos()
    {
        if (vertices.Count < 1) return;
        foreach (var pos in vertices)
        {
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }


    // Generate Vertices In Shape

    public void DrawVertices()
    {
        vertices = new();
        float sideLength;
        Vector3 offset;
        Vector3 currentPos;
        float origAngle;

        vertices.Add(Vector3.zero); // center (0,0)

        if (numOfSides == 6) // For hexagon
        {
            sideLength = 1;
            origAngle = -Mathf.PI / 6f;
            
            for (int i = 0; i <= radius; i++)
            {
                currentPos = transform.position;
                offset = new Vector3(0, 0, i);
                float angle = origAngle;
                for (int j = 0; j < 6 * i; j++)
                {
                    currentPos += offset;
                    vertices.Add(currentPos);
                    if( i > 0 && j % i == 0)
                    {
                        if (j > 0) angle += 2 * origAngle;
                        offset = sideLength * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                    }
                }
            }

        }
        else
        {
            origAngle = -Mathf.PI / numOfSides;
            sideLength = 2f * Mathf.Sin(Mathf.PI / numOfSides);
            for (int i = 0; i <= radius; i++)
            {
                
                currentPos = transform.position;
                offset = new Vector3(0, 0, i);
                float angle = origAngle;
                for (int j = 0; j < numOfSides * i; j++)
                {

                    currentPos += offset;
                    vertices.Add(currentPos);
                    if (i > 0 && j % i == 0)
                    {
                        if (j > 0) angle += 2 * origAngle;
                        offset = sideLength * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                    }
                }
            }
        }

    }

    // Delaunay Triangulation or something (make triangles)

    // Combine some number of triangles into quadrilaterals

    // Subdivide quads and triangles
        // Make center vertex, draw segments from midpoints of edges to center

    // Skew vertices somehow (figure this one out or ask Oskar)


    public void Clear()
    {
        vertices.Clear();
    }
}

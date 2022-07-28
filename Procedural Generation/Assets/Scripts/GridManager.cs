using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField] private int numOfSides = 3;
    [SerializeField] private int radius = 1;

    private float sideLength;

    private List<Vector3> vertices;
    private List<Triangle> triangulation;

    private void OnDrawGizmos()
    {
        if (vertices == null) return;
        if (vertices.Count < 1) return;
        Gizmos.color = Color.black;
        foreach (var pos in vertices)
        {
            Gizmos.DrawSphere(pos, 0.05f);
            
        }
        if (triangulation == null) return;
        foreach (Triangle triangle in triangulation)
        {
            foreach (Edge edge in triangle.edges)
            {
                Gizmos.DrawLine(edge.vertices[0], edge.vertices[1]);
            }
        }
    }


    // Generate Vertices In Shape

    public void DrawVertices()
    {
        vertices = new();
        Vector3 offset;
        Vector3 currentPos;
        sideLength = 0;
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

    // Delaunay Triangulation with Bowyer-Watson Algorithm

    public void Triangulate()
    {
        triangulation = new();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Triangle superTriangle = CalculateSuperTriangle();
        triangulation.Add(superTriangle);
        foreach (Vector3 point in vertices)
        {
            List<Triangle> badTriangles = new();
            foreach (Triangle triangle in triangulation)
            {
                if (triangle.InCircumcenter(point))
                {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> polygonHole = new();
            foreach (Triangle badTriangle in badTriangles)
            {
                foreach (Edge edge in badTriangle.edges)
                {
                    bool sharedEdge = false;
                    foreach (Triangle other in badTriangles)
                    {
                        if (badTriangle.vertices == other.vertices) continue;
                        if (other.HasEdge(edge)) sharedEdge = true;
                    }
                    if (!sharedEdge) polygonHole.Add(edge);
                }
            }
            foreach (Triangle badTriangle in badTriangles)
            {
                triangulation.Remove(badTriangle);
            }
            foreach (Edge edge in polygonHole)
            {
                triangulation.Add(new Triangle(edge.vertices[0], edge.vertices[1], point));
            }
        }

        foreach (Triangle triangle in triangulation.ToArray())
        {
            if (superTriangle.SharesPoint(triangle)) triangulation.Remove(triangle);
        }
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    private Triangle CalculateSuperTriangle()
    {
        int outerRing = radius * numOfSides;

        Vector3[] verts = new Vector3[3];

        verts[0] = new Vector3(0, 0, outerRing);
        verts[1] = new Vector3(outerRing, 0, 0);
        verts[2] = new Vector3(-outerRing, 0, -outerRing);


        return new Triangle(verts);
        
    }


    // Combine some number of triangles into quadrilaterals

    // Subdivide quads and triangles
        // Make center vertex, draw segments from midpoints of edges to center

    // Skew vertices somehow (figure this one out or ask Oskar)


    public void Clear()
    {
        if(vertices != null) vertices.Clear();
        if(triangulation != null) triangulation.Clear();
    }
}

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

    private HashSet<Vector3> vertices;
    private List<Triangle> triangulation;
    private HashSet<IShapeObject> shapes;
    private HashSet<Quad> subdivision;

    private int drawMode = 0;

    private void OnDrawGizmos()
    {
        if (vertices == null) return;
        if (vertices.Count < 1) return;
        Gizmos.color = Color.black;
        foreach (var pos in vertices)
        {
            Gizmos.DrawSphere(pos, 0.05f);

        }
        switch (drawMode)
        {
            case 1:
                if (triangulation == null) break;
                foreach (Triangle triangle in triangulation)
                {
                    foreach (Edge edge in triangle.edges)
                    {
                        Gizmos.DrawLine(edge.vertices[0], edge.vertices[1]);
                    }
                }
                break;
            case 2:
                if (shapes == null) break;
                foreach (IShapeObject shape in shapes)
                {
                    foreach (Edge edge in shape.GetEdges())
                    {
                        Gizmos.DrawLine(edge.vertices[0], edge.vertices[1]);
                    }
                }
                break;
            case 3:
                if (subdivision == null) break;
                foreach (Quad quad in subdivision)
                {
                    foreach (Edge edge in quad.GetEdges())
                    {
                        Gizmos.DrawLine(edge.vertices[0], edge.vertices[1]);
                    }
                }
                break;
            default:
                break;
        }
        
        
        
    }

    // Generate Vertices In Shape

    public void DrawVertices()
    {
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();
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
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    // Delaunay Triangulation with Bowyer-Watson Algorithm

    public void Triangulate()
    {
        drawMode = 1;

        System.Diagnostics.Stopwatch sw = new();
        sw.Start();
        triangulation = new();
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
            if (superTriangle.SharesPoint(triangle))
            {
                triangulation.Remove(triangle);
            }
        }
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    // Create a super triangle for Delaunay Triangulation
    private Triangle CalculateSuperTriangle()
    {
        int outerRing = radius * numOfSides;

        Vector3[] verts = new Vector3[3];

        verts[0] = new Vector3(0, 0, outerRing);
        verts[1] = new Vector3(outerRing, 0, 0);
        verts[2] = new Vector3(-outerRing, 0, -outerRing);


        return new Triangle(verts);
        
    }

    // Create Adjacency List for Each Triangle (Brute Force Method)
    // The prediction is that this will be very very slow - turns out it's not as slow as I initially thought
    private void CreateAdjacencyListsSlow()
    {
        foreach (Triangle triangle in triangulation)
        {
            if (triangle.adjacencyList.Count == 3) continue;
            foreach (Triangle other in triangulation)
            {
                if (other == triangle) continue;
                if(Triangle.GetSharedEdge(triangle, other).vertices != null && !triangle.adjacencyList.Contains(other))
                {
                    triangle.adjacencyList.Add(other);
                    other.adjacencyList.Add(triangle);
                }

            }
        }
    }

    // Combine some number of triangles into quadrilaterals
    // Inventing new words lol "Quadrilaterate"
    public void Quadrilaterate()
    {
        drawMode = 2;

        System.Diagnostics.Stopwatch sw = new();
        sw.Start();

        shapes = new(triangulation.Count * 2 / 3);
        CreateAdjacencyListsSlow();

        System.Random rng = new();
        triangulation.Shuffle(rng);

        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            Triangle tri = triangulation[i];
            List<Triangle> adjList = tri.adjacencyList;
            if (adjList.Count == 0)
            {
                shapes.Add(tri);
                triangulation.RemoveAt(i);
                continue;
            }
            Triangle t = adjList[rng.Next(adjList.Count)];
            shapes.Add(new Quad(t, tri));
            foreach (Triangle triangle in adjList)
            {
                triangle.adjacencyList.Remove(tri);
            }
            foreach (Triangle triangle in t.adjacencyList)
            {
                triangle.adjacencyList.Remove(t);
            }
            triangulation[triangulation.FindIndex(tri => tri == t)] = triangulation[i - 1];
            triangulation[i - 1] = t;
            triangulation.RemoveAt(i);
            triangulation.RemoveAt(i - 1);
            i--;
        }

        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    // Subdivide quads and triangles
    // Make center vertex, draw segments from midpoints of edges to center
    public void Subdivide()
    {
        drawMode = 3;
        // Use HashSet
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();
        subdivision = new();
        foreach (IShapeObject shape in shapes)
        {
            Vector3[] midpts = shape.GetMidpoints();
            Vector3[] verts = shape.GetVertices();
            Vector3 center = shape.GetCenter();
            int numQuads = midpts.Length;
            for (int i = 0; i < numQuads; i++)
            {
                subdivision.Add(new Quad(new Vector3[] { verts[i], midpts[i], center, midpts[(i + (numQuads-1)) % numQuads] }));
            }
            
        }
        
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    // Skew vertices
    // https://andersource.dev/2020/11/06/organic-grid.html

    // Make Dual Grid
    // Grid for placement is based on midpoints of shapes
    // Other grid is based on vertices of shapes (This one is actually drawn)

    public void Clear()
    {
        if(vertices != null) vertices.Clear();
        if(triangulation != null) triangulation.Clear();
        if (shapes != null) shapes.Clear();
        if (subdivision != null) subdivision.Clear();
        drawMode = 0;
    }

    
}

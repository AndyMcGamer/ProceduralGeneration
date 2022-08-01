using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Classes;

public class Quad : IShapeObject
{
    public Edge[] edges;
    public Vector3[] vertices;
    public Quad(Triangle t1, Triangle t2)
    {
        vertices = new Vector3[4];
        Edge sharedEdge = Triangle.GetSharedEdge(t1, t2);
        Vector3[] exception = t1.vertices.Union(t2.vertices).Except(sharedEdge.vertices).ToArray();
        vertices[0] = exception[0];
        vertices[1] = sharedEdge.vertices[0];
        vertices[2] = exception[1];
        vertices[3] = sharedEdge.vertices[1];

        edges = new Edge[4] { new Edge(vertices[0], vertices[1]), new Edge(vertices[1], vertices[2]), new Edge(vertices[2], vertices[3]), new Edge(vertices[3], vertices[0]) };
    }

    public Quad(Vector3[] vertices)
    {
        this.vertices = vertices;
        edges = new Edge[4] { new Edge(vertices[0], vertices[1]), new Edge(vertices[1], vertices[2]), new Edge(vertices[2], vertices[3]), new Edge(vertices[3], vertices[0]) };
    }

    // https://math.stackexchange.com/questions/1889173/point-that-divides-a-quadrilateral-into-four-quadrilaterals-of-equal-area
    // Get point of intersection of diagonals, then reflect that point about the centroid
    public Vector3 GetCenter()
    {
        float a1 = vertices[0].z - vertices[2].z;
        float b1 = vertices[2].x - vertices[0].x;
        float c1 = a1 * vertices[0].x + b1 * vertices[0].z;

        float a2 = vertices[1].z - vertices[3].z;
        float b2 = vertices[3].x - vertices[1].x;
        float c2 = a2 * vertices[1].x + b2 * vertices[1].z;

        float det = (a1 * b2) - (a2 * b1);
        float xDet = (c1 * b2) - (c2 * b1);
        float yDet = (a1 * c2) - (a2 * c1);

        Vector3 intersection =  new(xDet / det, 0f, yDet / det);

        Vector3 centroid = GetCentroid();
        return intersection + 2 * (centroid - intersection);
    }

    private Vector3 GetCentroid()
    {
        // Make 4 Triangles
        Triangle[] triangles = new Triangle[4] { new Triangle(vertices[0], vertices[1], vertices[2]), new Triangle(vertices[2], vertices[3], vertices[0]), new Triangle(vertices[1], vertices[2], vertices[3]), new Triangle(vertices[3], vertices[0], vertices[1])};

        Vector3[] centroids = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            centroids[i] = triangles[i].GetCentroid();
        }

        // Find Intersection of Centroids
        float a1 = centroids[0].z - centroids[1].z; 
        float b1 = centroids[1].x - centroids[0].x;
        float c1 = a1 * centroids[0].x + b1 * centroids[0].z;

        float a2 = centroids[2].z - centroids[3].z;
        float b2 = centroids[3].x - centroids[2].x;
        float c2 = a2 * centroids[2].x + b2 * centroids[2].z;

        float det = (a1 * b2) - (a2 * b1);
        float xDet = (c1 * b2) - (c2 * b1);
        float yDet = (a1 * c2) - (a2 * c1);

        return new Vector3(xDet / det, 0f, yDet / det);

    }

    public Vector3[] GetMidpoints()
    {
        Vector3[] midpoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            Edge e = edges[i];
            midpoints[i] = new Vector3((e.vertices[0].x + e.vertices[1].x) / 2f, 0, (e.vertices[0].z + e.vertices[1].z) / 2f);
        }
        return midpoints;
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public Edge[] GetEdges()
    {
        return edges;
    }

}

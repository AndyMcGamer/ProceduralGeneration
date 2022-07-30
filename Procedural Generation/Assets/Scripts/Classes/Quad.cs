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

        edges = new Edge[4];
        edges[0] = new Edge(vertices[0], vertices[1]);
        edges[1] = new Edge(vertices[1], vertices[2]);
        edges[2] = new Edge(vertices[2], vertices[3]);
        edges[3] = new Edge(vertices[3], vertices[0]);
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

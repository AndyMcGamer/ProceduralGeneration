using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes
{
    public struct Triangle
    {
        public Edge[] edges;
        public Vector3[] vertices;

        public Triangle(Vector3[] vertices)
        {
            edges = new Edge[3];
            this.vertices = vertices;
            edges[0] = new Edge(vertices[0], vertices[1]);
            edges[1] = new Edge(vertices[1], vertices[2]);
            edges[2] = new Edge(vertices[2], vertices[0]);

        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            edges = new Edge[3];
            vertices = new Vector3[3];
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;
            edges[0] = new Edge(vertices[0], vertices[1]);
            edges[1] = new Edge(vertices[1], vertices[2]);
            edges[2] = new Edge(vertices[2], vertices[0]);
        }

        public bool InCircumcenter(Vector3 point)
        {
            // get difference between vertices and point (vertices must be ccw)

            float ax = vertices[2].x - point.x;
            float ay = vertices[2].z - point.z;
            float bx = vertices[1].x - point.x;
            float by = vertices[1].z - point.z;
            float cx = vertices[0].x - point.x;
            float cy = vertices[0].z - point.z;

            // get determinant
            float det = ((ax * ax + ay * ay) * (bx * cy - cx * by)) - ((bx * bx + by * by) * (ax * cy - cx * ay)) + ((cx * cx + cy * cy) * (ax * by - bx * ay));

            return det > 0; // if det is positive, point is inside
        }

        public bool HasEdge(Edge edge)
        {
            if (vertices.Intersect(edge.vertices).Count() == 2) return true;
            return false;
        }

        public bool SharesPoint(Triangle other)
        {
            if (vertices.Intersect(other.vertices).Any()) return true;
            return false;
        }
    }
}

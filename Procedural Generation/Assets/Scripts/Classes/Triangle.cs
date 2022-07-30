using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes
{
    public class Triangle : IShapeObject
    {

        public Edge[] edges;
        public Vector3[] vertices;
        // private Vector3 circumcenter;
        // private float circumradius;

        public List<Triangle> adjacencyList;

        public Triangle(Vector3[] vertices)
        {
            adjacencyList = new(3);
            edges = new Edge[3];
            this.vertices = vertices;
            edges[0] = new Edge(vertices[0], vertices[1]);
            edges[1] = new Edge(vertices[1], vertices[2]);
            edges[2] = new Edge(vertices[2], vertices[0]);
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            adjacencyList = new(3);
            edges = new Edge[3];
            vertices = new Vector3[3];
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;
            edges[0] = new Edge(vertices[0], vertices[1]);
            edges[1] = new Edge(vertices[1], vertices[2]);
            edges[2] = new Edge(vertices[2], vertices[0]);

        }

        //private void CalculateCircumcenterAndRadius()
        //{
        //    float mx1 = (vertices[1].x + vertices[0].x) / 2f;
        //    float my1 = (vertices[1].z + vertices[0].z) / 2f;
        //    float a1 = vertices[1].x - vertices[0].x;
        //    float b1 = vertices[1].z - vertices[0].z;
        //    float c1 = a1 * mx1 + b1 * my1;

        //    float mx2 = (vertices[1].x + vertices[2].x) / 2;
        //    float my2 = (vertices[1].z + vertices[2].z) / 2;
        //    float a2 = vertices[2].x - vertices[1].x;
        //    float b2 = vertices[2].z - vertices[1].z;
        //    float c2 = a2 * mx2 + b2 * my2;

        //    // Use Cramers Rule

        //    float det = (a1 * b2) - (a2 * b1);
        //    float xDet = (c1 * b2) - (c2 * b1);
        //    float yDet = (a1 * c2) - (a2 * c1);

        //    circumcenter = new Vector3(xDet / det, 0, yDet / det);
        //    circumradius = (circumcenter - vertices[0]).sqrMagnitude;
        //}

        // Pre computing circumradius and circumcenter yields marginal improvement (30ms with 20 sides and radius of 10)
        // At large values of n, the method of computing circumcenter is inaccurate and the same speed (or even slower) compared to the typical method
        
        //public bool BetterInCircumcenter(Vector3 point) 
        //{
        //    return (circumcenter - point).sqrMagnitude <= circumradius;
        //}
        
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

        public Vector3[] GetVertices()
        {
            return vertices;
        }

        public Edge[] GetEdges()
        {
            return edges;
        }

        public static Edge GetSharedEdge(Triangle t1, Triangle t2)
        {
            Vector3[] shared = t1.vertices.Intersect(t2.vertices).ToArray();
            if (shared.Length == 2)
            {
                return new Edge(shared);
            }
            else
            {
                return new Edge();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes 
{
    public struct Edge
    {
        public readonly Vector3[] vertices;

        public Edge(Vector3 v1, Vector3 v2)
        {
            vertices = new Vector3[2];
            vertices[0] = v1;
            vertices[1] = v2;
        }

        public Edge(Vector3[] vertices)
        {
            this.vertices = new Vector3[2];
            this.vertices[0] = vertices[0];
            this.vertices[1] = vertices[1];
        }
    }
}


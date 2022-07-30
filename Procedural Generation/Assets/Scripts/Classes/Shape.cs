using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classes
{
    public interface IShapeObject
    {
        public Vector3[] GetVertices();

        public Edge[] GetEdges();
    }
}

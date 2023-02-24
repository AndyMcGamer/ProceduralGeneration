using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Organic.Classes
{
    public interface IShapeObject
    {
        public Vector3[] GetVertices();

        public Edge[] GetEdges();

        public Vector3 GetCenter();

        public Vector3[] GetMidpoints();
    }
}

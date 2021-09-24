using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEVertex
    {
        public HEVertex() { }

        public HEVertex(Vector3D position, int outerGoing)
        {
            _position = position;
            _outerGoing = outerGoing;
        }

        public Vector3D Position { get { return _position; } set { _position = value; } }
        private Vector3D _position;

        public int OuterGoing { get { return _outerGoing; } set { _outerGoing = value; } }
        private int _outerGoing;
    }
}
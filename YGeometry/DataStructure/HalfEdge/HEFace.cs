using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEFace
    {
        public HEFace()
        {

        }

        public HEFace(int relative)
        {
            _relativeEdge = relative;
        }

        public int RelativeEdge { get { return _relativeEdge; } set { _relativeEdge = value; } }
        private int _relativeEdge;
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEEdge
    {
        public HEEdge()
        {

        }

        public HEEdge(int goingTo, int relativeFace, int nextEdge, int oppEdge)
        {
            _goingTo = goingTo;
            _relativeFace = relativeFace;
            _nextEdge = nextEdge;
            _oppEdge = oppEdge;
        }

        public int GoingTo { get { return _goingTo; } set { _goingTo = value; } }
        private int _goingTo;

        public int RelativeFace { get { return _relativeFace; } set { _relativeFace = value; } }
        private int _relativeFace;

        public int NextEdge { get { return _nextEdge; } set { _nextEdge = value; } }
        private int _nextEdge;

        public int OppEdge { get { return _oppEdge; } set { _oppEdge = value; } }
        private int _oppEdge;
    }
}
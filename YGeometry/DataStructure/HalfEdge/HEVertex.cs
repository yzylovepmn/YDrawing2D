using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEVertex : IHEMeshNode
    {
        public HEVertex() { }

        public HEVertex(int id, Vector3D position, HEEdge outerGoing)
        {
            _id = id;
            _position = position;
            _outerGoing = outerGoing;
        }

        public int ID { get { return _id; } internal set { _id = value; } }
        private int _id = HEMesh.InvaildID;

        public Vector3D Position { get { return _position; } internal set { _position = value; } }
        private Vector3D _position;

        internal HEEdge OuterGoing { get { return _outerGoing; } set { _outerGoing = value; } }
        private HEEdge _outerGoing;

        public bool IsIsolated { get { return _outerGoing == null; } }

        public bool IsBoundary { get { return _outerGoing == null; } }

        public List<HEVertex> GetAdjacentVertice()
        {
            var neighbors = new List<HEVertex>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                var first = edge.GoingTo;
                neighbors.Add(first);
                while (true)
                {
                    edge = edge.PreEdge?.OppEdge;
                    if (edge != null && edge != _outerGoing)
                        neighbors.Add(edge.GoingTo);
                    else break;
                }
            }
            return neighbors;
        }

        public List<HEdge> GetAdjacentEdges()
        {
            var neighbors = new List<HEdge>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                neighbors.Add(edge.RelativeEdge);
                while (true)
                {
                    edge = edge.PreEdge?.OppEdge;
                    if (edge != null && edge != _outerGoing)
                        neighbors.Add(edge.RelativeEdge);
                    else break;
                }
            }
            return neighbors;
        }

        internal List<HEEdge> GetAdjacentHalfEdges()
        {
            var neighbors = new List<HEEdge>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                neighbors.Add(edge);
                while (true)
                {
                    edge = edge.PreEdge?.OppEdge;
                    if (edge != null && edge != _outerGoing)
                        neighbors.Add(edge);
                    else break;
                }
            }
            return neighbors;
        }

        public List<HEFace> GetAdjacentFaces()
        {
            var neighbors = new List<HEFace>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                neighbors.Add(edge.RelativeFace);
                while (true)
                {
                    edge = edge.PreEdge?.OppEdge;
                    if (edge != null && edge != _outerGoing)
                        neighbors.Add(edge.RelativeFace);
                    else break;
                }
            }
            return neighbors;
        }

        public void Dispose()
        {
            _id = HEMesh.InvaildID;
            _outerGoing = null;
        }
    }
}
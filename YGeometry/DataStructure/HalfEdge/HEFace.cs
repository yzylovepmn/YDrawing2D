using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEFace : IHEMeshNode
    {
        internal HEFace()
        {

        }

        internal HEFace(HEEdge relative)
        {
            _relative = relative;
        }

        public int ID { get { return _id; } internal set { _id = value; } }
        private int _id = HEMesh.InvaildID;

        int IHEMeshNode.ID { get { return _id; } set { _id = value; } }

        public bool IsDeleted { get { return _id == HEMesh.InvaildID; } }

        public bool IsIsolated { get { return GetHalfEdges().All(edge => edge.OppEdge.IsBoundary); } }

        public bool IsBoundary { get { return GetHalfEdges().Any(edge => edge.OppEdge.IsBoundary); } }

        internal HEdge RelativeEdge { get { return _relative.RelativeEdge; } }

        internal HEEdge Relative { get { return _relative; } set { _relative = value; } }
        private HEEdge _relative;

        public int Degree { get { return _degree; } }
        private int _degree = HEMesh.InvaildID;

        public bool IsTriangle { get { return _degree == 3; } }

        // ccw
        public List<HEVertex> GetVertice()
        {
            var vertice = new List<HEVertex>();

            var edge = _relative;
            while (true)
            {
                vertice.Add(edge.VertexTo);
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }

            return vertice;
        }

        // ccw
        public List<HEdge> GetEdges()
        {
            var edges = new List<HEdge>();

            var edge = _relative;
            while (true)
            {
                edges.Add(edge.RelativeEdge);
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }

            return edges;
        }

        public List<HEFace> GetAdjacentFaces()
        {
            var faces = new List<HEFace>();

            var edge = _relative;
            while (true)
            {
                var face = edge.OppEdge.RelativeFace;
                if (face != null)
                    faces.Add(face);
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }

            return faces;
        }

        internal List<HEEdge> GetHalfEdges()
        {
            var edges = new List<HEEdge>();

            var edge = _relative;
            while (true)
            {
                edges.Add(edge);
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }

            return edges;
        }

        public bool Contains(HEVertex vertex)
        {
            var edge = _relative;
            while (true)
            {
                if (edge.VertexTo == vertex)
                    return true;
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }
            return false;
        }

        public HEEdge HalfEdgeFromVertex(HEVertex vertex)
        {
            var edge = _relative;
            while (true)
            {
                if (edge.VertexFrom == vertex)
                    return edge;
                edge = edge.NextEdge;
                if (edge == _relative)
                    break;
            }
            return null;
        }

        internal void UpdateDegree()
        {
            if (_relative == null)
                _degree = HEMesh.InvaildID;
            else
            {
                var edge = _relative;
                var degree = 0;
                while (true)
                {
                    degree++;
                    edge = edge.NextEdge;
                    if (edge == _relative)
                        break;
                }
                _degree = degree;
            }
        }

        public void Dispose()
        {
            _id = HEMesh.InvaildID;
            _degree = HEMesh.InvaildID;
            _relative = null;
        }
    }
}
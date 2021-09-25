using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEFace : IHEMeshNode
    {
        public HEFace()
        {

        }

        public HEFace(HEEdge relative)
        {
            _relative = relative;
        }

        public int ID { get { return _id; } internal set { _id = value; } }
        private int _id = HEMesh.InvaildID;

        public bool IsIsolated { get { return GetHalfEdges().All(edge => edge.IsBoundary); } }

        public bool IsBoundary { get { return GetHalfEdges().Any(edge => edge.IsBoundary); } }

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
                vertice.Add(edge.GoingTo);
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
                faces.Add(edge.RelativeFace);
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

        public int UpdateDegree()
        {
            _UpdateDegree();
            return _degree;
        }

        private void _UpdateDegree()
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
using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEMesh : IMesh
    {
        const int DefaultSize = 1024;
        const int InvaildID = -1;

        private List<HEVertex> _vertices;
        private List<Vector3D> _verticeNormals;
        private List<HEEdge> _edges;
        private List<HEFace> _triangles;
        private List<Vector3D> _trianglesNormals;

        public HEMesh()
        {
            _InitData(DefaultSize);
        }

        private void _InitData(int vcount)
        {
            _vertices = new List<HEVertex>(vcount);
            _edges = new List<HEEdge>(vcount * 6);
            _triangles = new List<HEFace>(vcount * 2);
            _trianglesNormals = new List<Vector3D>(vcount * 2);
        }

        public IndexN<int> GetTriangle(int tid)
        {
            var vertice = new Queue<int>();

            var face = _triangles[tid];
            var edge = _edges[face.RelativeEdge];
            int first = edge.GoingTo;
            vertice.Enqueue(first);
            while (true)
            {
                edge = _edges[edge.NextEdge];
                if (edge.GoingTo != first)
                    vertice.Enqueue(edge.GoingTo);
                else break;
            }

            return new IndexN<int>(vertice);
        }

        public Vector3D GetVertex(int vid)
        {
            return _vertices[vid].Position;
        }

        public Vector3D GetNormal(int vid)
        {
            if (HasNormals)
                return _verticeNormals[vid];
            throw new InvalidOperationException("Please call ComputeNormals() method before invoke GetNormal() method!");
        }

        public Vector3D GetTriangleNormal(int tid)
        {
            var normal = _trianglesNormals[tid];
            if (normal.IsZero)
                normal = _ComputeTriangleNormal(GetTriangle(tid));
            return normal;
        }

        public IndexN<int> GetNeighborVertice(int vid)
        {
            var vertice = new Queue<int>();

            var vertex = _vertices[vid];
            var edge = _edges[vertex.OuterGoing];
            int first = edge.GoingTo;
            vertice.Enqueue(first);
            while (true)
            {
                edge = _GetOppEdge(_GetPreEdge(edge));
                if (edge.GoingTo != first)
                    vertice.Enqueue(edge.GoingTo);
                else break;
            }

            return new IndexN<int>(vertice);
        }

        private HEEdge _GetPreEdge(HEEdge edge)
        {
            return _edges[_edges[edge.NextEdge].NextEdge];
        }

        private HEEdge _GetOppEdge(HEEdge edge)
        {
            return _edges[edge.OppEdge];
        }

        public void ComputeNormals()
        {
            _verticeNormals = new List<Vector3D>(VertexCount);
            for (int i = 0; i < VertexCount; i++)
            {
                var normal = new Vector3D();
                var vertex = GetVertex(i);
                var neighbor = GetNeighborVertice(i);
                for (int j = 0; j < neighbor.Length; j++)
                {
                    int i1 = j;
                    int i2 = (j + 1) % neighbor.Length;
                    var v1 = GetVertex(i1) - vertex;
                    var v2 = GetVertex(i2) - vertex;
                    normal += Vector3D.CrossProduct(v1, v2) * Vector3D.AngleBetween(v1, v2);
                }
                normal.Normalize();
                _verticeNormals[i] = normal;
            }
        }

        public void ComputeFaceNormals()
        {
            for (int i = 0; i < TriangleCount; i++)
            {
                if (_trianglesNormals[i].IsZero)
                    _trianglesNormals[i] = _ComputeTriangleNormal(GetTriangle(i));
            }
        }

        private Vector3D _ComputeTriangleNormal(IndexN<int> vertice)
        {
            var p0 = GetVertex(vertice[0]);
            var p1 = GetVertex(vertice[1]);
            var p2 = GetVertex(vertice[2]);
            var normal = Vector3D.CrossProduct(p1 - p0, p2 - p0);
            normal.Normalize();
            return normal;
        }

        public int TriangleCount { get { return _triangles.Count; } }

        public bool HasNormals { get { return _verticeNormals != null; } }

        public int VertexCount { get { return _vertices.Count; } }
    }
}
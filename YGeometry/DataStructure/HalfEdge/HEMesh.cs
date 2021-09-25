using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEMesh : IMesh, IDisposable
    {
        const int DefaultSize = 1024;
        internal const int InvaildID = -1;

        private List<HEVertex> _vertices;
        private List<HEdge> _edges;
        private List<HEEdge> _hedges;
        private List<HEFace> _faces;
        private List<Vector3D> _verticeNormals;
        private List<Vector3D> _facesNormals;

        public int FaceCount { get { return _faces.Count; } }

        public int VertexCount { get { return _vertices.Count; } }

        public int EdgeCount { get { return _edges.Count; } }

        public int HEdgeCount { get { return _hedges.Count; } }

        public bool HasNormals { get { return _verticeNormals != null; } }

        public HEMesh()
        {
            _InitData(DefaultSize);
        }

        private void _InitData(int vcount)
        {
            _vertices = new List<HEVertex>(vcount);
            _edges = new List<HEdge>(vcount * 3);
            _hedges = new List<HEEdge>(vcount * 6);
            _faces = new List<HEFace>(vcount * 2);
            _facesNormals = new List<Vector3D>(vcount * 2);
        }

        public IndexN<int> GetVerticeOfFace(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            return new IndexN<int>(_faces[fid].GetVertice().Select(v => v.ID));
        }

        public Vector3D GetPosition(int vid)
        {
            Debugs.CheckRange(vid, 0, VertexCount);
            return _vertices[vid].Position;
        }

        public HEVertex GetVertex(int vid)
        {
            Debugs.CheckRange(vid, 0, VertexCount);
            return _vertices[vid];
        }

        public HEdge GetEdge(int eid)
        {
            Debugs.CheckRange(eid, 0, EdgeCount);
            return _edges[eid];
        }

        internal HEEdge GetHEdge(int eid)
        {
            Debugs.CheckRange(eid, 0, HEdgeCount);
            return _hedges[eid];
        }

        public HEFace GetFace(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            return _faces[fid];
        }

        public Vector3D GetNormal(int vid)
        {
            if (HasNormals)
                return _verticeNormals[vid];
            throw new InvalidOperationException("Please call ComputeNormals() method before invoke GetNormal() method!");
        }

        public Vector3D GetNormal(HEVertex vertex)
        {
            if (HasNormals)
                return _verticeNormals[vertex.ID];
            throw new InvalidOperationException("Please call ComputeNormals() method before invoke GetNormal() method!");
        }

        public Vector3D GetFaceNormal(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            var normal = _facesNormals[fid];
            if (normal.IsZero)
                normal = _ComputeFaceNormal(GetFace(fid).GetVertice());
            return normal;
        }

        public Vector3D GetFaceNormal(HEFace face)
        {
            return GetFaceNormal(face.ID);
        }

        public IndexN<int> GetNeighborVerticeOfVertex(int vid)
        {
            Debugs.CheckRange(vid, 0, VertexCount);
            return new IndexN<int>(_vertices[vid].GetAdjacentVertice().Select(v => v.ID));
        }

        public IndexN<int> GetEdgesOfFace(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            return new IndexN<int>(_faces[fid].GetEdges().Select(edge => edge.ID));
        }

        public IndexN<int> GetNeighborOfFace(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            throw new NotImplementedException();
        }

        public HEdge GetEdgeBetween(HEVertex v1, HEVertex v2)
        {
            if (IsConnected(v1, v2))
                return v1.OuterGoing.RelativeEdge;
            return null;
        }

        public void ComputeVerticeNormals()
        {
            _verticeNormals = new List<Vector3D>(VertexCount);
            for (int i = 0; i < VertexCount; i++)
            {
                var normal = new Vector3D();
                var vertex = GetVertex(i);
                var neighbor = vertex.GetAdjacentVertice();
                for (int j = 0; j < neighbor.Count; j++)
                {
                    int i1 = j;
                    int i2 = (j + 1) % neighbor.Count;
                    var v1 = neighbor[i1].Position - vertex.Position;
                    var v2 = neighbor[i2].Position - vertex.Position;
                    normal += Vector3D.CrossProduct(v1, v2) * Vector3D.AngleBetween(v1, v2);
                }
                normal.Normalize();
                _verticeNormals[i] = normal;
            }
        }

        public void ComputeFaceNormals()
        {
            for (int i = 0; i < FaceCount; i++)
            {
                if (_facesNormals[i].IsZero)
                    _facesNormals[i] = _ComputeFaceNormal(GetFace(i).GetVertice());
            }
        }

        private Vector3D _ComputeFaceNormal(List<HEVertex> vertice)
        {
            var p0 = vertice[0].Position;
            var p1 = vertice[1].Position;
            var p2 = vertice[2].Position;
            var normal = Vector3D.CrossProduct(p1 - p0, p2 - p0);
            normal.Normalize();
            return normal;
        }

        public bool IsConnected(HEVertex v1, HEVertex v2)
        {
            return v1.OuterGoing.GoingTo == v2;
        }

        public bool IsConnected(int v1, int v2)
        {
            Debugs.CheckRange(v1, 0, VertexCount);
            Debugs.CheckRange(v2, 0, VertexCount);
            return IsConnected(_vertices[v1], _vertices[v2]);
        }

        public bool IsTriangleMesh()
        {
            foreach (var face in _faces)
            {
                if (!face.IsTriangle)
                    return false;
            }
            return true;
        }

        #region Edit Method

        #endregion

        public void Dispose()
        {
        }
    }
}
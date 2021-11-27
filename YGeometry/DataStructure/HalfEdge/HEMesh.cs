using System;
using System.Collections;
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

        public IEnumerable<HEVertex> Vertices { get { return _vertices; } }

        public IEnumerable<HEFace> Faces { get { return _faces; } }

        public int FaceCount { get { return _faces.Count; } }

        public int VertexCount { get { return _vertices.Count; } }

        public int EdgeCount { get { return _edges.Count; } }

        public int HEdgeCount { get { return _hedges.Count; } }

        public bool HasVerticeNormals { get { return _verticeNormals != null; } }

        public bool HasFaceNormals { get { return _facesNormals != null; } }

        public HEMesh(int verticeSize = DefaultSize)
        {
            _InitData(DefaultSize);
        }

        private void _InitData(int vcount)
        {
            _vertices = new List<HEVertex>(vcount);
            _edges = new List<HEdge>(vcount * 3);
            _hedges = new List<HEEdge>(vcount * 6);
            _faces = new List<HEFace>(vcount * 2);
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

        public Vector3D GetVertexNormal(int vid)
        {
            Debugs.CheckRange(vid, 0, VertexCount);
            if (HasVerticeNormals)
                return _verticeNormals[vid];
            throw new InvalidOperationException("Please call UpdateVerticeNormals() method before invoke GetNormal() method!");
        }

        public Vector3D GetVertexNormal(HEVertex vertex)
        {
            return GetVertexNormal(vertex.ID);
        }

        public Vector3D GetFaceNormal(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            if (HasFaceNormals)
                return _facesNormals[fid];
            throw new InvalidOperationException("Please call UpdateFaceNormals() method before invoke GetNormal() method!");
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

        public IndexN<int> GetNeighborFaceOfFace(int fid)
        {
            Debugs.CheckRange(fid, 0, FaceCount);
            return new IndexN<int>(_faces[fid].GetAdjacentFaces().Select(f => f.ID));
        }

        public HEdge GetEdgeBetween(HEVertex v1, HEVertex v2)
        {
            if (IsConnected(v1, v2))
                return v1.OuterGoing.RelativeEdge;
            return null;
        }

        public void UpdateVerticeNormals()
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
                _verticeNormals.Add(normal);
            }
        }

        public void UpdateFaceNormals()
        {
            _facesNormals = new List<Vector3D>(FaceCount);
            for (int i = 0; i < FaceCount; i++)
                _facesNormals.Add(_ComputeFaceNormal(GetFace(i).GetVertice()));
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

        public HEdge EdgeBetween(HEVertex v1, HEVertex v2)
        {
            if (v1 == null || v2 == null) return null;
            if (!v1.IsIsolated)
            {
                var edge = v1.OuterGoing;
                do
                {
                    if (edge.VertexTo == v2) return edge.RelativeEdge;
                    edge = edge.RotateNext;
                }
                while (edge != v1.OuterGoing);
            }
            return null;
        }

        /// <summary>
        /// ret is point to v2
        /// </summary>
        internal HEEdge HEdgeBetween(HEVertex v1, HEVertex v2)
        {
            var edge = EdgeBetween(v1, v2);
            if (edge != null)
                return edge.Relative.VertexTo == v2 ? edge.Relative : edge.Relative.OppEdge;
            return null;
        }

        public HEdge EdgeBetween(int vid1, int vid2)
        {
            Debugs.CheckRange(vid1, 0, VertexCount);
            Debugs.CheckRange(vid2, 0, VertexCount);
            return EdgeBetween(_vertices[vid1], _vertices[vid2]);
        }

        public bool IsConnected(HEVertex v1, HEVertex v2)
        {
            return EdgeBetween(v1, v2) != null;
        }

        public bool IsConnected(int vid1, int vid2)
        {
            Debugs.CheckRange(vid1, 0, VertexCount);
            Debugs.CheckRange(vid2, 0, VertexCount);
            return IsConnected(_vertices[vid1], _vertices[vid2]);
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
        public void Reset(int verticeSize = DefaultSize)
        {
            Dispose();
            _InitData(verticeSize);
        }

        public HEVertex AddVertex(double x, double y, double z)
        {
            return AddVertex(new Vector3D(x, y, z));
        }

        public HEVertex AddVertex(Vector3D vertex)
        {
            var ret = _NewVertex();
            ret.Position = vertex;
            return ret;
        }

        /// <summary>
        /// keep sure input is ccw
        /// </summary>
        public HEFace AddFace(IndexN<int> vindice)
        {
            Debugs.Assert(vindice.All(id => id >= 0 && id < VertexCount));
            var vertice = new List<HEVertex>(vindice.Length);
            for (int i = 0; i < vindice.Length; i++)
                vertice.Add(_vertices[vindice[i]]);
            return AddFace(vertice);
        }

        /// <summary>
        /// keep sure input is ccw
        /// </summary>
        public HEFace AddFace(List<HEVertex> vertice)
        {
            if (vertice == null || vertice.Count < 3) return null;

            for (int i = 0; i < vertice.Count; i++)
            {
                var v1 = vertice[i];
                if (!v1.IsBoundary)
                {
                    Debugs.Log("will create none-manifold surface");
                    return null;
                }
                var v2 = vertice[(i + 1) % vertice.Count];
                var edge = HEdgeBetween(v1, v2);
                if (edge != null && !edge.IsBoundary)
                {
                    Debugs.Log("will create none-manifold surface");
                    return null;
                }
            }

            var face = _NewFace();
            var hedges = new List<HEEdge>(vertice.Count);
            var flags_edge_new = new bool[vertice.Count];
            for (int i = 0; i < vertice.Count; i++)
            {
                var vid1 = i;
                var vid2 = (i + 1) % vertice.Count;
                var v1 = vertice[vid1];
                var v2 = vertice[vid2];
                var edge = EdgeBetween(v1, v2);
                // keep sure edges exist
                if (edge == null)
                {
                    flags_edge_new[i] = true;
                    edge = AddEdge(v1, v2);
                }
                HEEdge toV1, toV2;
                if (edge.Relative.VertexTo == v1)
                {
                    toV1 = edge.Relative;
                    toV2 = edge.Relative.OppEdge;
                }
                else
                {
                    toV2 = edge.Relative;
                    toV1 = edge.Relative.OppEdge;
                }
                toV2.RelativeFace = face;
                if (face.Relative == null)
                    face.Relative = toV2;
                hedges.Add(toV2);
            }

            for (int i = 0; i < vertice.Count; i++)
            {
                var id1 = i;
                var id2 = (i + 1) % vertice.Count;
                if (!flags_edge_new[id1] && !flags_edge_new[id2])
                {
                    var edge1 = hedges[id1];
                    var edge2 = hedges[id2];
                    if (edge1.NextEdge != edge2)
                    {
                        var edgeopp2 = edge2.OppEdge;
                        var edgeopp1 = edge1.OppEdge;
                        var edge3 = edgeopp2;
                        do
                        {
                            edge3 = edge3.NextEdge.OppEdge;
                        }
                        while (!edge3.IsBoundary);
                        var edge4 = edge3.NextEdge;

                        var edge5 = edge1.NextEdge;
                        var edge6 = edge2.PreEdge;

                        edge3.NextEdge = edge5;
                        edge5.PreEdge = edge3;
                        edge6.NextEdge = edge4;
                        edge4.PreEdge = edge6;
                        edge1.NextEdge = edge2;
                        edge2.PreEdge = edge1;
                    }
                }
            }

            var vertice_need_update = new List<HEVertex>();
            for (int i = 0; i < vertice.Count; i++)
            {
                var id1 = i;
                var id2 = (i + 1) % vertice.Count;
                var edge1 = hedges[id1];
                var edge2 = hedges[id2];
                var edgeop1 = edge1.OppEdge;
                var edgeop2 = edge2.OppEdge;
                var vertex = vertice[id2];

                if (flags_edge_new[id1])
                {
                    if (flags_edge_new[id2])
                    {
                        if (vertex.IsIsolated)
                        {
                            vertex.OuterGoing = edgeop1;
                            edgeop2.NextEdge = edgeop1;
                            edgeop1.PreEdge = edgeop2;
                        }
                        else
                        {
                            var e1 = vertex.OuterGoing;
                            var e2 = vertex.OuterGoing.PreEdge;
                            edgeop2.NextEdge = e1;
                            e1.PreEdge = edgeop2;
                            edgeop1.PreEdge = e2;
                            e2.NextEdge = edgeop1;
                        }
                    }
                    else
                    {
                        var e1 = edge2.PreEdge;
                        e1.NextEdge = edgeop1;
                        edgeop1.PreEdge = e1;
                        vertex.OuterGoing = edgeop1;
                    }
                }
                else
                {
                    if (flags_edge_new[id2])
                    {
                        var e1 = edge1.NextEdge;
                        edgeop2.NextEdge = e1;
                        e1.PreEdge = edgeop2;
                        vertex.OuterGoing = e1;
                    }
                    else vertice_need_update.Add(vertex);
                }

                edge1.NextEdge = edge2;
                edge2.PreEdge = edge1;
            }

            foreach (var v in vertice_need_update)
                v.AdjustOutGoingToBoundary();

            face.UpdateDegree();
            return face;
        }

        internal HEdge AddEdge(int vid1, int vid2)
        {
            Debugs.CheckRange(vid1, 0, VertexCount);
            Debugs.CheckRange(vid2, 0, VertexCount);
            return AddEdge(_vertices[vid1], _vertices[vid2]);
        }

        internal HEdge AddEdge(HEVertex v1, HEVertex v2)
        {
            if (v1 == null || v2 == null)
                return null;

            var edge = EdgeBetween(v1, v2);
            if (edge != null) return edge;

            edge = _NewEdge();
            var hedge1 = _NewHEdge();
            var hedge2 = _NewHEdge();

            edge.V1 = v1;
            edge.V2 = v2;
            edge.Relative = hedge1;

            hedge1.VertexTo = v2;
            hedge1.OppEdge = hedge2;
            hedge1.RelativeEdge = edge;
            hedge1.PreEdge = hedge2;
            hedge1.NextEdge = hedge2;

            hedge2.VertexTo = v1;
            hedge2.OppEdge = hedge1;
            hedge2.RelativeEdge = edge;
            hedge2.PreEdge = hedge1;
            hedge2.NextEdge = hedge1;

            return edge;
        }

        public void DeleteVertex(int vid)
        {
            Debugs.CheckRange(vid, 0, VertexCount);
            DeleteVertex(_vertices[vid]);
        }

        public void DeleteVertex(HEVertex vertex)
        {
            if (vertex == null || vertex.IsDeleted) return;

            if (!vertex.IsIsolated)
            {
                var edges = vertex.GetAdjacentEdges();
                foreach (var edge in edges)
                    DeleteEdge(edge);
            }

            _DeleteVertex(vertex);
        }

        internal void DeleteEdge(HEdge edge)
        {
            if (edge == null || edge.IsDeleted) return;

            var h1 = edge.Relative;
            var h2 = h1.OppEdge;
            if (edge.IsIsolated)
            {
                var v_from_h1 = h1.VertexFrom;
                var v_from_h2 = h2.VertexFrom;
                var h1_next = h1.RotateNext;
                var h2_next = h2.RotateNext;
                if (v_from_h1.OuterGoing == h1)
                    v_from_h1.OuterGoing = h1_next == h1 ? null : h1_next;
                if (v_from_h2.OuterGoing == h2)
                    v_from_h2.OuterGoing = h2_next == h2 ? null : h2_next;

                var h1_to = h1.PreEdge;
                var h2_to = h2.PreEdge;

                h1_to.NextEdge = h1_next;
                h1_next.PreEdge = h1_to;
                h2_to.NextEdge = h2_next;
                h2_next.PreEdge = h2_to;

                _DeleteHEdge(h1);
                _DeleteHEdge(h2);
                _DeleteEdge(edge);
            }
            else
            {
                DeleteFace(h1.RelativeFace);
                DeleteFace(h2.RelativeFace);
            }
        }

        public void DeleteFace(HEFace face)
        {
            if (face == null || face.IsDeleted) return;

            var hedges = face.GetHalfEdges();
            var vertice = face.GetVertice();
            foreach (var hedge in hedges)
            {
                hedge.RelativeFace = null;
                if (hedge.RelativeEdge.IsIsolated)
                    DeleteEdge(hedge.RelativeEdge);
            }

            foreach (var vertex in vertice)
                vertex.AdjustOutGoingToBoundary();

            _DeleteFace(face);
        }

        private void _DeleteVertex(HEVertex vertex)
        {
            _DeleteElement(vertex, _vertices);
        }

        private void _DeleteEdge(HEdge edge)
        {
            _DeleteElement(edge, _edges);
        }

        private void _DeleteHEdge(HEEdge hedge)
        {
            _DeleteElement(hedge, _hedges);
        }

        private void _DeleteFace(HEFace face)
        {
            _DeleteElement(face, _faces);
        }

        private void _DeleteElement(IHEMeshNode element, IList list)
        {
            if (element.IsDeleted) return;
            var id = element.ID;
            Debugs.CheckRange(id, 0, list.Count);
            var v = list[id];
            if (v != element) throw new InvalidOperationException("The element is not belong to this mesh!");
            var lastID = list.Count - 1;
            if (id != lastID)
            {
                var last = list[lastID] as IHEMeshNode;
                last.ID = id;
                list[id] = last;
            }
            list.RemoveAt(lastID);
            element.Dispose();
        }

        private HEVertex _NewVertex()
        {
            var ret = new HEVertex();
            ret.ID = VertexCount;
            _vertices.Add(ret);
            return ret;
        }

        private HEdge _NewEdge()
        {
            var ret = new HEdge();
            ret.ID = _edges.Count;
            _edges.Add(ret);
            return ret;
        }

        private HEEdge _NewHEdge()
        {
            var ret = new HEEdge();
            ret.ID = _hedges.Count;
            _hedges.Add(ret);
            return ret;
        }

        private HEFace _NewFace()
        {
            var ret = new HEFace();
            ret.ID = FaceCount;
            _faces.Add(ret);
            return ret;
        }
        #endregion

        #region Properties check
#if DEBUG
        public bool IsManifold()
        {
            foreach (var ver in _vertices)
            {
                var num = 0;
                foreach (var edge in ver.GetAdjacentHalfEdges())
                {
                    if (edge.IsBoundary)
                        num++;
                }
                if (num > 1)
                    return false;
            }
            return true;
        }
#endif

        public bool IsClosed()
        {
            return _hedges.All(h => !h.IsBoundary);
        }
        #endregion

        public void Dispose()
        {
            _vertices.Dispose();
            _edges.Dispose();
            _hedges.Dispose();
            _faces.Dispose();

            _vertices = null;
            _edges = null;
            _hedges = null;
            _faces = null;
            _verticeNormals = null;
            _facesNormals = null;
        }
    }
}
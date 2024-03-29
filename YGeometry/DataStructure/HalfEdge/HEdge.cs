﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEdge : IHEMeshNode
    {
        internal HEdge()
        {

        }

        internal HEdge(int id, HEVertex v1, HEVertex v2, HEEdge relative)
        {
            _id = id;
            _v1 = v1;
            _v2 = v2;
            _relative = relative;
        }

        private int _id = HEMesh.InvaildID;
        private HEVertex _v1;
        private HEVertex _v2;
        private HEEdge _relative;

        public int ID { get { return _id; } internal set { _id = value; } }

        int IHEMeshNode.ID { get { return _id; } set { _id = value; } }

        public bool IsDeleted { get { return _id == HEMesh.InvaildID; } }

        public bool IsIsolated { get { return _relative.IsIsolated && _relative.OppEdge.IsIsolated; } }

        public bool IsBoundary { get { return _relative.IsBoundary || _relative.OppEdge.IsBoundary; } }

        public HEVertex V1 { get { return _v1; } internal set { _v1 = value; } }

        public HEVertex V2 { get { return _v2; } internal set { _v2 = value; } }

        internal HEEdge Relative { get { return _relative; } set { _relative = value; } }

        public List<HEFace> GetAdjacentFaces()
        {
            var adjacent = new List<HEFace>();
            if (_relative != null)
            {
                var face1 = _relative.RelativeFace;
                var face2 = _relative.OppEdge.RelativeFace;
                if (face1 != null)
                    adjacent.Add(face1);
                if (face2 != null)
                    adjacent.Add(face2);
            }
            return adjacent;
        }

        public void Dispose()
        {
            _id = HEMesh.InvaildID;
            _v1 = null;
            _v2 = null;
            _relative = null;
        }
    }
}
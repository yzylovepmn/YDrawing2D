using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEEdge : IHEMeshNode
    {
        public HEEdge()
        {

        }

        public HEEdge(int id, HEVertex goingTo, HEFace relativeFace, HEdge relativeEdge, HEEdge nextEdge, HEEdge preEdge, HEEdge oppEdge)
        {
            _id = id;
            _goingTo = goingTo;
            _relativeFace = relativeFace;
            _relativeEdge = relativeEdge;
            _nextEdge = nextEdge;
            _preEdge = preEdge;
            _oppEdge = oppEdge;
        }

        public int ID { get { return _id; } internal set { _id = value; } }
        private int _id = HEMesh.InvaildID;

        public bool IsIsolated { get { return _nextEdge == null && _preEdge == null; } }

        public bool IsBoundary { get { return _relativeFace == null ^ _oppEdge._relativeFace == null; } }

        public HEVertex GoingTo { get { return _goingTo; } internal set { _goingTo = value; } }
        private HEVertex _goingTo;

        public HEFace RelativeFace { get { return _relativeFace; } internal set { _relativeFace = value; } }
        private HEFace _relativeFace;

        public HEdge RelativeEdge { get { return _relativeEdge; } internal set { _relativeEdge = value; } }
        private HEdge _relativeEdge;

        public HEEdge NextEdge { get { return _nextEdge; } internal set { _nextEdge = value; } }
        private HEEdge _nextEdge;

        public HEEdge PreEdge { get { return _preEdge; } internal set { _preEdge = value; } }
        private HEEdge _preEdge;

        public HEEdge OppEdge { get { return _oppEdge; } internal set { _oppEdge = value; } }
        private HEEdge _oppEdge;

        public void Dispose()
        {
            _id = HEMesh.InvaildID;
            _goingTo = null;
            _relativeFace = null;
            _relativeEdge = null;
            _nextEdge = null;
            _preEdge = null;
            _oppEdge = null;
        }
    }
}
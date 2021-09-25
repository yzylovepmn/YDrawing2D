using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEdge : IHEMeshNode
    {
        public HEdge()
        {

        }

        public HEdge(int id, HEVertex head, HEVertex tail, HEEdge relative)
        {
            _id = id;
            _head = head;
            _tail = tail;
            _relative = relative;
        }

        private int _id = HEMesh.InvaildID;
        private HEVertex _head;
        private HEVertex _tail;
        private HEEdge _relative;

        public int ID { get { return _id; } internal set { _id = value; } }

        public bool IsIsolated { get { return _relative.IsIsolated; } }

        public bool IsBoundary { get { return _relative.IsBoundary; } }

        public HEVertex Head { get { return _head; } internal set { _head = value; } }

        public HEVertex Tail { get { return _tail; } internal set { _tail = value; } }

        internal HEEdge Relative { get { return _relative; } set { _relative = value; } }

        public void Dispose()
        {
            _id = HEMesh.InvaildID;
            _head = null;
            _tail = null;
            _relative = null;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure.HalfEdge
{
    public class HEVertex : IHEMeshNode
    {
        internal HEVertex() { }

        internal HEVertex(int id, Vector3D position, HEEdge outerGoing)
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

        public bool IsBoundary
        {
            get
            {
                if (IsIsolated) return true;
                var edge = _outerGoing;
                while (true)
                {
                    if (edge.IsBoundary) return true;
                    edge = edge.RotateNext;
                    if (edge == _outerGoing)
                        break;
                }
                return false;
            }
        }

        internal void AdjustOutGoingToBoundary()
        {
            if (IsIsolated) return;
            var edge = _outerGoing;
            while (true)
            {
                if (edge.IsBoundary)
                {
                    _outerGoing = edge;
                    break;
                }
                else edge = edge.RotateNext;
                if (edge == _outerGoing)
                    break;
            }
        }

        /// <summary>
        /// ccw
        /// </summary>
        public List<HEVertex> GetAdjacentVertice()
        {
            var neighbors = new List<HEVertex>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                do
                {
                    neighbors.Add(edge.GoingTo);
                    edge = edge.RotateNext;
                }
                while (edge != _outerGoing);
            }
            return neighbors;
        }

        /// <summary>
        /// ccw
        /// </summary>
        public List<HEdge> GetAdjacentEdges()
        {
            var neighbors = new List<HEdge>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                do
                {
                    neighbors.Add(edge.RelativeEdge);
                    edge = edge.RotateNext;
                }
                while (edge != _outerGoing);
            }
            return neighbors;
        }

        /// <summary>
        /// ccw
        /// </summary>
        internal List<HEEdge> GetAdjacentHalfEdges()
        {
            var neighbors = new List<HEEdge>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                do
                {
                    neighbors.Add(edge);
                    edge = edge.RotateNext;
                }
                while (edge != _outerGoing);
            }
            return neighbors;
        }

        /// <summary>
        /// ccw
        /// </summary>
        public List<HEFace> GetAdjacentFaces()
        {
            var neighbors = new List<HEFace>();
            if (!IsIsolated)
            {
                var edge = _outerGoing;
                do
                {
                    if (edge.RelativeFace != null)
                        neighbors.Add(edge.RelativeFace);
                    edge = edge.RotateNext;
                }
                while (edge != _outerGoing);
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
using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.DataStructure.HalfEdge
{
    public interface IHEMeshNode : IDisposable
    {
        int ID { get; }

        bool IsIsolated { get; }

        bool IsBoundary { get; }
    }
}
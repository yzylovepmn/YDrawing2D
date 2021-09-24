using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure
{
    public interface IPointSet
    {
        bool HasNormals { get; }

        int VertexCount { get; }

        Vector3D GetVertex(int vid);

        Vector3D GetNormal(int vid);
    }
}
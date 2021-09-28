using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure
{
    public interface IPointSet
    {
        bool HasVerticeNormals { get; }

        int VertexCount { get; }

        Vector3D GetPosition(int vid);

        Vector3D GetVertexNormal(int vid);
    }
}
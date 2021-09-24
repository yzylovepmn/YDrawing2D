using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure
{
    public interface IMesh : IPointSet
    {
        int TriangleCount { get; }

        IndexN<int> GetTriangle(int fid);

        Vector3D GetTriangleNormal(int fid);
    }
}
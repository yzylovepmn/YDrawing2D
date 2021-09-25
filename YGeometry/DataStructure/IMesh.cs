using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.DataStructure
{
    public interface IMesh : IPointSet
    {
        int FaceCount { get; }

        IndexN<int> GetVerticeOfFace(int fid);

        IndexN<int> GetEdgesOfFace(int fid);

        IndexN<int> GetNeighborOfFace(int fid);

        Vector3D GetFaceNormal(int fid);

        IndexN<int> GetNeighborVerticeOfVertex(int vid);
    }
}
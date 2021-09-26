using System;
using System.Collections.Generic;
using System.Text;
using YGeometry.Maths;

namespace YGeometry.IO
{
    public struct VertexData
    {
        public Vector3D Position { get; set; }

        public Vector3D? Normal { get; set; }
    }

    public struct FaceData
    {
        // ccw
        public IndexN<int> Vertices { get; set; }

        public Vector3D? Normal { get; set; }
    }

    public class MeshData
    {
        public List<VertexData> Vertices { get; set; }

        public List<FaceData> Faces { get; set; }
    }
}
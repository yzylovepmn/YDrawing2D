using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public struct PointMesh : IMesh
    {
        public PointMesh(Point3F point)
        {
            Point = point;
        }

        public Point3F Point;

        public MeshType Type { get { return MeshType.Point; } }

        public Point3F GetPoint()
        {
            return Point;
        }
    }
}
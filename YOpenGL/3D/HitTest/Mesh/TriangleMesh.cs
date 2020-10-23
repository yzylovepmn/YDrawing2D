using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public struct TriangleMesh : IMesh
    {
        public TriangleMesh(Point3F p1, Point3F p2, Point3F p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Point3F P1;
        public Point3F P2;
        public Point3F P3;

        public MeshType Type { get { return MeshType.Triangle; } }
    }
}
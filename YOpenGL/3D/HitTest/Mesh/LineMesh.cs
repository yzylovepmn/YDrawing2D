using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public struct LineMesh : IMesh
    {
        public LineMesh(Point3F p1, Point3F p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Point3F P1;
        public Point3F P2;

        public MeshType Type { get { return MeshType.Line; } }
    }
}
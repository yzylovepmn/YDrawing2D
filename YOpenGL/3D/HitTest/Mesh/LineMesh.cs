using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public struct LineMesh : IMesh
    {
        public LineMesh(Point3F p1, Point3F p2, int index1, int index2, float weight1, float weight2)
        {
            P1 = p1;
            P2 = p2;
            Index1 = index1;
            Index2 = index2;
            Weight1 = weight1;
            Weight2 = weight2;
        }

        public Point3F P1;
        public Point3F P2;

        public int Index1;
        public int Index2;

        public float Weight1;
        public float Weight2;

        public MeshType Type { get { return MeshType.Line; } }

        public Point3F GetPoint()
        {
            return GeometryHelper.Combine(P1, P2, Weight1, Weight2);
        }
    }
}
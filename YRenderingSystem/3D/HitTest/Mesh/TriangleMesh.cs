using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public struct TriangleMesh : IMesh
    {
        public TriangleMesh(Point3F p1, Point3F p2, Point3F p3, int index1, int index2, int index3, float weight1, float weight2, float weight3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
            Weight1 = weight1;
            Weight2 = weight2;
            Weight3 = weight3;
        }

        public Point3F P1;
        public Point3F P2;
        public Point3F P3;

        public int Index1;
        public int Index2;
        public int Index3;

        public float Weight1;
        public float Weight2;
        public float Weight3;

        public MeshType Type { get { return MeshType.Triangle; } }

        public Point3F GetPoint()
        {
            return GeometryHelper.Combine(P1, P2, P3, Weight1, Weight2, Weight3);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class Math3DHelper
    {
        public static RectF Transform(Rect3F rect, GLPanel3D viewport)
        {
            var p1 = rect.Location;
            var p2 = p1 + new Vector3F(rect.SizeX, 0, 0);
            var p3 = p1 + new Vector3F(0, rect.SizeY, 0);
            var p4 = p1 + new Vector3F(0, 0, rect.SizeZ);
            var p5 = p1 + new Vector3F(rect.SizeX, rect.SizeY, 0);
            var p6 = p1 + new Vector3F(rect.SizeX, 0, rect.SizeZ);
            var p7 = p1 + new Vector3F(0, rect.SizeY, rect.SizeZ);
            var p8 = p1 + new Vector3F(rect.SizeX, rect.SizeY, rect.SizeZ);

            var ret = RectF.Empty;
            ret.Union(viewport.Point3DToPointInWpf(p1));
            ret.Union(viewport.Point3DToPointInWpf(p2));
            ret.Union(viewport.Point3DToPointInWpf(p3));
            ret.Union(viewport.Point3DToPointInWpf(p4));
            ret.Union(viewport.Point3DToPointInWpf(p5));
            ret.Union(viewport.Point3DToPointInWpf(p6));
            ret.Union(viewport.Point3DToPointInWpf(p7));
            ret.Union(viewport.Point3DToPointInWpf(p8));
            return ret;
        }
    }
}
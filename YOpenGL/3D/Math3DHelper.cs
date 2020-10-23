using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public class Math3DHelper
    {
        public static RectF Transform(Rect3F rect, Matrix3F transform)
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
            ret.Union((PointF)(p1 * transform));
            ret.Union((PointF)(p2 * transform));
            ret.Union((PointF)(p3 * transform));
            ret.Union((PointF)(p4 * transform));
            ret.Union((PointF)(p5 * transform));
            ret.Union((PointF)(p6 * transform));
            ret.Union((PointF)(p7 * transform));
            ret.Union((PointF)(p8 * transform));
            return ret;
        }
    }
}
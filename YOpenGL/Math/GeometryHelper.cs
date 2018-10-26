using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YOpenGL
{
    public static class GeometryHelper
    {
        public static bool IsArcContain(Arc arc, PointF point)
        {
            var vec = point - arc.Center;
            var radian = (float)Math.Atan(vec.Y / vec.X);
            if (vec.X < 0)
                radian += (float)Math.PI;
            FormatRadian(ref radian);
            var startRadian = arc.StartRadian;
            var endRadian = arc.EndRadian;
            if (startRadian < endRadian)
            {
                endRadian -= 2 * (float)Math.PI;
                if (radian > startRadian)
                    radian -= 2 * (float)Math.PI;
            }
            return radian >= endRadian && radian <= startRadian;
        }

        public static void FormatRadian(ref float radian)
        {
            var _radian = (double)radian;

            var pi2 = 2 * Math.PI;
            while (_radian < 0)
                _radian += pi2;
            while (_radian > pi2)
                _radian -= pi2;

            radian = (float)_radian;
        }

        public static void FormatAngle(ref float angle)
        {
            var _angle = (double)angle;

            while (_angle < 0)
                _angle += 360;
            while (_angle > 360)
                _angle -= 360;

            angle = (float)_angle;
        }

        /// <summary>
        /// Generate points clockwise(In order to get more accurate results, no matrix rotation is used)
        /// </summary>
        public static IEnumerable<PointF> GenArcPoints(float deltaAngle, float startAngle, float wAngle)
        {
            if (MathUtil.IsZero(deltaAngle))
                throw new ArgumentOutOfRangeException("deltaAngle can not be zero!");

            FormatAngle(ref wAngle);
            deltaAngle = Math.Abs(deltaAngle);

            var fcnt = wAngle / deltaAngle;
            var cnt = (int)fcnt;

            var curRadian = GetRadian(startAngle);
            var deltaRadian = GetRadian(deltaAngle);
            for (int i = 0; i <= cnt; i++)
            {
                yield return (PointF)new Point(Math.Cos(curRadian), Math.Sin(curRadian));
                curRadian += deltaRadian;
            }

            if (fcnt > cnt)
                yield return (PointF)new Point(Math.Cos(GetRadian(startAngle + wAngle)), Math.Sin(GetRadian(startAngle + wAngle)));
        }

        public static float GetRadian(float angle)
        {
            return (float)(angle * Math.PI / 180);
        }
    }
}
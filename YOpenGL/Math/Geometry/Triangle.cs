using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public struct Triangle
    {
        public Triangle(PointF p1, PointF p2, PointF p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public PointF P1;
        public PointF P2;
        public PointF P3;

        public bool IsCompletelyInside(RectF rect)
        {
            return rect.Contains(P1) && rect.Contains(P2) && rect.Contains(P3);
        }

        public bool IsRectCompletelyInside(RectF rect)
        {
            return IsPointInside(rect.TopLeft) && IsPointInside(rect.TopRight)
                   && IsPointInside(rect.BottomLeft) && IsPointInside(rect.BottomRight);
        }

        public bool IsPointInside(PointF p)
        {
            var dx = p.X - P3.X;
            var dx1 = P1.X - P3.X;
            var dx2 = P2.X - P3.X;
            var dy = p.Y - P3.Y;
            var dy1 = P1.Y - P3.Y;
            var dy2 = P2.Y - P3.Y;

            var ac = dx * dy2 - dx2 * dy;
            var bc = -(dx * dy1 - dx1 * dy);

            if ((ac < 0) != (bc < 0)) return false;

            var c = dx1 * dy2 - dx2 * dy1;
            if (c < 0)
            {
                c = -c;
                ac = -ac;
                bc = -bc;
            }

            return ac >= 0 && ac + bc <= c;
        }

        /// <summary>
        /// 计算重心坐标
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void CalcBarycentric(PointF p, out float a, out float b)
        {
            var dx = p.X - P3.X;
            var dx1 = P1.X - P3.X;
            var dx2 = P2.X - P3.X;
            var dy = p.Y - P3.Y;
            var dy1 = P1.Y - P3.Y;
            var dy2 = P2.Y - P3.Y;

            var ac = dx * dy2 - dx2 * dy;
            var bc = -(dx * dy1 - dx1 * dy);
            var c = dx1 * dy2 - dx2 * dy1;
            a = ac / c;
            b = bc / c;
        }

        public bool IntersectsWith(RectF rect)
        {
            return Line.AreLineSegmentsIntersecting(P1, P2, rect.BottomLeft, rect.BottomRight)
                   || Line.AreLineSegmentsIntersecting(P1, P2, rect.BottomLeft, rect.TopLeft)
                   || Line.AreLineSegmentsIntersecting(P1, P2, rect.TopLeft, rect.TopRight)
                   || Line.AreLineSegmentsIntersecting(P1, P2, rect.TopRight, rect.BottomRight)
                   || Line.AreLineSegmentsIntersecting(P2, P3, rect.BottomLeft, rect.BottomRight)
                   || Line.AreLineSegmentsIntersecting(P2, P3, rect.BottomLeft, rect.TopLeft)
                   || Line.AreLineSegmentsIntersecting(P2, P3, rect.TopLeft, rect.TopRight)
                   || Line.AreLineSegmentsIntersecting(P2, P3, rect.TopRight, rect.BottomRight)
                   || Line.AreLineSegmentsIntersecting(P3, P1, rect.BottomLeft, rect.BottomRight)
                   || Line.AreLineSegmentsIntersecting(P3, P1, rect.BottomLeft, rect.TopLeft)
                   || Line.AreLineSegmentsIntersecting(P3, P1, rect.TopLeft, rect.TopRight)
                   || Line.AreLineSegmentsIntersecting(P3, P1, rect.TopRight, rect.BottomRight);
        }
    }
}
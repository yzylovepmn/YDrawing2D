using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public enum LineType
    {
        Segment,
        Ray,
        Line
    }

    public struct Line
    {
        public Line(PointF p1, PointF p2, LineType type = LineType.Segment)
        {
            P1 = p1;
            P2 = p2;
            GeometryHelper.CalcABC(p1, p2, out A, out B, out C);
            Type = type;
        }

        public Line(PointF p1, VectorF vec)
        {
            P1 = p1;
            P2 = p1 + vec;
            GeometryHelper.CalcABC(P1, P2, out A, out B, out C);
            Type = LineType.Ray;
        }

        public PointF P1;
        public PointF P2;
        public float A;
        public float B;
        public float C;
        public LineType Type;

        internal RectF GetBounds()
        {
            return new RectF(P1, P2);
        }

        internal bool HitTest(PointF p, float sensitive)
        {
            return Distance(p) < sensitive;
        }

        internal bool HitTest(RectF rect)
        {
            if (A == 0 || B == 0)
                return true;

            var p1 = B > 0 ? rect.TopLeft : rect.BottomLeft;
            var p2 = B > 0 ? rect.BottomRight : rect.TopRight;
            var v1 = CalcSymbol(p1);
            var v2 = CalcSymbol(p2);
            return !MathUtil.IsSameSymbol(v1, v2);
        }

        internal bool HitTest(RectF rect, float lineWidth)
        {
            if (A == 0 || B == 0)
                return true;

            var p1 = B > 0 ? rect.TopLeft : rect.BottomLeft;
            var p2 = B > 0 ? rect.BottomRight : rect.TopRight;
            var v1 = CalcSymbol(p1);
            var v2 = CalcSymbol(p2);
            var ret = !MathUtil.IsSameSymbol(v1, v2);
            if (!ret)
            {
                var distance = lineWidth * (float)Math.Sqrt(A * A + B * B);
                v1 = Math.Abs(v1);
                v2 = Math.Abs(v2);
                return v1 <= distance || v2 <= distance;
            }
            return true;
        }

        public float Distance(PointF p)
        {
            if (A == 0 && B == 0)
                return (p - P1).Length;
            else if (B == 0)
                return Math.Abs(p.X - P1.X);
            else if (A == 0)
                return Math.Abs(p.Y - P1.Y);
            else return Math.Abs(CalcSymbol(p)) / (float)Math.Sqrt(A * A + B * B);
        }

        internal float CalcLength(float symbol)
        {
            return Math.Abs(symbol) / (float)Math.Sqrt(A * A + B * B);
        }

        internal float CalcSymbol(PointF p)
        {
            return A * p.X + B * p.Y + C;
        }

        public static bool AreLineSegmentsIntersecting(PointF a1, PointF a2, PointF b1, PointF b2)
        {
            if (b1 == b2 || a1 == a2)
            {
                return false;
            }

            if ((((a2.X - a1.X) * (b1.Y - a1.Y)) - ((b1.X - a1.X) * (a2.Y - a1.Y)))
                * (((a2.X - a1.X) * (b2.Y - a1.Y)) - ((b2.X - a1.X) * (a2.Y - a1.Y))) > 0)
            {
                return false;
            }

            if ((((b2.X - b1.X) * (a1.Y - b1.Y)) - ((a1.X - b1.X) * (b2.Y - b1.Y)))
                * (((b2.X - b1.X) * (a2.Y - b1.Y)) - ((a2.X - b1.X) * (b2.Y - b1.Y))) > 0)
            {
                return false;
            }

            return true;
        }

        public bool IntersectsWith(Line other)
        {
            return AreLineSegmentsIntersecting(P1, P2, other.P1, other.P2);
        }

        public PointF? Cross(Line other)
        {
            var ret = default(PointF?);

            var d = A * other.B - B * other.A;
            if (d != 0)
            {
                var p = new PointF();
                p.X = (other.C * B - C * other.B) / d;
                p.Y = -(other.C * A - C * other.A) / d;
                ret = p;
            }

            return ret;
        }

        public PointF? Projection(PointF p)
        {
            var vec = new VectorF(A, B);
            var cps = Cross(new Line(p, vec));
            return cps;
        }

        /// <summary>
        /// <see cref="p"/> = t * <see cref="P1"/> + (1 - t) * <see cref="P2"/>
        /// </summary>
        /// <param name="p"></param>
        public float CalcT(PointF p)
        {
            var sum = P2.X + P2.Y;
            var a = p.X + p.Y - sum;
            var b = P1.X + P1.Y - sum;
            return a / b;
        }
    }
}
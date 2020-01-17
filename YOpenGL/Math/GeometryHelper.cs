using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YOpenGL
{
    public struct CLine
    {
        public CLine(PointF p1, PointF p2)
        {
            P1 = p1;
            P2 = p2;
            GeometryHelper.CalcABC(p1, p2, out A, out B, out C);
        }

        public PointF P1;
        public PointF P2;
        public float A;
        public float B;
        public float C;

        public RectF GetBounds()
        {
            return new RectF(P1, P2);
        }

        public bool HitTest(PointF p, float sensitive)
        {
            return CalcLength(p) < sensitive;
        }

        public bool HitTest(RectF rect)
        {
            if (A == 0 && B == 0)
                return true;
            if (B > 0)
            {
                var p1 = rect.TopLeft;
                var p2 = rect.BottomRight;
                var v1 = CalcSymbol(p1);
                var v2 = CalcSymbol(p2);
                return !MathUtil.IsSameSymbol(v1, v2);
            }
            else if (B < 0)
            {
                var p1 = rect.BottomLeft;
                var p2 = rect.TopRight;
                var v1 = CalcSymbol(p1);
                var v2 = CalcSymbol(p2);
                return !MathUtil.IsSameSymbol(v1, v2);
            }
            return true;
        }

        public float CalcLength(PointF p)
        {
            if (A == 0 && B == 0)
                return (p - P1).Length;
            else if (B == 0)
                return Math.Abs(p.X - P1.X);
            else if (A == 0)
                return Math.Abs(p.Y - P1.Y);
            else return Math.Abs(CalcSymbol(p)) / (float)Math.Sqrt(A * A + B * B);
        }

        public float CalcLength(float symbol)
        {
            return Math.Abs(symbol) / (float)Math.Sqrt(A * A + B * B);
        }

        public float CalcSymbol(PointF p)
        {
            return A * p.X + B * p.Y + C;
        }
    }

    public static class GeometryHelper
    {
        internal readonly static PointF[] UnitCicle;
        internal readonly static float DeltaRadian;

        static GeometryHelper()
        {
            DeltaRadian = GetRadian(360 / 64f);
            UnitCicle = GenCiclePoints().ToArray();
        }

        internal static void CalcABC(PointF p1, PointF p2, out float A, out float B, out float C)
        {
            var deltaY = p2.Y - p1.Y;
            var deltaX = p2.X - p1.X;
            var k = deltaY / deltaX;
            if (float.IsNaN(k))
            {
                A = 0;
                B = 0;
                C = 0;
            }
            else if (float.IsInfinity(k))
            {
                A = 1;
                B = 0;
                C = -p1.X;
            }
            else
            {
                var b = p1.Y - k * p1.X;
                A = k;
                B = -1;
                C = b;
                if (A < 0)
                {
                    A = -A;
                    B = -B;
                    C = -C;
                }
            }
        }

        internal static bool IsPossibleArcContain(_Arc arc, PointF point)
        {
            var vec = point - arc.Center;
            var radian = (float)Math.Atan2(vec.Y, vec.X);
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

        internal static bool IsIntersect(_Arc arc, RectF rect, float v1, float v2, float v3, float v4)
        {
            var start = new PointF((float)Math.Cos(arc.StartRadian) * arc.Radius + arc.Center.X, (float)Math.Sin(arc.StartRadian) * arc.Radius + arc.Center.Y);
            var end = new PointF((float)Math.Cos(arc.EndRadian) * arc.Radius + arc.Center.X, (float)Math.Sin(arc.EndRadian) * arc.Radius + arc.Center.Y);

            if (arc.StartRadian < Math.PI / 2)
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    if (arc.StartRadian < arc.EndRadian)
                    {
                        if (v1 < arc.Radius)
                            return (rect.X < end.X && (rect.Bottom > end.Y || v2 > arc.Radius))
                                    || (rect.Y < start.Y && (rect.Right > start.X || v3 > arc.Radius));
                        return true;
                    }
                    else return true;
                }
                else if (arc.EndRadian < Math.PI)
                {
                    if (start.Y > end.Y)
                    {
                        if (v1 < arc.Radius)
                            return v3 > arc.Radius || (v4 > arc.Radius && rect.Right > start.X) || (v2 > arc.Radius && rect.X < end.X);
                        else
                        {
                            if (rect.X < arc.Center.X && rect.Y > arc.Center.Y)
                                return rect.Y < end.Y || rect.Right > start.X;
                            return true;
                        }
                    }
                    else
                    {
                        if (v1 < arc.Radius)
                            return (v2 > arc.Radius && rect.X < end.X) || (v4 > arc.Radius && rect.Right > start.X && rect.Y < start.Y) || (v3 > arc.Radius && rect.Y < start.Y);
                        else
                        {
                            if (rect.X < arc.Center.X && rect.Y > arc.Center.Y)
                                return rect.Y < end.Y;
                            return true;
                        }
                    }
                }
                else return true;
            }
            else if (arc.StartRadian < Math.PI)
            {
                if (arc.EndRadian < Math.PI / 2)
                    return true;
                else if (arc.EndRadian < Math.PI)
                {
                    if (arc.StartRadian > arc.EndRadian)
                        return true;
                    else
                    {
                        if (v1 < arc.Radius)
                            return v3 > arc.Radius || (v4 > arc.Radius && rect.Right > start.X) || (v2 > arc.Radius && rect.Left < end.X);
                        else
                        {
                            if (rect.X < arc.Center.X && rect.Y > arc.Center.Y)
                                return rect.Y < end.Y || (v4 > arc.Radius && rect.Right > start.X);
                            return true;
                        }
                    }
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    if (start.X > end.X)
                    {
                        if (v2 > arc.Radius)
                        {
                            if (rect.Bottom > arc.Center.Y && rect.X < arc.Center.X)
                                return (v1 > arc.Radius && rect.Y < end.Y) || ((v4 > arc.Radius || rect.Bottom > start.Y) && rect.Right > start.X);
                            else return true;
                        }
                        else return true;
                    }
                    else
                    {
                        if (v1 > arc.Radius)
                        {
                            if (rect.X < arc.Center.X && rect.Y < arc.Center.Y)
                                return (v2 > arc.Radius && rect.Bottom > start.Y) || (rect.Right > end.X && (rect.Y < end.Y || v3 > arc.Radius));
                            return true;
                        }
                        else return true;
                    }
                }
                else return true;
            }
            else if (arc.StartRadian < Math.PI * 1.5)
            {
                if (arc.EndRadian < Math.PI)
                    return true;
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    if (arc.StartRadian > arc.EndRadian)
                        return true;
                    else
                    {
                        if (v1 > arc.Radius)
                            return ((v2 > arc.Radius || rect.Left < start.X) && rect.Bottom > start.Y) || ((v3 > arc.Radius || rect.Y < end.Y) && rect.Right > end.X);
                        else return true;
                    }
                }
                else
                {
                    if (start.Y > end.Y)
                    {
                        if (v1 > arc.Radius)
                            return (rect.Bottom > start.Y && (rect.X < start.X || v2 > arc.Radius)) || (rect.Right > end.X && v3 > arc.Radius);
                        else return true;
                    }
                    else
                    {
                        if (v3 > arc.Radius)
                            return (rect.X < start.X && v1 > arc.Radius) || (rect.Bottom > end.Y && (rect.Right > end.X || v4 > arc.Radius));
                        else return true;
                    }
                }
            }
            else
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    if (start.X < end.X)
                    {
                        if (v3 > arc.Radius)
                            return (rect.X < start.X && (v1 > arc.Radius || rect.Y < start.Y)) || (rect.Bottom > end.Y && v4 > arc.Radius);
                        else return true;
                    }
                    else
                    {
                        if (v4 > arc.Radius)
                            return (rect.X < end.X && (rect.Bottom > end.Y || v2 > arc.Radius)) || (rect.Y < start.Y && v3 > arc.Radius);
                        else return true;
                    }
                }
                else if (arc.EndRadian < Math.PI)
                {
                    if (v4 > arc.Radius)
                        return (rect.X < end.X && v2 > arc.Radius) || (rect.Y < start.Y && v3 > arc.Radius);
                    else return true;
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                    return true;
                else
                {
                    if (arc.StartRadian > arc.EndRadian)
                        return true;
                    else
                    {
                        if (v3 > arc.Radius)
                            return (rect.X < start.X && (rect.Y < start.Y || v1 > arc.Radius)) || (rect.Bottom > end.Y && (rect.Right > end.X || v4 > arc.Radius));
                        else return true;
                    }
                }
            }
        }

        internal static RectF CalcBounds(_Arc arc)
        {
            var rect = new RectF();
            var start = new PointF((float)Math.Cos(arc.StartRadian) * arc.Radius + arc.Center.X, (float)Math.Sin(arc.StartRadian) * arc.Radius + arc.Center.Y);
            var end = new PointF((float)Math.Cos(arc.EndRadian) * arc.Radius + arc.Center.X, (float)Math.Sin(arc.EndRadian) * arc.Radius + arc.Center.Y);

            if (arc.StartRadian < Math.PI / 2)
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    if (arc.StartRadian < arc.EndRadian)
                    {
                        rect.X = arc.Center.X - arc.Radius;
                        rect.Y = arc.Center.Y - arc.Radius;
                        rect.Width = arc.Radius + arc.Radius;
                        rect.Height = rect.Width;
                    }
                    else rect = new RectF(start, end);
                }
                else if (arc.EndRadian < Math.PI)
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = arc.Radius + arc.Radius;
                    rect.Height = Math.Max(start.Y, end.Y) - rect.Y;
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    rect.X = end.X;
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = arc.Radius + arc.Center.X - rect.X;
                    rect.Height = start.Y - rect.Y;
                }
                else
                {
                    rect.X = Math.Min(end.X, start.X);
                    rect.Y = end.Y;
                    rect.Width = arc.Radius + arc.Center.X - rect.X;
                    rect.Height = start.Y - rect.Y;
                }
            }
            else if (arc.StartRadian < Math.PI)
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    rect.X = start.X;
                    rect.Y = Math.Min(end.Y, start.Y);
                    rect.Width = end.X - start.X;
                    rect.Height = arc.Center.Y + arc.Radius - rect.Y;
                }
                else if (arc.EndRadian < Math.PI)
                {
                    if (arc.StartRadian > arc.EndRadian)
                        rect = new RectF(start, end);
                    else
                    {
                        rect.X = arc.Center.X - arc.Radius;
                        rect.Y = arc.Center.Y - arc.Radius;
                        rect.Width = arc.Radius + arc.Radius;
                        rect.Height = rect.Width;
                    }
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    rect.X = Math.Min(start.X, end.X);
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = arc.Radius + arc.Center.X - rect.X;
                    rect.Height = arc.Radius + arc.Radius;
                }
                else
                {
                    rect.X = start.X;
                    rect.Y = end.Y;
                    rect.Width = arc.Radius + arc.Center.X - rect.X;
                    rect.Height = arc.Radius + arc.Center.Y - rect.Y;
                }
            }
            else if (arc.StartRadian < Math.PI * 1.5)
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = start.Y;
                    rect.Width = end.X - rect.X;
                    rect.Height = arc.Radius + arc.Center.Y - rect.Y;
                }
                else if (arc.EndRadian < Math.PI)
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = start.Y;
                    rect.Width = Math.Max(end.X, start.X) - rect.X;
                    rect.Height = end.Y - rect.Y;
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    if (arc.StartRadian > arc.EndRadian)
                        rect = new RectF(start, end);
                    else
                    {
                        rect.X = arc.Center.X - arc.Radius;
                        rect.Y = arc.Center.Y - arc.Radius;
                        rect.Width = arc.Radius + arc.Radius;
                        rect.Height = rect.Width;
                    }
                }
                else
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = Math.Min(start.Y, end.Y);
                    rect.Width = arc.Radius + arc.Radius;
                    rect.Height = arc.Center.Y + arc.Radius - rect.Y;
                }
            }
            else
            {
                if (arc.EndRadian < Math.PI / 2)
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = Math.Max(start.X, end.X) - rect.X;
                    rect.Height = arc.Radius + arc.Radius;
                }
                else if (arc.EndRadian < Math.PI)
                {
                    rect.X = arc.Center.X - arc.Radius;
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = start.X - rect.X;
                    rect.Height = end.Y - rect.Y;
                }
                else if (arc.EndRadian < Math.PI * 1.5)
                {
                    rect.X = end.X;
                    rect.Y = arc.Center.Y - arc.Radius;
                    rect.Width = start.X - rect.X;
                    rect.Height = Math.Max(start.Y, end.Y) - rect.Y;
                }
                else
                {
                    if (arc.StartRadian > arc.EndRadian)
                        rect = new RectF(start, end);
                    else
                    {
                        rect.X = arc.Center.X - arc.Radius;
                        rect.Y = arc.Center.Y - arc.Radius;
                        rect.Width = arc.Radius + arc.Radius;
                        rect.Height = rect.Width;
                    }
                }
            }
            return rect;
        }

        internal static void CalcArcRadian(PointF start, PointF end, float radius, bool isLargeAngle, bool isClockwise, out PointF center, out float startRadian, out float endRadian)
        {
            var vec = end - start;
            if (vec.Length <= radius * 2)
            {
                var normal = new VectorF(vec.Y, -vec.X);
                normal.Normalize();
                center = new PointF((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                var len = normal * (float)Math.Sqrt(radius * radius - vec.LengthSquared / 4);
                if (isLargeAngle ^ isClockwise)
                    center += len;
                else center -= len;
                if (!isClockwise)
                    MathUtil.Switch(ref start, ref end);

                startRadian = GetRadian(center, start);
                endRadian = GetRadian(center, end);
            }
            else
            {
                center = new PointF();
                startRadian = 0;
                endRadian = 0;
            }
        }

        internal static float GetRadian(PointF center, PointF p)
        {
            var vec = p - center;
            var radian = (float)Math.Atan2(vec.Y, vec.X);
            FormatRadian(ref radian);
            return radian;
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
        internal static IEnumerable<PointF> GenCiclePoints()
        {
            var cnt = 64;

            var curRadian = 0f;
            for (int i = 0; i <= cnt; i++)
            {
                yield return (PointF)new Point(Math.Cos(curRadian), Math.Sin(curRadian));
                curRadian += DeltaRadian;
            }
        }

        internal static IEnumerable<PointF> GenArcPoints(float startRadian, float endRadian, bool needReverse = true)
        {
            var points = new List<PointF>();
            float curRadian = 0;
            bool flag = false, isAfter = startRadian > endRadian, needRestart = endRadian > 0;
            if (!isAfter)
                startRadian += (float)(2 * Math.PI);
            for (int i = 0; i < 65;)
            {
                if (!flag)
                {
                    if (endRadian <= curRadian)
                    {
                        flag = true;
                        if (endRadian < curRadian)
                            points.Add(new PointF((float)Math.Cos(endRadian), (float)Math.Sin(endRadian)));
                        if (curRadian < startRadian)
                            points.Add(UnitCicle[i]);
                        else if (curRadian >= startRadian)
                        {
                            points.Add(new PointF((float)Math.Cos(startRadian), (float)Math.Sin(startRadian)));
                            break;
                        }
                    }
                }
                else
                {
                    if (curRadian < startRadian)
                        points.Add(UnitCicle[i]);
                    else if (curRadian >= startRadian)
                    {
                        points.Add(new PointF((float)Math.Cos(startRadian), (float)Math.Sin(startRadian)));
                        break;
                    }
                }
                curRadian += DeltaRadian;
                i++;
                if (i == 65 && needRestart)
                    i = 1;
            }
            if (needReverse)
                points.Reverse();
            return points;
        }

        public static float GetRadian(float angle)
        {
            return (float)(angle * Math.PI / 180);
        }

        public static float GetAngle(float radian)
        {
            return (float)(radian * 180 / Math.PI);
        }

        internal static float GetLength(IEnumerable<PointF> points)
        {
            var len = 0f;
            var flag = true;
            var last = default(PointF);
            foreach (var p in points)
            {
                if (flag)
                {
                    flag = false;
                    last = p;
                }
                else
                {
                    len += (p - last).Length;
                    last = p;
                }
            }
            return len;
        }

        public static bool Contains(_SimpleGeometry geo, PointF point)
        {
            int leftpass = 0, toppass = 0, rightpass = 0, bottompass = 0;
            var primitives = new List<IPrimitive>();
            foreach (var primitive in geo.Stream)
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                    case PrimitiveType.Arc:
                        primitives.Add(primitive);
                        break;
                    case PrimitiveType.Bezier:
                        primitives.AddRange(((_Bezier)primitive).InnerLines.Cast<IPrimitive>());
                        break;
                }
            }

            if (geo.UnClosedLine != null)
                primitives.Add(geo.UnClosedLine);

            foreach (var primitive in primitives)
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                        var line = (_Line)primitive;
                        if (line.Line.P1.X > point.X)
                        {
                            if (line.Line.P1.Y > point.Y)
                            {
                                if (line.Line.P2.X > point.X)
                                {
                                    // 1 1
                                    if (line.Line.P2.Y > point.Y)
                                        continue;
                                    else
                                    {
                                        // 1 4
                                        rightpass++;
                                    }
                                }
                                else
                                {
                                    // 1 2
                                    if (line.Line.P2.Y > point.Y)
                                    {
                                        toppass++;
                                    }
                                    else
                                    {
                                        // 1 3
                                        var v = line.Line.CalcSymbol(point);
                                        if (v > 0)
                                        {
                                            toppass++;
                                            leftpass++;
                                        }
                                        if (v < 0)
                                        {
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (line.Line.P2.X > point.X)
                                {
                                    if (line.Line.P2.Y > point.Y)
                                    {
                                        // 4 1
                                        rightpass++;
                                    }
                                    else
                                    {
                                        // 4 4
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (line.Line.P2.Y > point.Y)
                                    {
                                        // 4 2
                                        var v = line.Line.CalcSymbol(point);
                                        if (v > 0)
                                        {
                                            bottompass++;
                                            leftpass++;
                                        }
                                        if (v < 0)
                                        {
                                            rightpass++;
                                            toppass++;
                                        }
                                    }
                                    else
                                    {
                                        // 4 3
                                        bottompass++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (line.Line.P1.Y > point.Y)
                            {
                                if (line.Line.P2.X > point.X)
                                {
                                    // 2 1
                                    if (line.Line.P2.Y > point.Y)
                                        toppass++;
                                    else
                                    {
                                        // 2 4
                                        var v = line.Line.CalcSymbol(point);
                                        if (v > 0)
                                        {
                                            bottompass++;
                                            leftpass++;
                                        }
                                        if (v < 0)
                                        {
                                            rightpass++;
                                            toppass++;
                                        }
                                    }
                                }
                                else
                                {
                                    // 2 2
                                    if (line.Line.P2.Y > point.Y)
                                        continue;
                                    else
                                    {
                                        // 2 3
                                        leftpass++;
                                    }
                                }
                            }
                            else
                            {
                                if (line.Line.P2.X > point.X)
                                {
                                    if (line.Line.P2.Y > point.Y)
                                    {
                                        // 3 1
                                        var v = line.Line.CalcSymbol(point);
                                        if (v > 0)
                                        {
                                            toppass++;
                                            leftpass++;
                                        }
                                        if (v < 0)
                                        {
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                    else
                                    {
                                        // 3 4
                                        bottompass++;
                                    }
                                }
                                else
                                {
                                    if (line.Line.P2.Y > point.Y)
                                    {
                                        // 3 2
                                        leftpass++;
                                    }
                                    else
                                    {
                                        // 3 3
                                        continue;
                                    }
                                }
                            }
                        }
                        break;
                    case PrimitiveType.Arc:
                        var arc = (_Arc)primitive;
                        var start = new PointF(arc.Center.X + arc.Radius * (float)Math.Cos(arc.StartRadian), arc.Center.Y + arc.Radius * (float)Math.Sin(arc.StartRadian));
                        var end = new PointF(arc.Center.X + arc.Radius * (float)Math.Cos(arc.EndRadian), arc.Center.Y + arc.Radius * (float)Math.Sin(arc.EndRadian));
                        if (start.X > point.X)
                        {
                            if (start.Y > point.Y)
                            {
                                if (end.X > point.X)
                                {
                                    // 1 1
                                    if (end.Y > point.Y)
                                    {
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius && (start.X - point.X) > (end.X - point.X))
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                    else
                                    {
                                        // 1 4
                                        rightpass++;
                                    }
                                }
                                else
                                {
                                    // 1 2
                                    if (end.Y > point.Y)
                                    {
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            bottompass++;
                                            rightpass++;
                                        }
                                        else if (len > arc.Radius)
                                            toppass++;
                                        else return true;
                                    }
                                    else
                                    {
                                        // 1 3
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius || arc.Center.X > point.X)
                                        {
                                            bottompass++;
                                            rightpass++;
                                        }
                                        else if (len > arc.Radius)
                                        {
                                            leftpass++;
                                            toppass++;
                                        }
                                        else return true;
                                    }
                                }
                            }
                            else
                            {
                                if (end.X > point.X)
                                {
                                    if (end.Y > point.Y)
                                    {
                                        // 4 1
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            bottompass++;
                                            toppass++;
                                        }
                                        else if (len > arc.Radius)
                                            rightpass++;
                                        else return true;
                                    }
                                    else
                                    {
                                        // 4 4
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius && (start.X - point.X) < (end.X - point.X))
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (end.Y > point.Y)
                                    {
                                        // 4 2
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius || arc.Center.X < point.X)
                                        {
                                            bottompass++;
                                            leftpass++;
                                        }
                                        else if (len > arc.Radius)
                                        {
                                            rightpass++;
                                            toppass++;
                                        }
                                        else return true;
                                    }
                                    else
                                    {
                                        // 4 3
                                        bottompass++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (start.Y > point.Y)
                            {
                                if (end.X > point.X)
                                {
                                    // 2 1
                                    if (end.Y > point.Y)
                                        toppass++;
                                    else
                                    {
                                        // 2 4
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius || arc.Center.X > point.X)
                                        {
                                            toppass++;
                                            rightpass++;
                                        }
                                        else if (len > arc.Radius)
                                        {
                                            leftpass++;
                                            bottompass++;
                                        }
                                        else return true;
                                    }
                                }
                                else
                                {
                                    // 2 2
                                    if (end.Y > point.Y)
                                    {
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius && (start.X - point.X) > (end.X - point.X))
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                    else
                                    {
                                        // 2 3
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius)
                                        {
                                            rightpass++;
                                            bottompass++;
                                            toppass++;
                                        }
                                        else if (len > arc.Radius)
                                            leftpass++;
                                        else return true;
                                    }
                                }
                            }
                            else
                            {
                                if (end.X > point.X)
                                {
                                    if (end.Y > point.Y)
                                    {
                                        // 3 1
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius || arc.Center.X < point.X)
                                        {
                                            toppass++;
                                            leftpass++;
                                        }
                                        else if (len > arc.Radius)
                                        {
                                            rightpass++;
                                            bottompass++;
                                        }
                                        else return true;
                                    }
                                    else
                                    {
                                        // 3 4
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius)
                                        {
                                            rightpass++;
                                            leftpass++;
                                            toppass++;
                                        }
                                        else if (len > arc.Radius)
                                            bottompass++;
                                        else return true;
                                    }
                                }
                                else
                                {
                                    if (end.Y > point.Y)
                                    {
                                        // 3 2
                                        leftpass++;
                                    }
                                    else
                                    {
                                        // 3 3
                                        var len = (point - arc.Center).Length;
                                        if (len < arc.Radius && (start.X - point.X) < (end.X - point.X))
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return leftpass % 2 == 1 && toppass % 2 == 1 && rightpass % 2 == 1 && bottompass % 2 == 1;
        }

        #region Indices
        internal static IEnumerable<uint> GenIndices(IPrimitive primitive, uint offset)
        {
            switch (primitive.Type)
            {
                case PrimitiveType.Arc:
                    return _GenCicleIndices(offset);
            }
            return null;
        }

        private static IEnumerable<uint> _GenCicleIndices(uint offset)
        {
            for (uint i = 1; i < 65; i++)
            {
                yield return offset;
                yield return offset + i;
                yield return offset + i + 1;
            }
        }
        #endregion

        #region Spline
        public static IEnumerable<PointF> CalcSamplePoints(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, float tolerance)
        {
            return _CalcSamplePoints(degree, knots, controlPoints, weights, fitPoints, tolerance);
        }

        public static RectF CalcBounds(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, float tolerance)
        {
            return _CalcBounds(_CalcSamplePoints(degree, knots, controlPoints, weights, fitPoints, tolerance));
        }

        private static RectF _CalcBounds(IEnumerable<PointF> samplePoints)
        {
            var bound = RectF.Empty;

            var flag = true;
            var last = default(PointF);
            foreach (var p in samplePoints)
            {
                if (flag)
                {
                    flag = false;
                    last = p;
                    bound.Union(p);
                }
                else
                {
                    bound.Union(new RectF(last, p));
                    last = p;
                }
            }

            return bound;
        }

        internal static List<_Line> CalcSampleLines(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, float tolerance)
        {
            var lines = new List<_Line>();

            var samplePoints = _CalcSamplePoints(degree, knots, controlPoints, weights, fitPoints, tolerance);

            var _samplePoints = samplePoints.ToArray();
            for (int i = 1; i < _samplePoints.Length; i++)
                lines.Add(new _Line(_samplePoints[i - 1], _samplePoints[i], PenF.NULL));

            return lines;
        }

        private static IEnumerable<PointF> _CalcSamplePoints(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, float tolerance)
        {
            var samplePoints = new List<PointF>();

            if (knots.Length == 0)
                samplePoints = fitPoints.ToList();
            else
            {
                var delta = tolerance / GetLength(controlPoints);
                if (delta > 1)
                {
                    samplePoints.Add(ComputePoint(degree, knots, controlPoints, weights, 0));
                    samplePoints.Add(ComputePoint(degree, knots, controlPoints, weights, 1));
                }
                else
                {
                    delta = Math.Max(tolerance / (8 * controlPoints.Length), delta);
                    var i = 0f;
                    while (i <= 1)
                    {
                        samplePoints.Add(ComputePoint(degree, knots, controlPoints, weights, i));
                        i += delta;
                    }
                    if (i - delta != 1)
                        samplePoints.Add(ComputePoint(degree, knots, controlPoints, weights, 1));
                }
            }

            return samplePoints.Where(p => !float.IsNaN(p.X) && !float.IsNaN(p.Y));
        }

        internal static PointF ComputePoint(int degree, float[] knots, PointF[] controlPoints, float[] weights, float u)
        {
            if (u > 1) throw new ArgumentOutOfRangeException();
            int i = degree;
            while (knots[i + 1] < u) i++;
            int start = i - degree;
            var p = new PointF();
            float down = 0;
            if (weights != null && weights.Length > 0)
            {
                for (int j = start; j <= i; j++)
                {
                    float value = _GetBaseFuncValue(knots, u, j, degree);
                    float downSpan = weights[j] * value;
                    down += downSpan;
                    p.X += downSpan * controlPoints[j].X;
                    p.Y += downSpan * controlPoints[j].Y;
                }
                p.X /= down;
                p.Y /= down;
            }
            else
            {
                for (int j = start; j <= i; j++)
                {
                    float value = _GetBaseFuncValue(knots, u, j, degree);
                    down += value;
                    p.X += value * controlPoints[j].X;
                    p.Y += value * controlPoints[j].Y;
                }
                p.X /= down;
                p.Y /= down;
            }
            return p;
        }

        public static VectorF ComputeVector(int k, int rank, float[] knots, PointF[] cps, float u)
        {
            int i = k;
            while (knots[i + 1] < u) i++;

            var vec = new VectorF();
            for (int j = i - k + rank; j <= i; j++)
            {
                var scale = _GetBaseFuncValue(knots, u, j, k - rank);
                var v = _GetDI(k , rank, j, knots, cps);
                vec += v * scale;
            }

            return vec;
        }

        public static float ComputeArcLength(int k, int rank, float[] knots, PointF[] cps, float u)
        {
            var len = 0.0;
            var hu = u / 2;
            for (int i = 0; i < 5; i++)
            {
                var vec = ComputeVector(k, rank, knots, cps, (float)(hu * _table_x[i] + 0.5));
                len += vec.Length * _table_w[i];
            }

            return (float)(len * hu);
        }

        public static float ComputeCurveParameter(int k, int rank, float[] knots, PointF[] cps, float s, float L)
        {
            var u = s / L;
            var low = 0f;
            var high = 1f;
            var iterate = 5;
            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(k, rank, knots, cps, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = ComputeVector(k, rank, knots, cps, u).Length;
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        private static VectorF _GetDI(int k, int rank, int j, float[] knots, PointF[] cps)
        {
            var vec = new VectorF();

            if (rank == 0)
                vec = (VectorF)cps[j];
            else vec = (k - rank + 1) * (_GetDI(k, rank - 1, j, knots, cps) - _GetDI(k, rank - 1, j - 1, knots, cps)) / (knots[j + k + 1 - rank] - knots[j]);

            return vec;
        }

        private static float _GetBaseFuncValue(float[] knots, float u, int pbase, int rank)
        {
            if (rank > 0)
            {
                return _GetRatioLeft(knots, u, pbase, rank) * _GetBaseFuncValue(knots, u, pbase, rank - 1)
                    + _GetRatioRight(knots, u, pbase, rank) * _GetBaseFuncValue(knots, u, pbase + 1, rank - 1);
            }
            else
            {
                if (u >= knots[pbase] && u <= knots[pbase + 1]) return 1;
                return 0;
            }
        }

        private static float _GetRatioLeft(float[] knots, float u, int pbase, int rank)
        {
            float up = u - knots[pbase];
            float down = knots[pbase + rank] - knots[pbase];
            if (MathUtil.IsZero(down)) return 0;
            return up / down;
        }

        private static float _GetRatioRight(float[] knots, float u, int pbase, int rank)
        {
            float up = knots[pbase + rank + 1] - u;
            float down = knots[pbase + rank + 1] - knots[pbase + 1];
            if (MathUtil.IsZero(down)) return 0;
            return up / down;
        }
        #endregion

        #region Bezier
        public static IEnumerable<PointF> CalcSamplePoints(PointF[] controlPoints, int degree, float tolerance)
        {
            return _CalcSamplePoints(controlPoints, degree, tolerance);
        }

        public static RectF CalcBounds(float tolerance, params PointF[] controlPoints)
        {
            var degree = controlPoints.Length - 1;
            return _CalcBounds(_CalcSamplePoints(controlPoints, degree, tolerance));
        }

        internal static List<_Line> CalcSampleLines(float tolerance, params PointF[] controlPoints)
        {
            var degree = controlPoints.Length - 1;

            var lines = new List<_Line>();

            var samplePoints = _CalcSamplePoints(controlPoints, degree, tolerance);

            var flag = true;
            var last = default(PointF);
            foreach (var p in samplePoints)
            {
                if (flag)
                {
                    flag = false;
                    last = p;
                }
                else
                {
                    lines.Add(new _Line(last, p, PenF.NULL));
                    last = p;
                }
            }
            return lines;
        }

        private static IEnumerable<PointF> _CalcSamplePoints(PointF[] controlPoints, int degree, float tolerance)
        {
            var samplePoints = new List<PointF>();
            var i = 0.0;
            var delta = tolerance / GetLength(controlPoints);

            if (delta > 1)
            {
                samplePoints.Add(ComputePoint(controlPoints, degree, 0));
                samplePoints.Add(ComputePoint(controlPoints, degree, 1));
            }
            else
            {
                delta = Math.Max(tolerance / (10 * controlPoints.Length), delta);
                while (i <= 1)
                {
                    samplePoints.Add(ComputePoint(controlPoints, degree, i));
                    i += delta;
                }
                if (i - delta != 1)
                    samplePoints.Add(ComputePoint(controlPoints, degree, 1));
            }
            return samplePoints;
        }

        internal static PointF ComputePoint(PointF[] controlPoints, int degree, double u)
        {
            return CalcValue(controlPoints, degree, 0, u);
        }

        public static VectorF ComputeVector(PointF[] cps, int degree, double u)
        {
            return degree * (CalcValue(cps, degree - 1, 1, u) - CalcValue(cps, degree - 1, 0, u));
        }

        public static float ComputeArcLength(PointF[] cps, int degree, double u)
        {
            var len = 0.0;
            var hu = u / 2;
            for (int i = 0; i < 5; i++)
            {
                var vec = ComputeVector(cps, degree, (float)(hu * _table_x[i] + 0.5));
                len += vec.Length * _table_w[i];
            }

            return (float)(len * hu);
        }

        public static float ComputeCurveParameter(PointF[] cps, int degree, float s, float L)
        {
            var u = s / L;
            var low = 0f;
            var high = 1f;
            var iterate = 5;
            for (int i = 0; i < iterate; i++)
            {
                var delta = ComputeArcLength(cps, degree, u) - s;
                if (Math.Abs(delta) < Epsilon)
                    return u;

                var df = ComputeVector(cps, degree, u).Length;
                var uNext = u - delta / df;
                if (delta > 0)
                {
                    high = u;
                    if (uNext <= low)
                        u = (low + high) / 2;
                    else u = uNext;
                }
                else
                {
                    low = u;
                    if (uNext >= high)
                        u = (low + high) / 2;
                    else u = uNext;
                }
            }
            return u;
        }

        internal static PointF CalcValue(PointF[] controlPoints, int degree, int index, double u)
        {
            if (degree == 0)
                return controlPoints[index];
            else return Combine(CalcValue(controlPoints, degree - 1, index, u), CalcValue(controlPoints, degree - 1, index + 1, u), u);
        }

        internal static PointF Combine(PointF p1, PointF p2, double u)
        {
            var u1 = 1 - u;
            var u2 = u;
            return (PointF)new Point(u1 * p1.X + u2 * p2.X, u1 * p1.Y + u2 * p2.Y);
        }
        #endregion

        #region Guass-lengendre n = 5, 代数精度为11
        private static double[] _table_w = new double[] { 0.5688888888888889, 0.4786286704993665, 0.4786286704993665, 0.2369268850561891, 0.2369268850561891 };// Weights
        private static double[] _table_x = new double[] { 0, -0.5384693101056831, 0.5384693101056831, -0.9061798459386640, 0.9061798459386640 };// Guass-Points
        private static double Epsilon = 0.00000001;
        #endregion
    }
}
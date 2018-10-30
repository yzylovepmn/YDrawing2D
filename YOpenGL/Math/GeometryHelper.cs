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
        internal readonly static PointF[] UnitCicle;
        internal readonly static float DeltaRadian;

        static GeometryHelper()
        {
            DeltaRadian = GetRadian(360 / 64f);
            UnitCicle = GenCiclePoints().ToArray();
        }

        internal static bool IsPossibleArcContain(_Arc arc, PointF point)
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

        internal static float GetRadian(PointF center, PointF p)
        {
            var vec = p - center;
            var radian = (float)Math.Atan(vec.Y / vec.X);
            if (vec.X < 0)
                radian += (float)Math.PI;
            FormatRadian(ref radian);
            return radian;
        }

        internal static void FormatRadian(ref float radian)
        {
            var _radian = (double)radian;

            var pi2 = 2 * Math.PI;
            while (_radian < 0)
                _radian += pi2;
            while (_radian > pi2)
                _radian -= pi2;

            radian = (float)_radian;
        }

        internal static void FormatAngle(ref float angle)
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

        internal static IEnumerable<PointF> GenArcPoints(float startRadian, float endRadian)
        {
            var points = new List<PointF>();
            float curRadian = 0;
            bool flag = false, isAfter = startRadian > endRadian, needRestart = endRadian > 0;
            if (!isAfter)
            {
                endRadian -= 2 * (float)Math.PI;
                curRadian -= 2 * (float)Math.PI;
            }
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
            points.Reverse();
            return points;
        }

        internal static float GetRadian(float angle)
        {
            return (float)(angle * Math.PI / 180);
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

        public static bool Contains(_Geometry geo, PointF point)
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

            if (geo.UnClosedLine.HasValue)
                primitives.Add(geo.UnClosedLine.Value);

            foreach (var primitive in primitives)
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                        var line = (_Line)primitive;
                        if (line.Start.X > point.X)
                        {
                            if (line.Start.Y > point.Y)
                            {
                                if (line.End.X > point.X)
                                {
                                    // 1 1
                                    if (line.End.Y > point.Y)
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
                                    if (line.End.Y > point.Y)
                                    {
                                        toppass++;
                                    }
                                    else
                                    {
                                        // 1 3
                                        var v = line.A * point.X + line.B * point.Y + line.C;
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
                                if (line.End.X > point.X)
                                {
                                    if (line.End.Y > point.Y)
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
                                    if (line.End.Y > point.Y)
                                    {
                                        // 4 2
                                        var v = line.A * point.X + line.B * point.Y + line.C;
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
                            if (line.Start.Y > point.Y)
                            {
                                if (line.End.X > point.X)
                                {
                                    // 2 1
                                    if (line.End.Y > point.Y)
                                        toppass++;
                                    else
                                    {
                                        // 2 4
                                        var v = line.A * point.X + line.B * point.Y + line.C;
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
                                    if (line.End.Y > point.Y)
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
                                if (line.End.X > point.X)
                                {
                                    if (line.End.Y > point.Y)
                                    {
                                        // 3 1
                                        var v = line.A * point.X + line.B * point.Y + line.C;
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
                                    if (line.End.Y > point.Y)
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
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                        else if (len == arc.Radius)
                                            return true;
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
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                        else if (len == arc.Radius)
                                            return true;
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
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                        else if (len == arc.Radius)
                                            return true;
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
                                        if (len < arc.Radius)
                                        {
                                            leftpass++;
                                            toppass++;
                                            rightpass++;
                                            bottompass++;
                                        }
                                        else if (len == arc.Radius)
                                            return true;
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
                case PrimitiveType.Rect:
                    return _GenRectIndices(offset);
            }
            return null;
        }

        private static IEnumerable<uint> _GenRectIndices(uint offset)
        {
            yield return offset + 0;
            yield return offset + 1;
            yield return offset + 2;
            yield return offset + 0;
            yield return offset + 2;
            yield return offset + 3;
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
        internal static List<_Line> CalcSampleLines(_Spline spline)
        {
            var lines = new List<_Line>();

            var samplePoints = new List<PointF>();
            if (spline.Knots.Length == 0)
                samplePoints = spline.FitPoints.ToList();
            else for (int i = 0; i <= (int)spline.Domain; i += 2)
                    samplePoints.Add(ComputePoint(spline, i));

            var _samplePoints = samplePoints.ToArray();
            for (int i = 1; i < _samplePoints.Length; i++)
                lines.Add(new _Line(_samplePoints[i - 1], _samplePoints[i], PenF.NULL));

            return lines;
        }

        internal static PointF ComputePoint(_Spline spline, float u)
        {
            if (u > spline.Domain) throw new ArgumentOutOfRangeException();
            int i = spline.Degree;
            while (spline.Knots[i + 1] < u) i++;
            int start = i - spline.Degree;
            var p = new PointF();
            float down = 0;
            if (spline.Weights.Length > 0)
            {
                for (int j = start; j <= i; j++)
                {
                    float value = _GetBaseFuncValue(spline, u, j, spline.Degree);
                    float downSpan = spline.Weights[j] * value;
                    down += downSpan;
                    p.X += downSpan * spline.ControlPoints[j].X;
                    p.Y += downSpan * spline.ControlPoints[j].Y;
                }
                p.X /= down;
                p.Y /= down;
            }
            else
            {
                for (int j = start; j <= i; j++)
                {
                    float value = _GetBaseFuncValue(spline, u, j, spline.Degree);
                    down += value;
                    p.X += value * spline.ControlPoints[j].X;
                    p.Y += value * spline.ControlPoints[j].Y;
                }
                p.X /= down;
                p.Y /= down;
            }
            return p;
        }

        private static float _GetBaseFuncValue(_Spline spline, float u, int pbase, int rank)
        {
            if (rank > 0)
            {
                return _GetRatioLeft(spline, u, pbase, rank) * _GetBaseFuncValue(spline, u, pbase, rank - 1)
                    + _GetRatioRight(spline, u, pbase, rank) * _GetBaseFuncValue(spline, u, pbase + 1, rank - 1);
            }
            else
            {
                if (u >= spline.Knots[pbase] && u <= spline.Knots[pbase + 1]) return 1;
                return 0;
            }
        }

        private static float _GetRatioLeft(_Spline spline, float u, int pbase, int rank)
        {
            float up = u - spline.Knots[pbase];
            float down = spline.Knots[pbase + rank] - spline.Knots[pbase];
            if (down < 0.001) return 0;
            return up / down;
        }

        private static float _GetRatioRight(_Spline spline, float u, int pbase, int rank)
        {
            float up = spline.Knots[pbase + rank + 1] - u;
            float down = spline.Knots[pbase + rank + 1] - spline.Knots[pbase + 1];
            if (down < 0.001) return 0;
            return up / down;
        }
        #endregion

        #region Bezier
        internal static List<_Line> CalcSampleLines(_Bezier bezier)
        {
            var lines = new List<_Line>();

            var samplePoints = new List<PointF>();
            var i = 0.0;
            var delta = 5f / GetLength(bezier.Points);

            if (delta > 1)
            {
                samplePoints.Add(ComputePoint(bezier, 0));
                samplePoints.Add(ComputePoint(bezier, 1));
            }
            else
            {
                while (i <= 1)
                {
                    samplePoints.Add(ComputePoint(bezier, i));
                    i += delta;
                }
                if (i - delta != 1)
                    samplePoints.Add(ComputePoint(bezier, 1));
            }

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
                    if (last != p)
                    {
                        lines.Add(new _Line(last, p, PenF.NULL));
                        last = p;
                    }
                }
            }
            return lines;
        }

        internal static PointF ComputePoint(_Bezier bezier, double u)
        {
            return CalcValue(bezier, bezier.Degree, 0, u);
        }

        internal static PointF CalcValue(_Bezier bezier, int degree, int index, double u)
        {
            if (degree == 0)
                return bezier.Points[index];
            else return Combine(CalcValue(bezier, degree - 1, index, u), CalcValue(bezier, degree - 1, index + 1, u), u);
        }

        internal static PointF Combine(PointF p1, PointF p2, double u)
        {
            var u1 = 1 - u;
            var u2 = u;
            return (PointF)new Point(u1 * p1.X + u2 * p2.X, u1 * p1.Y + u2 * p2.Y);
        }
        #endregion
    }
}
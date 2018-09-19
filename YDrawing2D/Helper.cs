using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Extensions;
using YDrawing2D.Model;
using YDrawing2D.Util;
using YDrawing2D.View;

namespace YDrawing2D
{
    public class Helper
    {
        public static byte[] CalcColor(Color color, double opacity = 1)
        {
            byte[] colors = new byte[4];

            colors[3] = (byte)(opacity * color.A); // A
            colors[2] |= color.R; // R
            colors[1] |= color.G; // G
            colors[0] |= color.B; // B

            return colors;
        }

        public static int CalcMCD(int a, int b)
        {
            if (a == b) return a;
            if (Math.Abs(a) > Math.Abs(b))
            {
                if (b == 0)
                    return a;
                return CalcMCD(b, a % b);
            }
            else
            {
                if (a == 0)
                    return b;
                return CalcMCD(a, b % a);
            }
        }

        public static Int64 CalcMCD(Int64 a, Int64 b)
        {
            if (a == b) return a;
            if (Math.Abs(a) > Math.Abs(b))
            {
                if (b == 0)
                    return a;
                return CalcMCD(b, a % b);
            }
            else
            {
                if (a == 0)
                    return b;
                return CalcMCD(a, b % a);
            }
        }

        public static void Switch(ref double a, ref double b)
        {
            var c = a;
            a = b;
            b = c;
        }

        public static void Switch(ref Point p1, ref Point p2)
        {
            var p = p1;
            p1 = p2;
            p2 = p;
        }

        public static IEnumerable<Int32> ConvertTo(IEnumerable<double> values)
        {
            foreach (var value in values)
                yield return ConvertTo(value);
        }

        public static Int32 ConvertTo(double value)
        {
            return Math.Max(1, (Int32)value);
        }

        public static IEnumerable<Int32Point> FilterWithDashes(IEnumerable<Int32Point> source, Int32[] dashes)
        {
            var currentDash = 0;
            var curDashValue = dashes[currentDash];
            var cnt = 0;
            var flag = true;
            foreach (var p in source)
            {
                if (cnt < curDashValue)
                {
                    cnt++;
                    if (flag)
                        yield return p;
                    else continue;
                }
                else
                {
                    cnt = 0;
                    flag = !flag;
                    currentDash++;
                    if (currentDash == dashes.Length)
                        currentDash = 0;
                    curDashValue = dashes[currentDash];

                    if (flag)
                        yield return p;
                    else continue;
                }
            }
        }

        public static IEnumerable<Int32Point> FilterUniquePoints(IEnumerable<PrimitivePath> paths, Int32Rect bound, int[] dash)
        {
            var points = new List<Int32Point>();
            foreach (var path in paths)
            {
                if (path.IsVirtual) continue;
                points.AddRange(path.Path);
            }
            if (dash != null)
                return FilterWithDashes(points, dash).Where(p => bound.Contains(p));
            return points.Where(p => bound.Contains(p));
        }
    }

    public class VisualHelper
    {
        public static int HitTestThickness;
        internal static List<Int32Point> HitTestPoints;

        static VisualHelper()
        {
            HitTestThickness = 7;
        }

        public static PresentationVisual HitTest(PresentationPanel panel, Point p)
        {
            var _p = GeometryHelper.ConvertToInt32Point(p, panel.DPIRatio);

            var points = GeometryHelper.CalcHitTestPoints(_p.X, _p.Y, panel.Bounds);

            //var color = default(byte[]);
            foreach (var point in points)
            {
                //color = panel.GetColor(point.X, point.Y);
                //if (color.SequenceEqual(panel.BackColorValue))
                    foreach (var visual in panel.Visuals)
                        if (visual.Contains(point))
                            return visual;
            }

            return null;
        }
    }

    public class GeometryHelper
    {
        /// <summary>
        /// DPI for WPF systems (independent of resolution)
        /// </summary>
        public const double SysDPI = 96;

        /// <summary>
        /// The color buffer size used by each pixel
        /// </summary>
        public const int PixelByteLength = 4;

        internal static Point ConvertWithTransform(Point p, double height, Matrix transform1, StackTransform transform2)
        {
            return transform1.Transform(transform2.Transform(new Point(p.X, height - p.Y)));
        }

        internal static Int32Point ConvertToInt32Point(Point p, double dpiRatio)
        {
            return new Int32Point((int)(p.X * dpiRatio), (int)(p.Y * dpiRatio));
        }

        internal static double GetRadian(double angle)
        {
            return angle * Math.PI / 180;
        }

        internal static Int32Rect ExtendBounds(Int32Rect origin, Int32Rect other)
        {
            var extend = Int32Rect.Empty;
            extend.X = Math.Min(origin.X, other.X);
            extend.Y = Math.Min(origin.Y, other.Y);
            extend.Width = Math.Max(origin.X + origin.Width, other.X + other.Width) - extend.X;
            extend.Height = Math.Max(origin.Y + origin.Height, other.Y + other.Height) - extend.Y;
            return extend;
        }

        internal static Int32Rect RestrictBounds(Int32Rect restriction, Int32Rect bounds)
        {
            int right = restriction.X + restriction.Width;
            int bottom = restriction.Y + restriction.Height;
            int left = Math.Min(Math.Max(restriction.X, bounds.X), right);
            int top = Math.Min(Math.Max(restriction.Y, bounds.Y), bottom);
            int avaWitdh = right - left;
            int avaHeight = bottom - top;
            return new Int32Rect(left, top, Math.Min(avaWitdh, bounds.Width + bounds.X - left), Math.Min(avaHeight, bounds.Height + bounds.Y - top));
        }

        internal static Int32Rect CalcBounds(int thickness, params Int32Point[] points)
        {
            var h = Math.Max(thickness >> 1, 1);
            int left = points[0].X, top = points[0].Y, right = left, bottom = top;
            foreach (var point in points)
            {
                if (point.X < left)
                    left = point.X;
                else if (point.X > right)
                    right = point.X;

                if (point.Y < top)
                    top = point.Y;
                else if (point.Y > bottom)
                    bottom = point.Y;
            }
            return new Int32Rect(left - h, top - h, right - left + (h << 1), bottom - top + (h << 1));
        }

        internal static Int32Rect CalcBounds(Int32Point center, Int32 radius, int thickness)
        {
            int h = Math.Max(thickness >> 1, 1);
            return new Int32Rect(center.X - radius - h, center.Y - radius - h, (radius + h) << 1, (radius + h) << 1);
        }

        internal static Int32Rect CalcBounds(Int32Point center, Int32 radiusX, Int32 radiusY, int thickness)
        {
            int h = Math.Max(thickness >> 1, 1);
            return new Int32Rect(center.X - radiusX - h, center.Y - radiusY - h, (radiusX + h) << 1, (radiusY + h) << 1);
        }

        internal static Int32Rect CalcBounds(Int32Point center, Int32Point start, Int32Point end, Int32 radius, Int32 thickness)
        {
            int h = Math.Max(thickness >> 1, 1);
            thickness = h << 1;
            int left = center.X - radius;
            int top = center.Y - radius;
            int right = center.X + radius;
            int bottom = center.Y + radius;
            if (start.X >= center.X)
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        if (end.Y <= center.Y)
                        {
                            // start 1 end 1
                            if (start.X < end.X)
                            {
                                left = start.X;
                                top = start.Y;
                                right = end.X;
                                bottom = end.Y;
                            }
                        }
                        else
                        {
                            // start 1 end 4
                            left = Math.Min(start.X, end.X);
                            top = start.Y;
                            bottom = end.Y;
                        }
                    }
                    else
                    {
                        // start 1 end 2
                        if (end.Y <= center.Y)
                            top = Math.Min(start.Y, end.Y);
                        else
                        {
                            // start 1 end 3
                            left = end.X;
                            top = start.Y;
                        }
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 4 end 1
                        if (end.Y <= center.Y)
                            right = Math.Max(start.X, end.X);
                        else
                        {
                            // start 4 end 4
                            if (end.X < start.X)
                            {
                                left = end.X;
                                top = start.Y;
                                right = start.X;
                                bottom = end.Y;
                            }
                        }
                    }
                    else
                    {
                        right = start.X;
                        // start 4 end 2
                        if (end.Y <= center.Y)
                            top = end.Y;
                        else
                        {
                            // start 4 end 3
                            left = end.X;
                            top = Math.Min(start.Y, end.Y);
                        }
                    }
                }
            }
            else
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        left = start.X;
                        // start 2 end 1
                        if (end.Y <= center.Y)
                        {
                            right = end.X;
                            bottom = Math.Max(start.Y, end.Y);
                        }
                        // start 2 end 4
                        else bottom = end.Y;
                    }
                    else
                    {
                        // start 2 end 2
                        if (end.Y <= center.Y)
                        {
                            if (start.X < end.X)
                            {
                                left = start.X;
                                top = end.Y;
                                right = end.X;
                                bottom = start.Y;
                            }
                        }
                        // start 2 end 3
                        else left = Math.Min(start.X, end.X);
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 3 end 1
                        if (end.Y <= center.Y)
                        {
                            right = end.X;
                            bottom = start.Y;
                        }
                        // start 3 end 4
                        else bottom = Math.Max(start.Y, end.Y);
                    }
                    else
                    {
                        // start 3 end 2
                        if (end.Y <= center.Y)
                        {
                            top = end.Y;
                            right = Math.Max(start.X, end.X);
                            bottom = start.Y;
                        }
                        else
                        {
                            // start 3 end 3
                            if (end.X < start.X)
                            {
                                left = end.X;
                                top = end.Y;
                                right = start.X;
                                bottom = start.Y;
                            }
                        }
                    }
                }
            }
            return new Int32Rect(left - h, top - h, right - left + thickness, bottom - top + thickness);
        }

        internal static void CalcLineABC(Int32Point p1, Int32Point p2, out Int32 a, out Int32 b, out Int32 c)
        {
            if (p1.X == p2.X)
            {
                b = 0;
                a = 1;
                c = -p1.X;
            }
            else if (p1.Y == p2.Y)
            {
                a = 0;
                b = 1;
                c = -p2.Y;
            }
            else
            {
                Int64 _a = p2.Y - p1.Y;
                Int64 _b = p1.X - p2.X;
                Int64 _c = (Int64)p1.Y * p2.X - p2.Y * p1.X;
                var mcd = Helper.CalcMCD(_a, _b);
                mcd = Helper.CalcMCD(mcd, _c);
                _a /= mcd;
                _b /= mcd;
                _c /= mcd;
                a = (int)_a;
                b = (int)_b;
                c = (int)_c;
            }
        }

        internal static IEnumerable<IntPtr> CalcPositions(int x, int y, IntPtr offset, int stride, int thickness, Int32Rect bounds, bool[,] flags)
        {
            IntPtr start = offset;
            if (thickness == 1)
            {
                start += y * stride;
                start += x * PixelByteLength;
                if (flags[x, y])
                    yield break;
                flags[x, y] = true;
                yield return start;
            }
            else
            {
                var len = thickness >> 1;
                x = Math.Max(bounds.X, x - len);
                y = Math.Max(bounds.Y, y - len);
                int width = Math.Min(x + thickness, bounds.Width + bounds.X) - x;
                int height = Math.Min(y + thickness, bounds.Height + bounds.Y) - y;
                start += y * stride;
                start += x * PixelByteLength;
                var curx = x;
                var cury = y;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        cury = y + i;
                        curx = x + j;
                        if (flags[curx, cury]) continue;
                        flags[curx, cury] = true;
                        yield return start + j * PixelByteLength;
                    }
                    start += stride;
                }
            }
        }

        internal static IEnumerable<Int32Point> CalcHitTestPoints(int x, int y, Int32Rect bounds)
        {
            if (VisualHelper.HitTestPoints == null)
                VisualHelper.HitTestPoints = _CalcHitTestPoints().ToList();
            return VisualHelper.HitTestPoints.Select(p => new Int32Point(p.X + x, p.Y + y)).Where(p => bounds.Contains(p));
        }

        private static IEnumerable<Int32Point> _CalcHitTestPoints()
        {
            int x = 0, y = 0;
            if (VisualHelper.HitTestThickness == 1)
            {
                yield return new Int32Point(x, y);
                yield break;
            }
            else
            {
                var len = VisualHelper.HitTestThickness >> 1;
                //int left = Math.Max(bounds.X, x - len);
                //int top = Math.Max(bounds.Y, y - len);
                //int right = Math.Min(left + VisualHelper.HitTestThickness - 1, bounds.Width + bounds.X - 1);
                //int bottom = Math.Min(top + VisualHelper.HitTestThickness - 1, bounds.Height + bounds.Y - 1);
                int curx, cury;
                for (int i = 0; i <= len; i++)
                {
                    curx = x - i;
                    cury = y;
                    //if (curx >= left)
                        yield return new Int32Point(curx, cury);
                    if (i != 0)
                    {
                        curx = x + i;
                        cury = y;
                        //if (curx <= right)
                            yield return new Int32Point(curx, cury);
                    }

                    var l = i - 1;
                    for (int j = 1; j <= l; j++)
                    {
                        curx = x - i;

                        cury = y - j;
                        //if (curx >= left && cury >= top)
                            yield return new Int32Point(curx, cury);

                        cury = y + j;
                        //if (curx >= left && cury <= bottom)
                            yield return new Int32Point(curx, cury);

                        if (i != 0)
                        {
                            curx = x + i;

                            cury = y - j;
                            //if (curx >= left && cury >= top)
                                yield return new Int32Point(curx, cury);

                            cury = y + j;
                            //if (curx >= left && cury <= bottom)
                                yield return new Int32Point(curx, cury);
                        }
                    }

                    if (i != 0)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            curx = x - j;

                            cury = y - i;
                            //if (curx >= left && cury >= top)
                                yield return new Int32Point(curx, cury);

                            cury = y + i;
                            //if (curx >= left && cury <= bottom)
                                yield return new Int32Point(curx, cury);

                            if (j != 0)
                            {
                                curx = x + j;

                                cury = y - i;
                                //if (curx <= right && cury >= top)
                                    yield return new Int32Point(curx, cury);

                                cury = y + i;
                                //if (curx <= right && cury <= bottom)
                                    yield return new Int32Point(curx, cury);
                            }
                        }
                    }
                }
            }
        }

        internal static IEnumerable<PrimitivePath> CalcPrimitivePaths(IPrimitive primitive, bool isVirtual = false)
        {
            switch (primitive.Type)
            {
                case PrimitiveType.Line:
                    var line = (Line)primitive;
                    yield return new PrimitivePath(primitive, _CalcLinePoints(line.Start, line.End), isVirtual);
                    break;
                case PrimitiveType.Cicle:
                    var cicle = (Cicle)primitive;
                    yield return new PrimitivePath(primitive, _CalcCiclePoints(cicle.Center, cicle.Radius), isVirtual);
                    break;
                case PrimitiveType.Arc:
                    var arc = (Arc)primitive;
                    yield return new PrimitivePath(primitive, _CalcArcPoints(arc.Center, arc.Start, arc.End, arc.Radius), isVirtual);
                    break;
                case PrimitiveType.Ellipse:
                    var ellipse = (Ellipse)primitive;
                    if (ellipse.RadiusX == ellipse.RadiusY)
                        yield return new PrimitivePath(primitive, _CalcCiclePoints(ellipse.Center, ellipse.RadiusX), isVirtual);
                    yield return new PrimitivePath(primitive, _CalcEllipsePoints(ellipse.Center, ellipse.RadiusX, ellipse.RadiusY, ellipse.RadiusXSquared, ellipse.RadiusYSquared, ellipse.SplitX), isVirtual);
                    break;
                case PrimitiveType.Spline:
                    var spline = (Spline)primitive;
                    var points = new List<Int32Point>();
                    foreach (var l in spline.InnerLines)
                        points.AddRange(_CalcLinePoints(l.Start, l.End));
                    yield return new PrimitivePath(primitive, points, isVirtual);
                    break;
                case PrimitiveType.Geometry:
                    var geo = (CustomGeometry)primitive;
                    var paths = new List<PrimitivePath>();
                    foreach (var _primitive in geo.Stream)
                        paths.AddRange(CalcPrimitivePaths(_primitive));
                    if (geo.UnClosedLine.HasValue)
                        paths.AddRange(CalcPrimitivePaths(geo.UnClosedLine.Value, true));
                    foreach (var path in paths)
                        yield return path;
                    break;
            }
        }

        /// <summary>
        /// Bresenham algorithm
        /// </summary>
        private static IEnumerable<Int32Point> _CalcLinePoints(Int32Point start, Int32Point end)
        {
            var deltaX = end.X - start.X;
            var deltaY = end.Y - start.Y;
            var deltaX_abs = Math.Abs(deltaX);
            var deltaY_abs = Math.Abs(deltaY);
            var _deltaX = deltaX_abs << 1;
            var _deltaY = deltaY_abs << 1;
            var isSameSymbol = IsSameSymbol(deltaX, deltaY);
            int x, y, stride = isSameSymbol ? 1 : -1;
            var isXdir = deltaX_abs >= deltaY_abs;
            var delta = isXdir ? deltaX : deltaY;
            if (delta > 0)
            {
                if (isSameSymbol)
                {
                    x = start.X;
                    y = start.Y;
                }
                else
                {
                    x = end.X;
                    y = end.Y;
                }
            }
            else
            {
                if (isSameSymbol)
                {
                    x = end.X;
                    y = end.Y;
                }
                else
                {
                    x = start.X;
                    y = start.Y;
                }
            }
            if (isXdir)
            {
                var e = -deltaX_abs;
                for (int i = 0; i <= deltaX_abs; i++)
                {
                    yield return new Int32Point(x, y);
                    x += stride;
                    e += _deltaY;
                    if (e >= 0)
                    {
                        y++;
                        e -= _deltaX;
                    }
                }
            }
            else
            {
                var e = -deltaY_abs;
                for (int i = 0; i <= deltaY_abs; i++)
                {
                    yield return new Int32Point(x, y);
                    y += stride;
                    e += _deltaX;
                    if (e >= 0)
                    {
                        x++;
                        e -= _deltaY;
                    }
                }
            }
        }


        /// <summary>
        /// Bresenham algorithm
        /// </summary>
        private static IEnumerable<Int32Point> _CalcCiclePoints(Int32Point center, Int32 radius)
        {
            Int32 x = 0;
            Int32 y = radius;
            Int32 d = (1 + x - y) << 1;
            while (x <= y)
            {
                byte condition = 0;
                foreach (var point in _GenCiclePoints(new Int32Point(x, y)))
                    yield return new Int32Point(point.X + center.X, point.Y + center.Y);
                if (d > 0)
                {
                    if (((d - x) << 1) > 1)
                        condition = 2;
                }
                else if (d < 0)
                {
                    if (((d + y) << 1) < 1)
                        condition = 1;
                }

                switch (condition)
                {
                    case 0:
                        x++;
                        y--;
                        d += ((1 + x - y) << 1);
                        break;
                    case 1:
                        x++;
                        d += (x << 1) + 1;
                        break;
                    case 2:
                        y--;
                        d += 1 - (y << 1);
                        break;
                }
            }
        }

        /// <summary>
        /// Bresenham algorithm
        /// </summary>
        private static IEnumerable<Int32Point> _CalcEllipsePoints(Int32Point center, Int32 a, Int32 b, Int64 aSquared, Int64 bSquared, Int32 splitX)
        {
            Int32 x = 0;
            Int32 y = b;
            Int64 d = aSquared + bSquared - ((aSquared * b) << 1);
            while (x <= splitX)
            {
                byte condition = 0;
                foreach (var point in _GenEllipsePoints(new Int32Point(x, y)))
                    yield return new Int32Point(point.X + center.X, point.Y + center.Y);

                if (d > 0)
                {
                    if (((d - bSquared * x) << 1) > bSquared)
                        condition = 2;
                }
                else if (d < 0)
                {
                    if (((d + aSquared * y) << 1) < aSquared)
                        condition = 1;
                }

                switch (condition)
                {
                    case 0:
                        x++;
                        y--;
                        d += bSquared + aSquared + ((bSquared * x - aSquared * y) << 1);
                        break;
                    case 1:
                        x++;
                        d += bSquared + ((bSquared * x) << 1);
                        break;
                    case 2:
                        y--;
                        d += aSquared - ((aSquared * y) << 1);
                        break;
                }
            }

            x = a;
            y = 0;
            d = aSquared + bSquared - ((bSquared * a) << 1);
            while (x > splitX)
            {
                byte condition = 0;
                foreach (var point in _GenEllipsePoints(new Int32Point(x, y)))
                    yield return new Int32Point(point.X + center.X, point.Y + center.Y);

                if (d > 0)
                {
                    if (((d - aSquared * y) << 1) > aSquared)
                        condition = 1;
                }
                else if (d < 0)
                {
                    if (((d + bSquared * x) << 1) < bSquared)
                        condition = 2;
                }

                switch (condition)
                {
                    case 0:
                        x--;
                        y++;
                        d += bSquared + aSquared - ((bSquared * x - aSquared * y) << 1);
                        break;
                    case 1:
                        x--;
                        d += bSquared - ((bSquared * x) << 1);
                        break;
                    case 2:
                        y++;
                        d += aSquared + ((aSquared * y) << 1);
                        break;
                }
            }
        }

        private static IEnumerable<Int32Point> _CalcArcPoints(Int32Point center, Int32Point start, Int32Point end, Int32 radius)
        {
            return ArcContains(center, start, end, _CalcCiclePoints(center, radius));
        }

        private static IEnumerable<Int32Point> _GenCiclePoints(Int32Point origin)
        {
            yield return new Int32Point(origin.X, -origin.Y);
            yield return origin;
            yield return new Int32Point(-origin.X, -origin.Y);
            yield return new Int32Point(-origin.X, origin.Y);
            yield return new Int32Point(origin.Y, -origin.X);
            yield return new Int32Point(origin.Y, origin.X);
            yield return new Int32Point(-origin.Y, -origin.X);
            yield return new Int32Point(-origin.Y, origin.X);
        }

        private static IEnumerable<Int32Point> _GenEllipsePoints(Int32Point origin)
        {
            yield return new Int32Point(origin.X, -origin.Y);
            yield return origin;
            yield return new Int32Point(-origin.X, -origin.Y);
            yield return new Int32Point(-origin.X, origin.Y);
        }

        public static IEnumerable<Int32Point> GenScanPoints(Int32Point start, Int32Point end, int delta)
        {
            for (int i = start.Y + delta + 1; i < end.Y - delta; i++)
                yield return new Int32Point(start.X, i);
        }

        internal static IEnumerable<Int32Point> ArcContains(Int32Point center, Int32Point start, Int32Point end, IEnumerable<Int32Point> points)
        {
            if (start.X >= center.X)
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        // start 1 end 1
                        if (end.Y <= center.Y)
                        {
                            if (start.X < end.X)
                                return points.Where(p => p.X >= start.X && p.Y <= end.Y);
                            else return points.Where(p => p.X <= end.X || p.Y >= start.Y);
                        }
                        else
                        {
                            // start 1 end 4
                            return points.Where(p => p.X >= center.X && p.Y <= end.Y && p.Y >= start.Y);
                        }
                    }
                    else
                    {
                        // start 1 end 2
                        if (end.Y <= center.Y)
                            return points.Where(p => p.X <= end.X || p.X >= start.X || p.Y >= center.Y);
                        else
                        {
                            // start 1 end 3
                            return points.Where(p => !(p.X <= center.X && p.Y <= center.Y) && p.X >= end.X && p.Y >= start.Y);
                        }
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 4 end 1
                        if (end.Y <= center.Y)
                            return points.Where(p => p.Y <= end.Y || p.Y >= start.Y || p.X <= center.X);
                        else
                        {
                            // start 4 end 4
                            if (end.X < start.X)
                                return points.Where(p => p.X >= end.X && p.Y >= start.Y);
                            else return points.Where(p => p.X <= start.X || p.Y <= end.Y);
                        }
                    }
                    else
                    {
                        // start 4 end 2
                        if (end.Y <= center.Y)
                            return points.Where(p => !(p.X >= center.X && p.Y <= center.Y) && p.X <= start.X && p.Y >= end.Y);
                        else
                        {
                            // start 4 end 3
                            return points.Where(p => p.Y >= center.Y && p.X <= start.X && p.X >= end.X);
                        }
                    }
                }
            }
            else
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        // start 2 end 1
                        if (end.Y <= center.Y)
                            return points.Where(p => p.Y <= center.Y && p.X >= start.X && p.X <= end.X);
                        // start 2 end 4
                        else return points.Where(p => !(p.X <= center.X && p.Y >= center.Y) && p.X >= start.X && p.Y <= end.Y);
                    }
                    else
                    {
                        // start 2 end 2
                        if (end.Y <= center.Y)
                        {
                            if (start.X < end.X)
                                return points.Where(p => p.X <= end.X && p.Y <= start.Y);
                            else return points.Where(p => p.X >= start.X || p.Y >= end.Y);
                        }
                        // start 2 end 3
                        else return points.Where(p => p.Y <= start.Y || p.Y >= end.Y || p.X >= center.X);
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 3 end 1
                        if (end.Y <= center.Y)
                            return points.Where(p => !(p.X >= center.X && p.Y >= center.Y) && p.X <= end.X && p.Y <= start.Y);
                        // start 3 end 4
                        else return points.Where(p => p.X <= start.X || p.X >= end.X || p.Y <= center.Y);
                    }
                    else
                    {
                        // start 3 end 2
                        if (end.Y <= center.Y)
                            return points.Where(p => p.X <= center.X && p.Y >= end.Y && p.Y <= start.Y);
                        else
                        {
                            // start 3 end 3
                            if (end.X < start.X)
                                return points.Where(p => p.X <= start.X && p.Y >= end.Y);
                            else return points.Where(p => p.X >= end.X || p.Y <= start.Y);
                        }
                    }
                }
            }
        }

        internal static bool IsPossibleArcContains(Int32Point center, Int32Point start, Int32Point end, params Int32Point[] points)
        {
            if (start.X >= center.X)
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        // start 1 end 1
                        if (end.Y <= center.Y)
                        {
                            if (start.X < end.X)
                                return true;
                            else return points.Any(p => p.X <= end.X || p.Y >= start.Y);
                        }
                        else
                        {
                            // start 1 end 4
                            return true;
                        }
                    }
                    else
                    {
                        // start 1 end 2
                        if (end.Y <= center.Y)
                            return points.Any(p => p.X <= end.X || p.X >= start.X || p.Y >= center.Y);
                        else
                        {
                            // start 1 end 3
                            return points.Any(p => !(p.X <= center.X && p.Y <= center.Y) && p.X >= end.X && p.Y >= start.Y);
                        }
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 4 end 1
                        if (end.Y <= center.Y)
                            return points.Any(p => p.Y <= end.Y || p.Y >= start.Y || p.X <= center.X);
                        else
                        {
                            // start 4 end 4
                            if (end.X < start.X)
                                return true;
                            else return points.Any(p => p.X <= start.X || p.Y <= end.Y);
                        }
                    }
                    else
                    {
                        // start 4 end 2
                        if (end.Y <= center.Y)
                            return points.Any(p => !(p.X >= center.X && p.Y <= center.Y) && p.X <= start.X && p.Y >= end.Y);
                        else
                        {
                            // start 4 end 3
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (start.Y <= center.Y)
                {
                    if (end.X >= center.X)
                    {
                        // start 2 end 1
                        if (end.Y <= center.Y)
                            return true;
                        // start 2 end 4
                        else return points.Any(p => !(p.X <= center.X && p.Y >= center.Y) && p.X >= start.X && p.Y <= end.Y);
                    }
                    else
                    {
                        // start 2 end 2
                        if (end.Y <= center.Y)
                        {
                            if (start.X < end.X)
                                return true;
                            else return points.Any(p => p.X >= start.X || p.Y >= end.Y);
                        }
                        // start 2 end 3
                        else return points.Any(p => p.Y <= start.Y || p.Y >= end.Y || p.X >= center.X);
                    }
                }
                else
                {
                    if (end.X >= center.X)
                    {
                        // start 3 end 1
                        if (end.Y <= center.Y)
                            return points.Any(p => !(p.X >= center.X && p.Y >= center.Y) && p.X <= end.X && p.Y <= start.Y);
                        // start 3 end 4
                        else return points.Any(p => p.X <= start.X || p.X >= end.X || p.Y <= center.Y);
                    }
                    else
                    {
                        // start 3 end 2
                        if (end.Y <= center.Y)
                            return true;
                        else
                        {
                            // start 3 end 3
                            if (end.X < start.X)
                                return true;
                            else return points.Any(p => p.X >= end.X || p.Y <= start.Y);
                        }
                    }
                }
            }
        }

        private static bool IsSameSymbol(int a, int b)
        {
            if (a == 0 || b == 0) return true;
            if (a > 0)
            {
                if (b > 0)
                    return true;
                else return false;
            }
            else
            {
                if (b > 0)
                    return false;
                else return true;
            }
        }

        private static bool IsSameSymbol(long a, long b)
        {
            if (a == 0 || b == 0) return true;
            if (a > 0)
            {
                if (b > 0)
                    return true;
                else return false;
            }
            else
            {
                if (b > 0)
                    return false;
                else return true;
            }
        }

        private static bool IsAllNegative(params int[] values)
        {
            return values.All(v => v < 0);
        }

        private static bool IsAllNegative(params long[] values)
        {
            return values.All(v => v < 0);
        }

        #region Intersect
        internal static bool IsIntersect(Line line1, Line line2)
        {
            bool isSameSymbol;
            var s1 = line2.A * line1.Start.X + line2.B * line1.Start.Y + line2.C;
            var s2 = line2.A * line1.End.X + line2.B * line1.End.Y + line2.C;
            isSameSymbol = IsSameSymbol(s1, s2);
            if (isSameSymbol)
            {
                var _s1 = Math.Abs(s1);
                var _s2 = Math.Abs(s2);
                var smaller = Math.Min(_s1, _s2);
                if (smaller > line2.Len * line2.Property.Pen.Thickness)
                    return false;
            }

            s1 = line1.A * line2.Start.X + line1.B * line2.Start.Y + line1.C;
            s2 = line1.A * line2.End.X + line1.B * line2.End.Y + line1.C;
            isSameSymbol = IsSameSymbol(s1, s2);
            if (isSameSymbol)
            {
                var _s1 = Math.Abs(s1);
                var _s2 = Math.Abs(s2);
                var smaller = Math.Min(_s1, _s2);
                if (smaller > line1.Len * line1.Property.Pen.Thickness)
                    return false;
            }

            return true;
        }

        internal static bool IsIntersect(Cicle cicle1, Cicle cicle2)
        {
            var vec = cicle2.Center - cicle1.Center;
            var len = vec.Length;
            var delta = cicle1.Property.Pen.Thickness + cicle2.Property.Pen.Thickness;
            if (len > cicle1.Radius + cicle2.Radius + delta)
                return false;
            var reduce = cicle1.Radius - cicle2.Radius;
            if (reduce >= 0)
            {
                if (len < reduce - delta && cicle1.FillColor == null)
                    return false;
            }
            else
            {
                if (len < -reduce - delta && cicle2.FillColor == null)
                    return false;
            }
            return true;
        }

        internal static bool IsIntersect(Arc arc1, Arc arc2)
        {
            var vec = arc2.Center - arc1.Center;
            var len = vec.Length;
            var delta = arc1.Property.Pen.Thickness + arc2.Property.Pen.Thickness;
            if ((len > arc1.Radius + arc2.Radius + delta)
                || (len + delta < Math.Abs(arc1.Radius - arc2.Radius)))
                return false;
            return true;
        }

        internal static bool IsIntersect(Ellipse ellipse1, Ellipse ellipse2)
        {
            if (ellipse1.Contains(ellipse2.Property.Bounds) || ellipse2.Contains(ellipse1.Property.Bounds))
                return false;

            var vec = ellipse1.Center - ellipse2.Center;
            var len = vec.Length;
            var mRadius1 = Math.Max(ellipse1.RadiusX, ellipse1.RadiusY);
            var sRadius1 = Math.Min(ellipse1.RadiusX, ellipse1.RadiusY);
            var mRadius2 = Math.Max(ellipse2.RadiusX, ellipse2.RadiusY);
            var sRadius2 = Math.Min(ellipse2.RadiusX, ellipse2.RadiusY);
            var delta = ellipse1.Property.Pen.Thickness + ellipse2.Property.Pen.Thickness;
            if (len > mRadius1 + mRadius2 + delta)
                return false;
            if (len + delta < sRadius1 - mRadius2 && ellipse1.FillColor == null)
                return false;
            if (len + delta < sRadius2 - mRadius1 && ellipse2.FillColor == null)
                return false;

            return true;
        }

        internal static bool IsIntersect(Ellipse ellipse, Cicle cicle)
        {
            var vec = ellipse.Center - cicle.Center;
            var len = vec.Length;
            var delta = ellipse.Property.Pen.Thickness + cicle.Property.Pen.Thickness;
            var mRadius = Math.Max(ellipse.RadiusX, ellipse.RadiusY);
            var sRadius = Math.Min(ellipse.RadiusX, ellipse.RadiusY);
            if (len > mRadius + cicle.Radius + delta)
                return false;
            if (len + delta < sRadius - cicle.Radius && ellipse.FillColor == null)
                return false;
            if (len + delta < cicle.Radius - mRadius && cicle.FillColor == null)
                return false;

            return true;
        }

        internal static bool IsIntersect(Ellipse ellipse, Arc arc)
        {
            var vec = ellipse.Center - arc.Center;
            var len = vec.Length;
            var delta = ellipse.Property.Pen.Thickness + arc.Property.Pen.Thickness;
            var mRadius = Math.Max(ellipse.RadiusX, ellipse.RadiusY);
            var sRadius = Math.Min(ellipse.RadiusX, ellipse.RadiusY);
            if (len > mRadius + arc.Radius + delta)
                return false;
            if (len + delta < sRadius - arc.Radius && ellipse.FillColor == null)
                return false;
            if (len + delta < arc.Radius - mRadius)
                return false;

            return true;
        }

        internal static bool IsIntersect(Ellipse ellipse, Line line)
        {
            if (ellipse.FillColor == null && ((line.Start - ellipse.FocusP1).Length + (line.Start - ellipse.FocusP2).Length < ellipse.A_2 - ellipse.Property.Pen.Thickness)
                && ((line.End - ellipse.FocusP1).Length + (line.End - ellipse.FocusP2).Length < ellipse.A_2 - ellipse.Property.Pen.Thickness))
                return false;

            if (line.A == 0)
            {
                var y = -line.C / line.B;
                if (y > ellipse.Center.Y + ellipse.RadiusY + ellipse.Property.Pen.Thickness
                    || y < ellipse.Center.Y - ellipse.RadiusY - ellipse.Property.Pen.Thickness)
                    return false;
            }
            else if (line.B == 0)
            {
                var x = -line.C / line.A;
                if (x > ellipse.Center.X + ellipse.RadiusX + ellipse.Property.Pen.Thickness
                    || x < ellipse.Center.X - ellipse.RadiusX - ellipse.Property.Pen.Thickness)
                    return false;
            }

            return true;
        }

        internal static bool IsIntersect(Cicle cicle, Arc arc)
        {
            var vec = arc.Center - cicle.Center;
            var len = vec.Length;
            var delta = cicle.Property.Pen.Thickness + arc.Property.Pen.Thickness;
            if (len > cicle.Radius + arc.Radius + delta)
                return false;
            var reduce = cicle.Radius - arc.Radius;
            if (reduce >= 0)
            {
                if (len < reduce - delta && cicle.FillColor == null)
                    return false;
            }
            else
            {
                if (len < -reduce - delta)
                    return false;
            }
            return true;
        }

        internal static bool IsIntersect(Line line, Cicle cicle)
        {
            var len = line.A * cicle.Center.X + line.B * cicle.Center.Y + line.C;
            if (len > cicle.Radius * line.Len)
                return false;
            else if (cicle.FillColor == null)
            {
                var radiusSquared = (long)cicle.Radius * cicle.Radius;
                var len1 = (line.Start - cicle.Center).LengthSquared - radiusSquared;
                var len2 = (line.End - cicle.Center).LengthSquared - radiusSquared;
                return !IsAllNegative(len1, len2);
            }
            else return true;
        }

        internal static bool IsIntersect(Line line, Arc arc)
        {
            if (!IsPossibleArcContains(arc.Center, arc.Start, arc.End, line.Start, line.End))
                return false;
            var len = line.A * arc.Center.X + line.B * arc.Center.Y + line.C;
            if (len > arc.Radius * line.Len)
                return false;
            else
            {
                var radiusSquared = (long)arc.Radius * arc.Radius;
                var len1 = (line.Start - arc.Center).LengthSquared - radiusSquared;
                var len2 = (line.End - arc.Center).LengthSquared - radiusSquared;
                return !IsAllNegative(len1, len2);
            }
        }

        internal static bool IsIntersect(Spline spline, IPrimitive other)
        {
            foreach (var line in spline.InnerLines)
                if (line.IsIntersect(other))
                    return true;
            return false;
        }
        #endregion

        #region Region
        public static IEnumerable<Int32Point> CalcRegionSingle(IEnumerable<Int32Point> path, int delta)
        {
            var flag = false;
            Int32Point startp = default(Int32Point), endp = default(Int32Point);
            foreach (var point in path)
            {
                if (!flag)
                    startp = point;
                else
                {
                    endp = point;
                    foreach (var p in GenScanPoints(startp, endp, delta))
                        yield return p;
                }
                flag = !flag;
            }
        }

        internal static IEnumerable<Int32Point> GetVerticalPoints(IEnumerable<PrimitivePath> paths, int x)
        {
            var points = new SortedSet<Int32Point>();
            foreach (var path in paths)
            {
                switch (path.Primitive.Type)
                {
                    case PrimitiveType.Line:
                        foreach (var p in path.Path)
                        {
                            if (p.X == x)
                            {
                                points.Add(p);
                                break;
                            }
                        }
                        break;
                    case PrimitiveType.Arc:
                        var flag = false;
                        foreach (var p in path.Path)
                        {
                            if (flag)
                            {
                                if (p.X == x)
                                    points.Add(p);
                                break;
                            }
                            else
                            {
                                if (p.X == x)
                                {
                                    points.Add(p);
                                    flag = true;
                                }
                            }
                        }
                        break;
                }
            }
            return points;
        }
        #endregion

        #region Spline
        internal static List<Line> CalcSampleLines(Spline spline, double dpiRatio)
        {
            var lines = new List<Line>();

            var samplePoints = new List<Point>();
            if (spline.Knots.Length == 0)
                samplePoints = spline.FitPoints.ToList();
            else for(int i = 0; i <= (int)spline.Domain; i+=2)
                    samplePoints.Add(ComputePoint(spline, i));

            var _samplePoints = samplePoints.Select(sp => ConvertToInt32Point(sp, dpiRatio)).ToArray();
            for (int i = 1; i < _samplePoints.Length; i++)
                lines.Add(new Line(_samplePoints[i - 1], _samplePoints[i], spline.Property.Pen));

            return lines;
        }

        internal static Point ComputePoint(Spline spline, double u)
        {
            if (u > spline.Domain) throw new ArgumentOutOfRangeException();
            int i = spline.Degree;
            while (spline.Knots[i + 1] < u) i++;
            int start = i - spline.Degree;
            var p = new Point();
            double down = 0;
            if (spline.Weights.Length > 0)
            {
                for (int j = start; j <= i; j++)
                {
                    double value = _GetBaseFuncValue(spline, u, j, spline.Degree);
                    double downSpan = spline.Weights[j] * value;
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
                    double value = _GetBaseFuncValue(spline, u, j, spline.Degree);
                    down += value;
                    p.X += value * spline.ControlPoints[j].X;
                    p.Y += value * spline.ControlPoints[j].Y;
                }
                p.X /= down;
                p.Y /= down;
            }
            return p;
        }

        private static double _GetBaseFuncValue(Spline spline, double u, int pbase, int rank)
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

        private static double _GetRatioLeft(Spline spline, double u, int pbase, int rank)
        {
            double up = u - spline.Knots[pbase];
            double down = spline.Knots[pbase + rank] - spline.Knots[pbase];
            if (down < 0.001) return 0;
            return up / down;
        }

        private static double _GetRatioRight(Spline spline, double u, int pbase, int rank)
        {
            double up = spline.Knots[pbase + rank + 1] - u;
            double down = spline.Knots[pbase + rank + 1] - spline.Knots[pbase + 1];
            if (down < 0.001) return 0;
            return up / down;
        }
        #endregion
    }
}
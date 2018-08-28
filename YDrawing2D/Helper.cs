using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static int CalcColor(Color color)
        {
            int color_data = color.A << 24;   // A
            color_data |= color.R << 16;      // R
            color_data |= color.G << 8;       // G
            color_data |= color.B << 0;       // B

            return color_data;
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
    }

    public class VisualHelper
    {
        public static int HitTestThickness = 5;

        public static PresentationVisual HitTest(PresentationPanel panel, Point p)
        {
            var _p = GeometryHelper.ConvertToInt32Point(p, panel.DPIRatio);

            var points = GeometryHelper.CalcPoints(_p.X, _p.Y, HitTestThickness, panel.Bounds);

            int color = default(int);
            foreach (var point in points)
            {
                color = panel.GetColor(point.X, point.Y);
                if (color != panel.BackColorValue)
                    foreach (var visual in panel.Visuals)
                        if (visual.Contains(point, color))
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

        public static Point ConvertWithTransform(Point p, double height, Transform transform)
        {
            return transform.Transform(new Point(p.X, height - p.Y));
        }

        public static Int32Point ConvertToInt32Point(Point p, double dpiRatio)
        {
            return new Int32Point((int)(p.X * dpiRatio), (int)(p.Y * dpiRatio));
        }

        public static Int32Rect RestrictBounds(Int32Rect restriction, Int32Rect bounds)
        {
            int right = restriction.X + restriction.Width;
            int bottom = restriction.Y + restriction.Height;
            int left = Math.Min(Math.Max(restriction.X, bounds.X), right);
            int top = Math.Min(Math.Max(restriction.Y, bounds.Y), bottom);
            int avaWitdh = right - left;
            int avaHeight = bottom - top;
            return new Int32Rect(left, top, Math.Min(avaWitdh, bounds.Width + bounds.X - left), Math.Min(avaHeight, bounds.Height + bounds.Y - top));
        }

        public static Int32Rect CalcBounds(int thickness, params Int32Point[] points)
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

        public static Int32Rect CalcBounds(Int32Point center, Int32 radius, int thickness)
        {
            int h = Math.Max(thickness >> 1, 1);
            return new Int32Rect(center.X - radius - h, center.Y - radius - h, (radius + h) << 1, (radius + h) << 1);
        }

        public static void CalcLineABC(Int32Point p1, Int32Point p2, out Int32 a, out Int32 b, out Int32 c)
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
                a = p2.Y - p1.Y;
                b = p1.X - p2.X;
                c = p1.Y * p2.X - p2.Y * p1.X;
                var mcd = Helper.CalcMCD(a, b);
                mcd = Helper.CalcMCD(mcd, c);
                a /= mcd;
                b /= mcd;
                c /= mcd;
            }
        }

        public static IEnumerable<IntPtr> CalcPositions(int x, int y, IntPtr offset, int stride, int thickness, Int32Rect bounds)
        {
            IntPtr start = offset;
            if (thickness == 1)
            {
                start += y * stride;
                start += x * PixelByteLength;
                yield return start;
                yield break;
            }
            else
            {
                var len = thickness / 2;
                x = Math.Max(bounds.X, x - len);
                y = Math.Max(bounds.Y, y - len);
                int width = Math.Min(x + thickness, bounds.Width + bounds.X) - x;
                int height = Math.Min(y + thickness, bounds.Height + bounds.Y) - y;
                start += y * stride;
                start += x * PixelByteLength;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                        yield return start + j * PixelByteLength;
                    start += stride;
                }
            }
        }

        public static IEnumerable<Int32Point> CalcPoints(int x, int y, int thickness, Int32Rect bounds)
        {
            if (thickness == 1)
            {
                yield return new Int32Point(x, y);
                yield break;
            }
            else
            {
                var len = thickness / 2;
                int left = Math.Max(bounds.X, x - len);
                int top = Math.Max(bounds.Y, y - len);
                int right = Math.Min(left + thickness - 1, bounds.Width + bounds.X - 1);
                int bottom = Math.Min(top + thickness - 1, bounds.Height + bounds.Y - 1);
                int curx, cury;
                for (int i = 0; i <= len; i++)
                {
                    curx = x - i;
                    cury = y;
                    if (curx >= left)
                        yield return new Int32Point(curx, cury);
                    if (i != 0)
                    {
                        curx = x + i;
                        cury = y;
                        if (curx <= right)
                            yield return new Int32Point(curx, cury);
                    }

                    var l = i - 1;
                    for (int j = 1; j <= l; j++)
                    {
                        curx = x - i;

                        cury = y - j;
                        if (curx >= left && cury >= top)
                            yield return new Int32Point(curx, cury);

                        cury = y + j;
                        if (curx >= left && cury <= bottom)
                            yield return new Int32Point(curx, cury);

                        if (i != 0)
                        {
                            curx = x + i;

                            cury = y - j;
                            if (curx >= left && cury >= top)
                                yield return new Int32Point(curx, cury);

                            cury = y + j;
                            if (curx >= left && cury <= bottom)
                                yield return new Int32Point(curx, cury);
                        }
                    }

                    if (i != 0)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            curx = x - j;

                            cury = y - i;
                            if (curx >= left && cury >= top)
                                yield return new Int32Point(curx, cury);

                            cury = y + i;
                            if (curx >= left && cury <= bottom)
                                yield return new Int32Point(curx, cury);

                            if (j != 0)
                            {
                                curx = x + j;

                                cury = y - i;
                                if (curx <= right && cury >= top)
                                    yield return new Int32Point(curx, cury);

                                cury = y + i;
                                if (curx <= right && cury <= bottom)
                                    yield return new Int32Point(curx, cury);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bresenham algorithm
        /// </summary>
        public static IEnumerable<Int32Point> CalcLinePoints(Int32Point start, Int32Point end)
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
        public static IEnumerable<Int32Point> CalcCiclePoints(Int32Point center, Int32 radius)
        {
            Int32 x = 0;
            Int32 y = radius;
            Int32 d = (1 + x - y) << 1;
            while (x <= y)
            {
                byte condition = 0;
                foreach (var p in GenCiclePoints(new Int32Point(x, y)))
                    yield return new Int32Point(p.X + center.X, p.Y + center.Y);
                if (d > 0)
                {
                    if (((d - x) << 1) - 1 > 0)
                        condition = 2;
                }
                else if (d < 0)
                {
                    if (((d + y) << 1) - 1 <= 0)
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

        private static IEnumerable<Int32Point> GenCiclePoints(Int32Point origin)
        {
            yield return origin;
            yield return new Int32Point(origin.Y, origin.X);
            yield return new Int32Point(origin.X, -origin.Y);
            yield return new Int32Point(origin.Y, -origin.X);
            yield return new Int32Point(-origin.X, origin.Y);
            yield return new Int32Point(-origin.Y, origin.X);
            yield return new Int32Point(-origin.X, -origin.Y);
            yield return new Int32Point(-origin.Y, -origin.X);
        }

        private static bool IsSameSymbol(int a, int b)
        {
            if (a == 0 || b == 0) return true;
            if (a >= 0)
            {
                if (b >= 0)
                    return true;
                else return false;
            }
            else
            {
                if (b >= 0)
                    return false;
                else return true;
            }
        }

        #region Intersect
        public static bool IsIntersect(Line line1, Line line2)
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
                if (smaller > line2.Len * line2.Property.Thickness)
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
                if (smaller > line1.Len * line1.Property.Thickness)
                    return false;
            }

            return true;
        }

        public static bool IsIntersect(Cicle cicle1, Cicle cicle2)
        {
            var vec = cicle2.Center - cicle1.Center;
            var len = vec.Length;
            var delta = cicle1.Property.Thickness + cicle2.Property.Thickness;
            if ((len > cicle1.Radius + cicle2.Radius + delta)
                || (len < Math.Max(0, Math.Abs(cicle1.Radius - cicle2.Radius) - delta)))
                return false;
            return true;
        }

        public static bool IsIntersect(Line line, Cicle cicle)
        {
            var len = line.A * cicle.Center.X + line.B * cicle.Center.Y + line.C;
            if (len > cicle.Radius * line.Len)
                return false;
            else
            {
                var len1 = (line.Start - cicle.Center).Length - cicle.Radius;
                var len2 = (line.End - cicle.Center).Length - cicle.Radius;
                bool isSameSymbol = IsSameSymbol(len1, len2);
                if (isSameSymbol && len1 < 0)
                {
                    len1 = -len1;
                    len2 = -len2;
                    var smaller = Math.Min(len1, len2);
                    if (smaller > line.Len * line.Property.Thickness)
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}
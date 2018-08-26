﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Util;

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

        public static Point ConvertToWPFSystem(Point p, double height)
        {
            return new Point(p.X, height - p.Y);
        }

        public static Int32Point ConvertToInt32Point(Point p, double dpiRatio)
        {
            return new Int32Point((int)(p.X * dpiRatio - 0.5), (int)(p.Y * dpiRatio - 0.5));
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

        public static Int32Rect CalcBounds(Int32Point p1, Int32Point p2)
        {
            bool isStartXLargger = p1.X > p2.X;
            bool isStartYLargger = p1.Y > p2.Y;
            int left = isStartXLargger ? p2.X : p1.X;
            int top = isStartYLargger ? p2.Y : p1.Y;
            int width = isStartXLargger ? p1.X - p2.X : p2.X - p1.X;
            int height = isStartYLargger ? p1.Y - p2.Y : p2.Y - p1.Y;
            return new Int32Rect(left, top, width, height);
        }

        public static Int32Rect CalcBounds(Int32Point center, Int32 radius)
        {
            return new Int32Rect(center.X - radius, center.Y - radius, radius, radius);
        }

        public static IEnumerable<IntPtr> CalcPoint(int x, int y, IntPtr offset, int stride, int thickness, Int32Rect bounds)
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
                x = Math.Max(0, x - (thickness % 2 == 0 ? len - 1 : len));
                y = Math.Max(0, y - (thickness % 2 == 0 ? len - 1 : len));
                int width = Math.Min(x + len, bounds.Width - 1) - x;
                int height = Math.Min(y + len, bounds.Height - 1) - y;
                start += y * stride;
                start += x * PixelByteLength;
                for (int i = 0; i <= height; i++)
                {
                    for (int j = 0; j <= width; j++)
                        yield return start + j * PixelByteLength;
                    start += stride;
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
            var _deltaX = deltaX_abs * 2;
            var _deltaY = deltaY_abs * 2;
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
            Int32 d = 2 * (1 + x - y);
            while (x <= y)
            {
                byte condition = 0;
                foreach (var p in GenCiclePoints(new Int32Point(x, y)))
                    yield return new Int32Point(p.X + center.X, p.Y + center.Y);
                if (d > 0)
                {
                    if (2 * (d - x) - 1 > 0)
                        condition = 2;
                }
                else if (d < 0)
                {
                    if (2 * (d + y) - 1 <= 0)
                        condition = 1;
                }

                switch (condition)
                {
                    case 0:
                        x++;
                        y--;
                        d += 2 * (1 + x - y);
                        break;
                    case 1:
                        x++;
                        d += 2 * x + 1;
                        break;
                    case 2:
                        y--;
                        d += 1 - 2 * y;
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
    }
}
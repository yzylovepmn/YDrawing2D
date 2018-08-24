using System;
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
            return new Int32Point((int)(p.X * dpiRatio), (int)(p.Y * dpiRatio));
        }

        public static Int32Rect RestrictBounds(Int32Rect restriction, Int32Rect bounds)
        {
            int left = Math.Max(restriction.X, bounds.X);
            int top = Math.Max(restriction.Y, bounds.Y);
            int right = restriction.X + restriction.Width;
            int bottom = restriction.Y + restriction.Height;
            int avaWitdh = Math.Max(0, right - left);
            int avaHeight = Math.Max(0, bottom - top);
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
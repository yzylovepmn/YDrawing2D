using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public static class Extension
    {
        public static float ScaleX(this MatrixF m)
        {
            return (float)Math.Sqrt(m.M11 * m.M11 + m.M12 * m.M12);
        }

        public static float ScaleY(this MatrixF m)
        {
            return (float)Math.Sqrt(m.M21 * m.M21 + m.M22 * m.M22);
        }

        public static void Dispose<T>(this IEnumerable<T> source) where T : IDisposable
        {
            foreach (var item in source)
                item.Dispose();
        }

        public static float[] GetData(this IEnumerable<PointF> points)
        {
            var data = new List<float>();
            foreach (var point in points)
            {
                data.Add(point.X);
                data.Add(point.Y);
            }
            return data.ToArray();
        }

        public static float[] GetData(this Color color)
        {
            var maxValue = (float)byte.MaxValue;
            return new float[] { color.R / maxValue, color.G / maxValue, color.B / maxValue };
        }

        public static float[] GetData(this MatrixF matrix)
        {
            return new float[] 
            {
                matrix.M11, matrix.M21, 0,
                matrix.M12, matrix.M22, 0,
                matrix.OffsetX, matrix.OffsetY, 1f
            };
        }

        public static bool Contains(this RectF bounds, PointF p, float sensitive)
        {
            if (p.X + sensitive < bounds.X
                || p.X - sensitive > bounds.X + bounds.Width
                || p.Y + sensitive < bounds.Y
                || p.Y - sensitive > bounds.Y + bounds.Height)
                return false;
            return true;
        }
    }
}
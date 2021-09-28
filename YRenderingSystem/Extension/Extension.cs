using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace YRenderingSystem
{
    public static class Extension
    {
        public static IEnumerable<T> TakeOrDefault<T>(this IEnumerable<T> source, int count)
        {
            foreach (var item in source)
            {
                if (count <= 0) yield break;
                yield return item;
                count--;
            }

            while (count-- > 0)
                yield return default;
        }

        public static IEnumerable<PointF> CastTo(this IEnumerable<Point> source)
        {
            foreach (var item in source)
                yield return (PointF)item;
        }

        public static float ScaleX(this MatrixF m)
        {
            return (float)Math.Sqrt(m.M11 * m.M11 + m.M12 * m.M12);
        }

        public static float ScaleY(this MatrixF m)
        {
            return (float)Math.Sqrt(m.M21 * m.M21 + m.M22 * m.M22);
        }

        internal static void DisposeInner<T>(this IEnumerable<T> source) where T : IDisposable
        {
            foreach (var item in source)
                item.Dispose();
        }

        internal static int IndexOfInner<T>(this IEnumerable<T> source, T item)
        {
            var index = 0;
            foreach (var _item in source)
            {
                if (ReferenceEquals(item, _item))
                    return index;
                index++;
            }
            return -1;
        }

        public static float[] GetData(this IEnumerable<PointF> points)
        {
            var data = new float[points.Count() * 2];
            var index = 0;
            foreach (var point in points)
            {
                data[index++] = point.X;
                data[index++] = point.Y;
            }

            return data;
        }

        public static float[] GetData(this Color color)
        {
            return new float[] { color.ScR, color.ScG, color.ScB, color.ScA };
        }

        public static float[] GetData(this IEnumerable<Point3F> points)
        {
            var data = new float[points.Count() * 3];
            var index = 0;
            foreach (var point in points)
            {
                data[index++] = point.X;
                data[index++] = point.Y;
                data[index++] = point.Z;
            }

            return data;
        }

        public static float[] GetData(this IEnumerable<Vector3F> vectors)
        {
            var data = new float[vectors.Count() * 3];
            var index = 0;
            foreach (var vector in vectors)
            {
                data[index++] = vector.X;
                data[index++] = vector.Y;
                data[index++] = vector.Z;
            }

            return data;
        }

        public static int GetValue(this Color color)
        {
            return (color.A << 24) + (color.R << 16) + (color.G << 8) + color.B;
        }

        /// <summary>
        /// Base alignment of vec3 is 16 in Uniform buffer(std140), So we fill the data with 0
        /// </summary>
        public static float[] GetData(this MatrixF matrix, bool needfill = false)
        {
            if (needfill)
                return new float[] 
                {
                    matrix.M11, matrix.M21, 0, 0,
                    matrix.M12, matrix.M22, 0, 0,
                    matrix.OffsetX, matrix.OffsetY, 1f, 0
                };
            else
                return new float[]
                {
                    matrix.M11, matrix.M21, 0,
                    matrix.M12, matrix.M22, 0,
                    matrix.OffsetX, matrix.OffsetY, 1f
                };
        }

        public static float[] GetData(this Matrix3F matrix)
        {
            return new float[]
                {
                    matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                    matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                    matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                    matrix.OffsetX, matrix.OffsetY, matrix.OffsetZ, matrix.M44
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

        #region View
        public static PointF PointToScreenDPIWithoutFlowDirection(this Visual element, PointF point)
        {
            return element.PointToScreenDPI(point);
        }

        public static PointF PointToScreenDPI(this Visual visual, PointF pt)
        {
            PointF resultPt = (PointF)visual.PointToScreen(pt);
            return TransformToDeviceDPI(visual, resultPt);
        }

        public static PointF TransformToDeviceDPI(this Visual visual, PointF pt)
        {
            MatrixF m = (MatrixF)PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new PointF(pt.X / m.M11, pt.Y / m.M22);
        }
        #endregion
    }
}
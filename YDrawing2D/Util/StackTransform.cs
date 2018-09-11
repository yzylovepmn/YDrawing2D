using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Extensions;

namespace YDrawing2D.Util
{
    public class StackTransform : IDisposable
    {
        public StackTransform()
        {
            _matrix = new Matrix();
            _matrices = new Stack<Matrix>();
        }

        private Matrix _matrix;
        private Stack<Matrix> _matrices;

        public double ScaleX { get { return _scaleX; } }
        private double _scaleX = 1;

        public double ScaleY { get { return _scaleY; } }
        private double _scaleY = 1;

        #region Transform
        public Point Transform(Point point)
        {
            return _matrix.Transform(point);
        }

        public Vector Transform(Vector vector)
        {
            return _matrix.Transform(vector);
        }

        public void Transform(Point[] points)
        {
            _matrix.Transform(points);
        }

        public void Transform(Vector[] vectors)
        {
            _matrix.Transform(vectors);
        }

        public void Reset()
        {
            _matrix = new Matrix();
            _matrices.Clear();
        }

        public void PushTranslate(double offsetX, double offsetY)
        {
            var matrix = new Matrix();
            matrix.Translate(offsetX, offsetY);
            _Push(matrix);
        }

        public void PushScale(double scaleX, double scaleY)
        {
            var matrix = new Matrix();
            matrix.Scale(scaleX, scaleY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        public void PushScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            var matrix = new Matrix();
            matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        public void PushRotate(double angle)
        {
            var matrix = new Matrix();
            matrix.Rotate(angle);
            _Push(matrix);
        }

        public void PushRotateAt(double angle, double centerX, double centerY)
        {
            var matrix = new Matrix();
            matrix.RotateAt(angle, centerX, centerY);
            _Push(matrix);
        }

        public void Pop()
        {
            var m = _matrices.Pop();
            m.Invert();
            _matrix *= m;
            _scaleX = _matrix.ScaleX();
            _scaleY = _matrix.ScaleY();
        }

        internal void _Push(Matrix matrix)
        {
            _matrices.Push(matrix);
            _matrix *= matrix;
        }
        #endregion

        public void Dispose()
        {
            _matrices.Clear();
            _matrices = null;
        }
    }
}
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
    internal class StackTransform : IDisposable
    {
        internal StackTransform()
        {
            _opacity = 1;
            _matrix = new Matrix();
            _transforms = new Stack<object>();
        }

        private Matrix _matrix;
        private Stack<object> _transforms;

        internal double Opacity { get { return _opacity; } }
        private double _opacity;

        internal double ScaleX { get { return _scaleX; } }
        private double _scaleX = 1;

        internal double ScaleY { get { return _scaleY; } }
        private double _scaleY = 1;

        #region Transform
        internal Point Transform(Point point)
        {
            return _matrix.Transform(point);
        }

        internal Vector Transform(Vector vector)
        {
            return _matrix.Transform(vector);
        }

        internal void Transform(Point[] points)
        {
            _matrix.Transform(points);
        }

        internal void Transform(Vector[] vectors)
        {
            _matrix.Transform(vectors);
        }

        internal void Transform(Color color)
        {
            color.A = (byte)(color.A * _opacity);
        }

        internal void Reset()
        {
            _opacity = 1;
            _matrix = new Matrix();
            _transforms.Clear();
            _transforms = new Stack<object>();
        }

        internal void PushOpacity(double opacity)
        {
            _opacity *= opacity;
            _transforms.Push(opacity);
        }

        internal void PushTranslate(double offsetX, double offsetY)
        {
            var matrix = new Matrix();
            matrix.Translate(offsetX, offsetY);
            _Push(matrix);
        }

        internal void PushScale(double scaleX, double scaleY)
        {
            var matrix = new Matrix();
            matrix.Scale(scaleX, scaleY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        internal void PushScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            var matrix = new Matrix();
            matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        internal void PushRotate(double angle)
        {
            var matrix = new Matrix();
            matrix.Rotate(angle);
            _Push(matrix);
        }

        internal void PushRotateAt(double angle, double centerX, double centerY)
        {
            var matrix = new Matrix();
            matrix.RotateAt(angle, centerX, centerY);
            _Push(matrix);
        }

        internal void Pop()
        {
            var t = _transforms.Pop();
            if (t is Matrix)
            {
                var m = (Matrix)t;
                m.Invert();
                _matrix *= m;
                _scaleX = _matrix.ScaleX();
                _scaleY = _matrix.ScaleY();
            }
            if (t is double)
            {
                var d = (double)t;
                _opacity = 1;
                foreach (double item in _transforms.TakeWhile(_t => _t is double))
                    _opacity *= item;
            }
        }

        private void _Push(Matrix matrix)
        {
            _transforms.Push(matrix);
            _matrix *= matrix;
        }
        #endregion

        public void Dispose()
        {
            _transforms.Clear();
            _transforms = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    internal class StackTransform : IDisposable
    {
        internal StackTransform()
        {
            _opacity = 1;
            _matrix = new MatrixF();
            _transforms = new Stack<object>();
        }

        public MatrixF Matrix { get { return _matrix; } }
        private MatrixF _matrix;
        private Stack<object> _transforms;

        private float _opacity;

        internal float ScaleX { get { return _scaleX; } }
        private float _scaleX = 1;

        internal float ScaleY { get { return _scaleY; } }
        private float _scaleY = 1;

        internal bool IsIdentity { get { return _matrix.IsIdentity; } }

        #region Transform
        internal Color Transform(Color color)
        {
            return Color.FromArgb((byte)(color.A * _opacity) , color.R, color.G, color.B);
        }

        internal PointF Transform(PointF point)
        {
            return _matrix.Transform(point);
        }

        internal VectorF Transform(VectorF vector)
        {
            return _matrix.Transform(vector);
        }

        internal void Transform(PointF[] points)
        {
            _matrix.Transform(points);
        }

        internal void Transform(VectorF[] vectors)
        {
            _matrix.Transform(vectors);
        }

        internal void Reset()
        {
            _opacity = 1;
            _matrix = new MatrixF();
            _transforms.Clear();
            _transforms = new Stack<object>();
        }

        internal void PushOpacity(float opacity)
        {
            _opacity *= opacity;
            _transforms.Push(opacity);
        }

        internal void PushTranslate(float offsetX, float offsetY)
        {
            var matrix = new MatrixF();
            matrix.Translate(offsetX, offsetY);
            _Push(matrix);
        }

        internal void PushScale(float scaleX, float scaleY)
        {
            var matrix = new MatrixF();
            matrix.Scale(scaleX, scaleY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        internal void PushScaleAt(float scaleX, float scaleY, float centerX, float centerY)
        {
            var matrix = new MatrixF();
            matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            _Push(matrix);
        }

        internal void PushRotate(float angle)
        {
            var matrix = new MatrixF();
            matrix.Rotate(angle);
            _Push(matrix);
        }

        internal void PushRotateAt(float angle, float centerX, float centerY)
        {
            var matrix = new MatrixF();
            matrix.RotateAt(angle, centerX, centerY);
            _Push(matrix);
        }

        internal void Pop()
        {
            var t = _transforms.Pop();
            if (t is MatrixF)
            {
                var m = (MatrixF)t;
                m.Invert();
                _matrix *= m;
                _scaleX = _matrix.ScaleX();
                _scaleY = _matrix.ScaleY();
            }
            if (t is float)
            {
                var d = (float)t;
                _opacity = 1;
                foreach (float item in _transforms.TakeWhile(_t => _t is float))
                    _opacity *= item;
            }
        }

        private void _Push(MatrixF matrix)
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
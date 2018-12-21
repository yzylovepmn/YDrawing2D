using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Point : IPrimitive
    {
        public _Point(PointF point, float pointSize, Color fillColor)
        {
            _point = point;
            _pointSize = Math.Min(20f, pointSize);
            _fillColor = fillColor;
        }

        public PointF Point { get { return _point; } }
        private PointF _point;

        public float PointSize { get { return _pointSize; } }
        private float _pointSize;

        public RectF Bounds
        {
            get
            {
                var hSize = _pointSize / 2;
                var bound = new RectF(new PointF(_point.X - hSize, _point.Y - hSize), new PointF(_point.X + hSize, _point.Y + hSize));
                return bound;
            }
        }

        public PenF Pen { get { return PenF.NULL; } }

        public PrimitiveType Type { get { return PrimitiveType.Point; } }

        public bool Filled { get { return true; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        public MeshModel Model { get { return null; } set { } }

        public MeshModel FillModel { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public IEnumerable<PointF> this[bool isOutline] { get { yield return _point; } }

        public bool HitTest(PointF p, float sensitive)
        {
            return (p - _point).Length < _pointSize / 2;
        }

        public bool HitTest(RectF rect)
        {
            var hSize = _pointSize / 2;
            rect.Intersect(Bounds);
            var v1 = (rect.TopLeft - _point).Length;
            var v2 = (rect.BottomLeft - _point).Length;
            var v3 = (rect.TopRight - _point).Length;
            var v4 = (rect.BottomRight - _point).Length;

            if (v1 > hSize && v2 > hSize && v3 > hSize && v4 > hSize)
                return (rect.Left < _point.X && rect.Right > _point.X)
                    || (rect.Top < _point.Y && rect.Bottom > _point.Y);

            return true;
        }

        public void Dispose()
        {
            _model = null;
        }
    }
}
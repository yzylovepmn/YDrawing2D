using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Arrow : IPrimitive
    {
        public _Arrow(PointF origin, float height, float width, VectorF direction, Color fillColor)
        {
            _origin = origin;
            _height = height;
            _width = width;
            _direction = direction;
            _fillColor = fillColor;
        }

        public PointF Origin { get { return _origin; } }
        private PointF _origin;

        public float Height { get { return _height; } }
        private float _height;

        public float Width { get { return _width; } }
        private float _width;

        public VectorF Direction { get { return _direction; } }
        private VectorF _direction;

        public PenF Pen { get { return PenF.NULL; } }

        public PrimitiveType Type { get { return PrimitiveType.Arrow; } }

        public bool Filled { get { return true; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        public MeshModel Model { get { return null; } set { } }

        public MeshModel FillModel { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public IEnumerable<PointF> this[bool isOutline] { get { yield return _origin; } }

        public RectF GetBounds(float scale)
        {
            return _GetBounds(scale);
        }

        public RectF GetGeometryBounds(float scale)
        {
            return _GetBounds(scale);
        }

        private RectF _GetBounds(float scale)
        {
            if (_height == 0 || _width == 0) return RectF.Empty;

            var height = _height / scale;
            var hwidth = _width / (2 * scale);
            var p = _origin + _direction * height;
            var hdir = new VectorF(_direction.Y, -_direction.X);
            var wl = hdir * hwidth;
            var bound = RectF.Empty;
            bound.Union(new RectF(_origin, p + wl));
            bound.Union(new RectF(_origin, p - wl));
            return bound;
        }

        public bool HitTest(PointF p, float sensitive, float scale)
        {
            if (_height == 0 || _width == 0) return false;

            var height = _height / scale;
            var hwidth = _width / (2 * scale);
            var b = _origin + _direction * height;
            var hdir = new VectorF(_direction.Y, -_direction.X);
            var wl = hdir * hwidth;
            var p1 = b + wl;
            var p2 = b - wl;
            var line1 = new Line(_origin, b);
            var line2 = new Line(p1, p2);

            return _Contains(line1, line2, p, hwidth, height);
        }

        public bool HitTest(RectF rect, float scale)
        {
            if (_height == 0 || _width == 0) return false;

            var height = _height / scale;
            var hwidth = _width / (2 * scale);
            var b = _origin + _direction * height;
            var hdir = new VectorF(_direction.Y, -_direction.X);
            var wl = hdir * hwidth;
            var p1 = b + wl;
            var p2 = b - wl;
            var line1 = new Line(_origin, b);
            var line2 = new Line(p1, p2);

            if (_Contains(line1, line2, rect.BottomLeft, hwidth, height)
                || _Contains(line1, line2, rect.BottomRight, hwidth, height)
                || _Contains(line1, line2, rect.TopLeft, hwidth, height)
                || _Contains(line1, line2, rect.TopRight, hwidth, height))
                return true;

            return rect.Contains(_origin) || rect.Contains(p1) || rect.Contains(p2);
        }

        private bool _Contains(Line line1, Line line2, PointF p, float hwidth, float height)
        {
            var s1 = line2.CalcSymbol(_origin);
            var s2 = line2.CalcSymbol(p);

            if (MathUtil.IsSameSymbol(s1, s2))
            {
                var len1 = line2.CalcLength(s2);
                var len2 = line1.Distance(p);
                return len2 <= (hwidth * (height - len1) / height);
            }

            return false;
        }

        public void Dispose()
        {
            _model = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Arc : IPrimitive
    {
        public _Arc(PointF center, float radius, float startRadian, float endRadian, PenF pen, Color? fillColor = null, bool isReverse = false)
        {
            _pen = pen;
            _fillColor = fillColor;
            _bounds = RectF.Empty;

            Center = center;
            Radius = radius;
            StartRadian = startRadian;
            EndRadian = endRadian;

            _isReverse = isReverse;

            if (IsCicle)
                _bounds = new RectF(new PointF(center.X - radius, center.Y - radius), new PointF(center.X + radius, center.Y + radius));
            else _bounds = GeometryHelper.CalcBounds(this);
        }

        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Arc; } }

        public bool Filled { get { return _fillColor.HasValue; } }

        public bool IsCicle { get { return float.IsInfinity(StartRadian) && float.IsInfinity(EndRadian); } }

        public bool IsEmpty { get { return Radius == 0 || (StartRadian == EndRadian && !float.IsInfinity(StartRadian)); } }

        public Color? FillColor { get { return _fillColor; } }

        public MeshModel Model { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public MeshModel FillModel { get { return _fillModel; } set { _fillModel = value; } }
        private MeshModel _fillModel;

        private Color? _fillColor;
        private bool _isReverse;

        public PointF Center;
        public float Radius;
        public float StartRadian;
        public float EndRadian;

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                if (isOutline)
                    yield return Center;
                else
                {
                    if (IsCicle)
                    {
                        yield return Center;
                        foreach (var point in GeometryHelper.UnitCicle)
                            yield return new PointF(Center.X + point.X * Radius, Center.Y + point.Y * Radius);
                    }
                    else
                    {
                        foreach (var point in GeometryHelper.GenArcPoints(StartRadian, EndRadian, !_isReverse).Skip(1))
                            yield return new PointF(Center.X + point.X * Radius, Center.Y + point.Y * Radius);
                    }
                }
            }
        }

        public RectF GetBounds(float scale)
        {
            return _bounds;
        }

        public RectF GetGeometryBounds(float scale)
        {
            return _bounds;
        }

        public bool HitTest(PointF p, float sensitive, float scale)
        {
            if (IsEmpty) return false;
            if (IsCicle || GeometryHelper.IsPossibleArcContain(this, p))
            {
                var vec = p - Center;
                if (Filled)
                    return vec.Length - Radius < sensitive;
                return Math.Abs(vec.Length - Radius) < sensitive;
            }
            return false;
        }

        public bool HitTest(RectF rect, float scale)
        {
            rect.Intersect(_bounds);
            var v1 = (rect.TopLeft - Center).Length;
            var v2 = (rect.BottomLeft - Center).Length;
            var v3 = (rect.TopRight - Center).Length;
            var v4 = (rect.BottomRight - Center).Length;

            if (v1 > Radius && v2 > Radius && v3 > Radius && v4 > Radius)
                return (rect.Left < Center.X && rect.Right > Center.X)
                    || (rect.Top < Center.Y && rect.Bottom > Center.Y);

            if (!Filled && v1 < Radius && v2 < Radius && v3 < Radius && v4 < Radius)
                return false;

            if (!IsCicle)
                return GeometryHelper.IsIntersect(this, rect, v1, v2, v3, v4);

            return true;
        }

        public void Dispose()
        {
            _model = null;
            _fillModel = null;
        }
    }
}
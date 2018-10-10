using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Bezier : IPrimitive
    {
        public Bezier(Point[] points, int degree, _DrawingPen pen, double dpiRatio)
        {
            _points = points;
            _degree = degree;

            _property = new PrimitiveProperty(pen, Int32Rect.Empty);
            _innerLines = default(List<Line>);

            _innerLines = GeometryHelper.CalcSampleLines(this, dpiRatio);

            foreach (var innerLine in _innerLines)
                _property.Bounds = GeometryHelper.ExtendBounds(_property.Bounds, innerLine.Property.Bounds);
        }

        /// <summary>
        /// Degree of the bezier
        /// </summary>
        public int Degree { get { return _degree; } }
        private int _degree;

        /// <summary>
        /// Points of the bezier
        /// </summary>
        internal Point[] Points { get { return _points; } }
        private Point[] _points;

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Bezier; } }

        internal IEnumerable<Line> InnerLines { get { return _innerLines; } }
        private List<Line> _innerLines;

        public bool HitTest(Int32Point p)
        {
            foreach (var innerLine in _innerLines)
                if (innerLine.HitTest(p))
                    return true;
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;

            return GeometryHelper.IsIntersect(this, other);
        }
    }
}
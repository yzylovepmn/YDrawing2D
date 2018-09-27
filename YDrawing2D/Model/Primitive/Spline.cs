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
    /// <summary>
    /// Non-uniform rational B-spline (NURBS)
    /// </summary>
    public struct Spline : IPrimitive
    {
        public Spline(int degree, double[] knots, Point[] controlPoints, double[] weights, Point[] fitPoints, _DrawingPen pen, double dpiRatio)
        {
            _degree = degree;
            _knots = knots;
            _controlPoints = controlPoints;
            _weights = weights;
            _fitPoints = fitPoints;
            if (_knots?.Length > 0)
                _domain = _knots.Last();
            else _domain = 0;

            _property = new PrimitiveProperty(pen, Int32Rect.Empty);
            _innerLines = default(List<Line>);

            if (_domain != 0)
                Regular(1000);

            _innerLines = GeometryHelper.CalcSampleLines(this, dpiRatio);

            foreach (var innerLine in _innerLines)
                _property.Bounds = GeometryHelper.ExtendBounds(_property.Bounds, innerLine.Property.Bounds);
        }

        /// <summary>
        /// Degree of the spline
        /// </summary>
        public int Degree { get { return _degree; } }
        private int _degree;

        /// <summary>
        /// Knots of the spline
        /// </summary>
        public double[] Knots { get { return _knots; } }
        private double[] _knots;

        /// <summary>
        /// Weights of the spline
        /// </summary>
        public double[] Weights { get { return _weights; } }
        private double[] _weights;

        /// <summary>
        /// ControlPoints of the spline
        /// </summary>
        public Point[] ControlPoints { get { return _controlPoints; } }
        private Point[] _controlPoints;

        /// <summary>
        /// FitPoints of the spline
        /// </summary>
        public Point[] FitPoints { get { return _fitPoints; } }
        private Point[] _fitPoints;

        /// <summary>
        /// Domain of the spline
        /// </summary>
        public double Domain { get { return _domain; } }
        private double _domain;

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Spline; } }

        internal IEnumerable<Line> InnerLines { get { return _innerLines; } }
        private List<Line> _innerLines;

        internal void Regular(double newDomain)
        {
            if (_knots.Length == 0) return;
            var oldDomain = _domain;
            _domain = newDomain;
            for (int i = 0; i < _knots.Length; i++)
                _knots[i] = Math.Round(_knots[i] * _domain / oldDomain, 3);
        }

        public bool HitTest(Int32Point p)
        {
            foreach (var innerLine in _innerLines)
                if (innerLine.Property.Bounds.Contains(p) && innerLine.HitTest(p))
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
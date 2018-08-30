using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    /// <summary>
    /// Non-uniform rational B-spline (NURBS)
    /// </summary>
    public struct Spline : IPrimitive
    {
        public Spline(int degree, List<double> knots, List<Point> controlPoints, List<double> weights, List<Point> fitPoints, _DrawingPen pen)
        {
            _degree = degree;
            _knots = knots;
            _controlPoints = controlPoints;
            _weights = weights;
            _fitPoints = fitPoints;
            if (_knots?.Count > 0)
                _domain = _knots.Last();
            else _domain = 0;

            _property = default(PrimitiveProperty);
        }

        /// <summary>
        /// Degree of the spline
        /// </summary>
        public int Degree { get { return _degree; } }
        private int _degree;

        /// <summary>
        /// Knots of the spline
        /// </summary>
        public List<double> Knots { get { return _knots; } }
        private List<double> _knots;

        /// <summary>
        /// Weights of the spline
        /// </summary>
        public List<double> Weights { get { return _weights; } }
        private List<double> _weights;

        /// <summary>
        /// ControlPoints of the spline
        /// </summary>
        public List<Point> ControlPoints { get { return _controlPoints; } }
        private List<Point> _controlPoints;

        /// <summary>
        /// FitPoints of the spline
        /// </summary>
        public List<Point> FitPoints { get { return _fitPoints; } }
        private List<Point> _fitPoints;

        /// <summary>
        /// Domain of the spline
        /// </summary>
        public double Domain { get { return _domain; } }
        private double _domain;

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Spline; } }

        public void Regular(double newDomain)
        {
            if (_knots.Count == 0) return;
            var oldDomain = _domain;
            _domain = newDomain;
            for (int i = 0; i < _knots.Count; i++)
                _knots[i] = Math.Round(_knots[i] * _domain / oldDomain, 3);
        }

        public bool HitTest(Int32Point p)
        {
            throw new NotImplementedException();
        }

        public bool IsIntersect(IPrimitive other)
        {
            throw new NotImplementedException();
        }
    }
}
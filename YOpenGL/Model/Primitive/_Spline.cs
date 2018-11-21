using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Spline : IPrimitive
    {
        public _Spline(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, PenF pen)
        {
            _degree = degree;
            _knots = knots;
            _controlPoints = controlPoints;
            _weights = weights;
            _fitPoints = fitPoints;
            _pen = pen;

            if (_knots?.Length > 0)
                _domain = _knots.Last();
            else _domain = 0;


            _bounds = RectF.Empty;
            _innerLines = default(List<_Line>);

            if (_domain != 0)
                Regular(1000);

            _innerLines = GeometryHelper.CalcSampleLines(this);

            foreach (var innerLine in _innerLines)
                _bounds.Union(innerLine.Bounds);
        }

        /// <summary>
        /// Degree of the spline
        /// </summary>
        public int Degree { get { return _degree; } }
        private int _degree;

        /// <summary>
        /// Knots of the spline
        /// </summary>
        public float[] Knots { get { return _knots; } }
        private float[] _knots;

        /// <summary>
        /// Weights of the spline
        /// </summary>
        public float[] Weights { get { return _weights; } }
        private float[] _weights;

        /// <summary>
        /// ControlPoints of the spline
        /// </summary>
        public PointF[] ControlPoints { get { return _controlPoints; } }
        private PointF[] _controlPoints;

        /// <summary>
        /// FitPoints of the spline
        /// </summary>
        public PointF[] FitPoints { get { return _fitPoints; } }
        private PointF[] _fitPoints;

        /// <summary>
        /// Domain of the spline
        /// </summary>
        public float Domain { get { return _domain; } }
        private float _domain;

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Spline; } }

        public bool Filled { get { return false; } }

        public Color? FillColor { get { return null; } }

        public MeshModel Model { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public MeshModel FillModel { get { return null; } set { } }

        internal IEnumerable<_Line> InnerLines { get { return _innerLines; } }
        private List<_Line> _innerLines;

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                foreach (var line in _innerLines)
                    foreach (var p in line[isOutline])
                        yield return p;
            }
        }

        internal void Regular(float newDomain)
        {
            if (_knots.Length == 0) return;
            var oldDomain = _domain;
            _domain = newDomain;
            for (int i = 0; i < _knots.Length; i++)
                _knots[i] = (float)Math.Round(_knots[i] * _domain / oldDomain, 3);
        }

        public bool HitTest(PointF p, float sensitive)
        {
            foreach (var line in _innerLines)
                if (line.Bounds.Contains(p, sensitive) && line.HitTest(p, sensitive))
                    return true;
            return false;
        }

        public void Dispose()
        {
            _innerLines.Dispose();
            _innerLines.Clear();
            _innerLines = null;
            _model = null;
        }
    }
}
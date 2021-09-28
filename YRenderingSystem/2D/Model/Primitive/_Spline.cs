using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YRenderingSystem
{
    public class _Spline : IPrimitive
    {
        public _Spline(int degree, float[] knots, PointF[] controlPoints, float[] weights, PointF[] fitPoints, PenF pen, float tolerance)
        {
            //degree = degree;
            knots = knots ?? new float[0];
            controlPoints = controlPoints ?? new PointF[0];
            weights = weights ?? new float[0];
            fitPoints = fitPoints ?? new PointF[0];
            _pen = pen;

            _bounds = RectF.Empty;
            _innerLines = default(List<_Line>);

            Regular(knots, degree);

            _innerLines = GeometryHelper.CalcSampleLines(degree, knots, controlPoints, weights, fitPoints, tolerance);

            foreach (var innerLine in _innerLines)
                _bounds.Union(innerLine.GetBounds(1f));
        }

        /// <summary>
        /// Degree of the spline
        /// </summary>
        //public int Degree { get { return _degree; } }
        //private int _degree;

        /// <summary>
        /// Knots of the spline
        /// </summary>
        //public float[] Knots { get { return _knots; } }
        //private float[] _knots;

        /// <summary>
        /// Weights of the spline
        /// </summary>
        //public float[] Weights { get { return _weights; } }
        //private float[] _weights;

        /// <summary>
        /// ControlPoints of the spline
        /// </summary>
        //public PointF[] ControlPoints { get { return _controlPoints; } }
        //private PointF[] _controlPoints;

        /// <summary>
        /// FitPoints of the spline
        /// </summary>
        //public PointF[] FitPoints { get { return _fitPoints; } }
        //private PointF[] _fitPoints;

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

        internal void Regular(float[] knots, int degree)
        {
            if (knots == null || knots.Length == 0) return;

            var b = -knots[degree];
            for (int i = 0; i < knots.Length; i++)
                knots[i] += b;
            b = knots[knots.Length - 1 - degree];
            for (int i = 0; i < knots.Length; i++)
                knots[i] /= b;
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
            foreach (var line in _innerLines)
                if (line.GetBounds(scale).Contains(p, sensitive) && line.HitTest(p, sensitive, scale))
                    return true;
            return false;
        }

        public bool HitTest(RectF rect, float scale)
        {
            foreach (var line in _innerLines)
                if (line.GetBounds(scale).IntersectsWith(rect) && line.HitTest(rect, scale))
                    return true;
            return false;
        }

        public void Dispose()
        {
            _innerLines.DisposeInner();
            _innerLines.Clear();
            _innerLines = null;
            _model = null;
        }
    }
}
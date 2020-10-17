using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Bezier : IPrimitive
    {
        public _Bezier(PointF[] points, PenF pen, float tolerance)
        {
            _points = points;
            _pen = pen;

            _bounds = RectF.Empty;

            _innerLines = default(List<_Line>);
            _innerLines = GeometryHelper.CalcSampleLines(tolerance, _points);

            foreach (var line in _innerLines)
                _bounds.Union(line.GetBounds(1f));
        }

        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Bezier; } }

        public bool Filled { get { return false; } }

        public Color? FillColor { get { return null; } }

        public MeshModel Model { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public MeshModel FillModel { get { return null; } set { } }

        internal IEnumerable<_Line> InnerLines { get { return _innerLines; } }
        private List<_Line> _innerLines;

        /// <summary>
        /// Degree of the bezier
        /// </summary>
        public int Degree { get { return _points.Length - 1; } }

        /// <summary>
        /// Points of the bezier
        /// </summary>
        internal PointF[] Points { get { return _points; } }
        private PointF[] _points;

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                foreach (var line in _innerLines)
                    foreach (var p in line[isOutline])
                        yield return p;
            }
        }

        public RectF GetBounds(float scale)
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
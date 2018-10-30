using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct _Bezier : IPrimitive
    {
        public _Bezier(PointF[] points, int degree, PenF pen)
        {
            _points = points;
            _degree = degree;
            _pen = pen;

            _bounds = RectF.Empty;

            _innerLines = default(List<_Line>);
            _innerLines = GeometryHelper.CalcSampleLines(this);

            foreach (var line in _innerLines)
                _bounds.Union(line.Bounds);
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Bezier; } }

        public bool Filled { get { return false; } }

        public Color? FillColor { get { return null; } }

        internal IEnumerable<_Line> InnerLines { get { return _innerLines; } }
        private List<_Line> _innerLines;

        /// <summary>
        /// Degree of the bezier
        /// </summary>
        public int Degree { get { return _degree; } }
        private int _degree;

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
            _pen = null;
        }
    }
}
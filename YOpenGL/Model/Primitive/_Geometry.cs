using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct _Geometry : IPrimitive
    {
        public _Geometry(PenF pen, Color? fillColor, PointF begin, bool isClosed)
        {
            _bounds = RectF.Empty;
            _pen = pen;
            _fillColor = fillColor;
            Begin = begin;
            _isClosed = isClosed;
            UnClosedLine = null;
            _stream = new List<IPrimitive>();
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Geometry; } }

        public bool Filled { get { return _fillColor.HasValue; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        internal bool IsClosed { get { return _isClosed; } set { _isClosed = value; } }
        private bool _isClosed;

        internal PointF Begin;

        internal _Line? UnClosedLine;

        internal IEnumerable<IPrimitive> Stream { get { return _stream; } }
        private List<IPrimitive> _stream;

        internal void StreamTo(IPrimitive primitive)
        {
            _stream.Add(primitive);
            _bounds.Union(primitive.Bounds);
        }

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                if (!isOutline)
                    yield return Begin;
                foreach (var primitive in _stream)
                    foreach (var p in primitive[isOutline])
                        yield return p;
                if (!isOutline && UnClosedLine.HasValue)
                    yield return UnClosedLine.Value.End;
            }
        }

        public bool HitTest(PointF p, float sensitive)
        {
            foreach (var primitive in _stream)
                if (primitive.Bounds.Contains(p, sensitive) && primitive.HitTest(p, sensitive))
                    return true;
            if (Filled)
                return GeometryHelper.Contains(this, p);
            return false;
        }

        public void Dispose()
        {
            UnClosedLine = null;
            _stream.Dispose();
            _stream.Clear();
            _stream = null;
            _pen = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct Line : IPrimitive
    {
        public Line(PointF start, PointF end, PenF pen)
        {
            Start = start;
            End = end;
            _pen = pen;
            _bounds = new RectF(start, end);
            _fillColor = null;
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public bool Filled { get { return _fillColor.HasValue; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        internal PointF Start;
        internal PointF End;

        public bool HitTest(PointF p, float sensitive)
        {
            var deltaY = End.Y - Start.Y;
            var deltaX = End.X - Start.X;
            var k = deltaY / deltaX;
            if (float.IsInfinity(k))
                return Math.Abs(p.X - Start.X) < sensitive;
            else
            {
                var b = Start.Y - k * Start.X;
                return Math.Abs(p.Y - k * p.X - b) / Math.Sqrt(k * k + 1) < sensitive;
            }
        }

        public bool IsIntersect(IPrimitive other)
        {
            return false;
        }

        public void Dispose()
        {
            _pen = null;
        }
    }
}
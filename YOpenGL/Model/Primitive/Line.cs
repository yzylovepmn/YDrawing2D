using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        internal PointF Start;
        internal PointF End;

        public bool HitTest(PointF p, float sensitive)
        {
            return true;
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
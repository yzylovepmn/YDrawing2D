using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct _Line : IPrimitive
    {
        public _Line(PointF start, PointF end, PenF pen)
        {
            _pen = pen;
            _bounds = new RectF(start, end);

            Start = start;
            End = end;

            var deltaY = End.Y - Start.Y;
            var deltaX = End.X - Start.X;
            var k = deltaY / deltaX;
            if (float.IsInfinity(k))
            {
                A = 1;
                B = 0;
                C = -Start.X;
            }
            else
            {
                var b = Start.Y - k * Start.X;
                A = k;
                B = -1;
                C = b;
                if (k < 0)
                {
                    A = -A;
                    B = -B;
                    C = -C;
                }
            }
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public bool Filled { get { return false; } }

        public Color? FillColor { get { return null; } }

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                if (isOutline)
                    yield return Start;
                yield return End;
            }
        }

        internal PointF Start;
        internal PointF End;
        internal float A;
        internal float B;
        internal float C;

        public bool HitTest(PointF p, float sensitive)
        {
            if (B == 0)
                return Math.Abs(p.X - Start.X) < sensitive;
            else return Math.Abs(A * p.X + B * p.Y + C) / Math.Sqrt(A * A + B * B) < sensitive;
        }

        public void Dispose()
        {
            _pen = null;
        }
    }
}
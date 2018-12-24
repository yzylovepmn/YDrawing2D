using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Line : IPrimitive
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
                if (A < 0)
                {
                    A = -A;
                    B = -B;
                    C = -C;
                }
            }
        }

        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public bool Filled { get { return false; } }

        public Color? FillColor { get { return null; } }

        public MeshModel Model { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public MeshModel FillModel { get { return null; } set { } }

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

        public RectF GetBounds(float scale)
        {
            return _bounds;
        }

        public bool HitTest(PointF p, float sensitive, float scale)
        {
            if (B == 0)
                return Math.Abs(p.X - Start.X) < sensitive;
            else return Math.Abs(A * p.X + B * p.Y + C) / Math.Sqrt(A * A + B * B) < sensitive;
        }

        public bool HitTest(RectF rect, float scale)
        {
            if (B > 0)
            {
                var p1 = rect.TopLeft;
                var p2 = rect.BottomRight;
                var v1 = A * p1.X + B * p1.Y + C;
                var v2 = A * p2.X + B * p2.Y + C;
                return !MathUtil.IsSameSymbol(v1, v2);
            }
            else if (B < 0)
            {
                var p1 = rect.BottomLeft;
                var p2 = rect.TopRight;
                var v1 = A * p1.X + B * p1.Y + C;
                var v2 = A * p2.X + B * p2.Y + C;
                return !MathUtil.IsSameSymbol(v1, v2);
            }
            return true;
        }

        public void Dispose()
        {
            _model = null;
        }
    }
}
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

            Line = new Line(start, end);
        }

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
                    yield return Line.P1;
                yield return Line.P2;
            }
        }

        internal Line Line;

        public RectF GetBounds(float scale)
        {
            return Line.GetBounds();
        }

        public RectF GetGeometryBounds(float scale)
        {
            return Line.GetBounds();
        }

        public bool HitTest(PointF p, float sensitive, float scale)
        {
            return Line.HitTest(p, sensitive);
        }

        public bool HitTest(RectF rect, float scale)
        {
            return Line.HitTest(rect);
        }

        public void Dispose()
        {
            _model = null;
        }
    }
}
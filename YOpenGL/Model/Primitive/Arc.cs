using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct Arc : IPrimitive
    {
        public Arc(PointF center, float radius, float startRadian, float endRadian, PenF pen, Color? fillColor = null)
        {
            _pen = pen;
            _fillColor = fillColor;
            _bounds = new RectF(new PointF(center.X - radius, center.Y - radius), new PointF(center.X + radius, center.Y + radius));

            Center = center;
            Radius = radius;
            StartRadian = startRadian;
            EndRadian = endRadian;
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.Arc; } }

        public bool Filled { get { return _fillColor.HasValue; } }

        public bool IsCicle { get { return float.IsInfinity(StartRadian) && float.IsInfinity(EndRadian); } }

        public bool IsEmpty { get { return Radius == 0 || (StartRadian == EndRadian && !float.IsInfinity(StartRadian)); } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        public PointF Center;
        public float Radius;
        public float StartRadian;
        public float EndRadian;

        public IEnumerable<PointF> Points
        {
            get
            {
                yield return Center;
            }
        }

        public bool HitTest(PointF p, float sensitive)
        {
            if (IsEmpty) return false;
            if (IsCicle || GeometryHelper.IsArcContain(this, p))
                return Math.Abs((p - Center).Length - Radius) < sensitive;
            return false;
        }

        public void Dispose()
        {

        }
    }
}
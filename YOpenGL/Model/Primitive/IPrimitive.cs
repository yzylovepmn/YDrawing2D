using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public interface IPrimitive : IDisposable
    {
        RectF Bounds { get; }
        PenF Pen { get; }
        PrimitiveType Type { get; }
        bool HitTest(PointF p, float sensitive);
        bool IsIntersect(IPrimitive other);
    }

    public enum PrimitiveType
    {
        Line,
        Cicle,
        Arc,
        Ellipse,
        Spline,
        Bezier,
        Geometry
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public interface IPrimitive : IDisposable
    {
        PenF Pen { get; }
        Color? FillColor { get; }
        bool Filled { get; }
        PrimitiveType Type { get; }
        RectF GetBounds(float scale);
        bool HitTest(PointF p, float sensitive, float scale);
        bool HitTest(RectF rect, float scale);
        IEnumerable<PointF> this[bool isOutline] { get; }
        MeshModel Model { get; set; }
        MeshModel FillModel { get; set; }
    }

    public enum PrimitiveType
    {
        Point,
        Arrow,
        Line,
        Arc,
        Spline,
        Bezier,
        SimpleGeometry,
        ComplexGeometry
    }
}
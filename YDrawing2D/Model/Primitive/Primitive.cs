using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public interface IPrimitive
    {
        PrimitiveProperty Property { get; }
        PrimitiveType Type { get; }
        bool HitTest(Int32Point p);
        bool IsIntersect(IPrimitive other);
    }

    public enum PrimitiveType
    {
        Line,
        Cicle,
        Arc,
        Ellipse,
        Spline,
        Geometry
    }
}
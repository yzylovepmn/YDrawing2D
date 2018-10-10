using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Line : IPrimitive
    {
        internal Line(Int32Point start, Int32Point end, _DrawingPen pen)
        {
            Start = start;
            End = end;
            Len = (Start - End).Length;
            GeometryHelper.CalcLineABC(Start, End, out A, out B, out C);
            Arg = (long)Math.Sqrt((long)A * A + (long)B * B);
            var _bounds = GeometryHelper.CalcBounds(pen.Thickness, start, end);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        internal readonly Int32Point Start;
        internal readonly Int32Point End;
        internal readonly Int32 A;
        internal readonly Int32 B;
        internal readonly Int32 C;
        internal readonly Int64 Arg;
        internal readonly Int32 Len;

        public bool HitTest(Int32Point p)
        {
            if (Start == End)
                return false;
            if (Len < VisualHelper.HitTestThickness)
                return (p - Start).Length < VisualHelper.HitTestThickness || (p - End).Length < VisualHelper.HitTestThickness;
            if (A == 0)
                return p.X >= Math.Min(Start.X, End.X) && p.X <= Math.Max(Start.X, End.X) && Math.Abs(p.Y - Start.Y) < VisualHelper.HitTestThickness;
            if (B == 0)
                return p.Y >= Math.Min(Start.Y, End.Y) && p.Y <= Math.Max(Start.Y, End.Y) && Math.Abs(p.X - Start.X) < VisualHelper.HitTestThickness;
            return _property.Bounds.Contains(p) && Math.Abs((long)A * p.X + (long)B * p.Y + C) < Arg * VisualHelper.HitTestThickness;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect(this, (Line)other);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect(this, (Cicle)other);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
                case PrimitiveType.Ellipse:
                    return GeometryHelper.IsIntersect((Ellipse)other, this);
                case PrimitiveType.Spline:
                    return GeometryHelper.IsIntersect((Spline)other, this);
                case PrimitiveType.Bezier:
                    return GeometryHelper.IsIntersect((Bezier)other, this);
            }
            return true;
        }
    }
}
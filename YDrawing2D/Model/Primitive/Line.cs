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
            GeometryHelper.CalcLineABC(Start, End, out A, out B, out C);
            Len = (int)Math.Sqrt(A * A + B * B);
            var _bounds = GeometryHelper.CalcBounds(pen.Thickness, start, end);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        internal Int32Point Start;

        internal Int32Point End;

        internal Int32 A;
        internal Int32 B;
        internal Int32 C;
        internal Int32 Len;

        public bool HitTest(Int32Point p)
        {
            if (Start.X == End.X && Start.Y == End.Y)
                return false;
            return Math.Abs(A * p.X + B * p.Y + C) < Len;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.IsIntersectWith(this)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect(this, (Line)other);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect(this, (Cicle)other);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
            }
            return true;
        }
    }
}
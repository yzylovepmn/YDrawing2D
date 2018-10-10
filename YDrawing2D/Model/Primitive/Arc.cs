using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    /// <summary>
    /// Default clockwise
    /// </summary>
    public struct Arc : IPrimitive
    {
        internal Arc(Int32Point start, Int32Point end, Int32Point center, _DrawingPen pen)
        {
            Start = start;
            End = end;
            Center = center;
            Radius = (Start - Center).Length;
            var _bounds = GeometryHelper.CalcBounds(center, start, end, Radius, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Arc; } }

        internal readonly Int32Point Center;
        internal readonly Int32Point Start;
        internal readonly Int32Point End;
        internal readonly Int32 Radius;

        public bool HitTest(Int32Point p)
        {
            if (GeometryHelper.IsPossibleArcContains(Center, Start, End, p))
                return Math.Abs((p - Center).Length - Radius) <= _property.Pen.Thickness + VisualHelper.HitTestThickness;
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect((Line)other, this);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect((Cicle)other, this);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Cicle : IPrimitive
    {
        internal Cicle(Int32Point center, Int32 radius, _DrawingPen pen)
        {
            Center = center;
            Radius = radius;
            var _bounds = GeometryHelper.CalcBounds(center, radius, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Cicle; } }

        internal readonly Int32Point Center;
        internal readonly Int32 Radius;

        public bool HitTest(Int32Point p)
        {
            return Math.Abs((p - Center).Length - Radius) <= _property.Pen.Thickness;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect((Line)other, this);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect(this, (Cicle)other);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
                case PrimitiveType.Ellipse:
                    return GeometryHelper.IsIntersect((Ellipse)other, this);
            }
            return true;
        }
    }
}
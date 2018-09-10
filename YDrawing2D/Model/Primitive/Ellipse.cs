using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Ellipse : IPrimitive
    {
        public Ellipse(Int32Point center, Int32 radiusX, Int32 radiusY, _DrawingPen pen)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            RadiusXSquared = (Int64)RadiusX * RadiusX;
            RadiusYSquared = (Int64)RadiusY * RadiusY;
            SplitX = (Int32)(RadiusXSquared / Math.Sqrt(RadiusXSquared + RadiusYSquared));
            A_2 = Math.Max(RadiusX, RadiusY) << 1;
            Int32 c;
            if (RadiusX > RadiusY)
                c = (Int32)Math.Sqrt(RadiusXSquared - RadiusYSquared);
            else c = (Int32)Math.Sqrt(RadiusYSquared - RadiusXSquared);
            if (radiusX > radiusY)
            {
                FocusP1 = new Int32Point(center.X + c, center.Y);
                FocusP2 = new Int32Point(center.X - c, center.Y);
            }
            else
            {
                FocusP1 = new Int32Point(center.X, center.Y + c);
                FocusP2 = new Int32Point(center.X, center.Y - c);
            }
            var _bounds = GeometryHelper.CalcBounds(center, radiusX, RadiusY, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Ellipse; } }

        internal readonly Int32Point Center;
        internal readonly Int32 RadiusX;
        internal readonly Int32 RadiusY;
        internal readonly Int64 RadiusXSquared;
        internal readonly Int64 RadiusYSquared;
        internal readonly Int32 SplitX;
        internal readonly Int32 A_2;
        internal readonly Int32Point FocusP1;
        internal readonly Int32Point FocusP2;

        public bool HitTest(Int32Point p)
        {
            return Math.Abs((p - FocusP1).Length + (p - FocusP2).Length - A_2) <= _property.Pen.Thickness + 1;
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
                case PrimitiveType.Ellipse:
                    return GeometryHelper.IsIntersect(this, (Ellipse)other);
                case PrimitiveType.Spline:
                    break;
            }
            return true;
        }
    }
}
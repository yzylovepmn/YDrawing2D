using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Ellipse : IPrimitive, ICanFilledPrimitive
    {
        public Ellipse(byte[] fillColor, Int32Point center, Int32 radiusX, Int32 radiusY, _DrawingPen pen)
        {
            _fillColor = fillColor;
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

        public byte[] FillColor { get { return _fillColor; } }
        private byte[] _fillColor;

        public bool HitTest(Int32Point p)
        {
            var len1 = (p - FocusP1).Length;
            var len2 = (p - FocusP2).Length;
            if (_fillColor == null)
                return Math.Abs(len1 + len2 - A_2) <= _property.Pen.Thickness + 1;
            else return len1 + len2 <= A_2 + _property.Pen.Thickness + 1;
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
                    return GeometryHelper.IsIntersect(this, (Ellipse)other);
                case PrimitiveType.Spline:
                    return GeometryHelper.IsIntersect((Spline)other, this);
            }
            return true;
        }

        public IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths)
        {
            var region = new List<Int32Point>();
            var delta = _property.Pen.Thickness / 2;
            if (_fillColor != null)
                foreach (var path in paths)
                    region.AddRange(GeometryHelper.CalcRegionSingle(path.Path, delta));
            return region;
        }
    }
}
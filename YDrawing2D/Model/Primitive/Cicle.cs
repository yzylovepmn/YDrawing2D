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
    public struct Cicle : IPrimitive, ICanFilledPrimitive
    {
        internal Cicle(byte[] fillColor, Int32Point center, Int32 radius, _DrawingPen pen)
        {
            _fillColor = fillColor;
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

        public byte[] FillColor { get { return _fillColor; } }
        private byte[] _fillColor;

        public bool HitTest(Int32Point p)
        {
            var len = (p - Center).Length;
            if (_fillColor == null)
                return Math.Abs(len - Radius) <= _property.Pen.Thickness;
            else return len < Radius + _property.Pen.Thickness;
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
                case PrimitiveType.Spline:
                    return GeometryHelper.IsIntersect((Spline)other, this);
                case PrimitiveType.Bezier:
                    return GeometryHelper.IsIntersect((Bezier)other, this);
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
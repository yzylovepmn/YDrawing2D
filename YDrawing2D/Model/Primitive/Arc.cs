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
            _start = start;
            _end = end;
            _center = center;
            _radius = (_start - _center).Length;
            var _bounds = GeometryHelper.CalcBounds(center, start, end, _radius, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Arc; } }

        public Int32Point Center { get { return _center; } }
        private Int32Point _center;

        public Int32Point Start { get { return _start; } }
        private Int32Point _start;

        public Int32Point End { get { return _end; } }
        private Int32Point _end;

        public Int32 Radius { get { return _radius; } }
        private Int32 _radius;

        public bool HitTest(Int32Point p)
        {
            if (GeometryHelper.IsPossibleArcContains(_center, _start, _end, p))
                return Math.Abs((p - _center).Length - _radius) <= _property.Pen.Thickness;
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.IsIntersectWith(this)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect((Line)other, this);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect((Cicle)other, this);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
            }
            return true;
        }
    }
}
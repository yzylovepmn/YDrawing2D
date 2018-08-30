﻿using System;
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
            _center = center;
            _radius = radius;
            var _bounds = GeometryHelper.CalcBounds(center, radius, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Cicle; } }

        public Int32Point Center { get { return _center; } }
        private Int32Point _center;

        public Int32 Radius { get { return _radius; } }
        private Int32 _radius;

        public bool HitTest(Int32Point p)
        {
            return Math.Abs((p - _center).Length - _radius) <= _property.Pen.Thickness;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.IsIntersectWith(this)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect((Line)other, this);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect(this, (Cicle)other);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
            }
            return true;
        }
    }
}
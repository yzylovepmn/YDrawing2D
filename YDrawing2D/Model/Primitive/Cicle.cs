using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Cicle : IPrimitive
    {
        public Cicle(int thickness, int color, Int32Point center, Int32 radius)
        {
            _center = center;
            _radius = radius;
            _radiusSquared = _radius * _radius;
            var _bounds = GeometryHelper.CalcBounds(center, radius);
            _property = new PrimitiveProperty(_bounds, thickness, color);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Cicle; } }

        public Int32Point Center { get { return _center; } }
        private Int32Point _center;

        public Int32 Radius { get { return _radius; } }
        private Int32 _radius;
        private Int64 _radiusSquared;

        public bool HitTest(Int32Point p)
        {
            return Math.Abs(((p - _center).LengthSquared - _radiusSquared)) < 100;
        }
    }
}
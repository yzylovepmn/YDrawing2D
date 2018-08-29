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
    internal struct Line : IPrimitive
    {
        public Line(int thickness, int color, Int32Point start, Int32Point end)
        {
            _start = start;
            _end = end;
            GeometryHelper.CalcLineABC(_start, _end, out _a, out _b, out _c);
            _len = (int)Math.Sqrt(_a * _a + _b * _b);
            var _bounds = GeometryHelper.CalcBounds(thickness, start, end);
            _property = new PrimitiveProperty(_bounds, thickness, color);
        }

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public Int32Point Start { get { return _start; } }
        private Int32Point _start;

        public Int32Point End { get { return _end; } }
        private Int32Point _end;

        public Int32 A { get { return _a; } }
        public Int32 B { get { return _b; } }
        public Int32 C { get { return _c; } }
        public Int32 Len { get { return _len; } }
        private Int32 _a, _b, _c, _len;

        public bool HitTest(Int32Point p)
        {
            if (_start.X == _end.X && _start.Y == _end.Y)
                return false;
            return Math.Abs(_a * p.X + _b * p.Y + _c) < _len;
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
                    break;
            }
            return true;
        }
    }
}
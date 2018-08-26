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
        public Line(int thickness, int color, Int32Point start, Int32Point end)
        {
            _start = start;
            _end = end;
            var _bounds = GeometryHelper.CalcBounds(start, end);
            _property = new PrimitiveProperty(_bounds, thickness, color);
        }

        public PrimitiveType Type { get { return PrimitiveType.Line; } }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public Int32Point Start { get { return _start; } }
        private Int32Point _start;

        public Int32Point End { get { return _end; } }
        private Int32Point _end;
    }
}
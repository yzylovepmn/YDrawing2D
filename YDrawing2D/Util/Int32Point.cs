using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDrawing2D.Util
{
    public struct Int32Point
    {
        public static readonly Int32Point Empty;

        static Int32Point()
        {
            Empty = new Int32Point();
        }

        public Int32Point(Int32 x, Int32 y)
        {
            _x = x;
            _y = y;
        }

        public Int32 X { get { return _x; } }
        private Int32 _x;

        public Int32 Y { get { return _y; } }
        private Int32 _y;

        public override string ToString()
        {
            return string.Format("({0}, {1})", _x, _y);
        }
    }
}
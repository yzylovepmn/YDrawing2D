using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDrawing2D.Util
{
    public struct Int32Vector
    {
        public Int32Vector(Int32 x, Int32 y)
        {
            _x = x;
            _y = y;
        }

        public Int32 X { get { return _x; } }
        private Int32 _x;

        public Int32 Y { get { return _y; } }
        private Int32 _y;

        public Int64 LengthSquared { get { return _x * _x + _y * _y; } }

        public static Int32Vector operator -(Int32Vector v1, Int32Vector v2)
        {
            return new Int32Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Int32Vector operator +(Int32Vector v1, Int32Vector v2)
        {
            return new Int32Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static int operator *(Int32Vector v1, Int32Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static int CrossProduct(Int32Vector v1, Int32Vector v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public static explicit operator Int32Point(Int32Vector v)
        {
            return new Int32Point(v.X, v.Y);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", _x, _y);
        }
    }
}
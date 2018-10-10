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
            X = x;
            Y = y;
        }

        public Int32 X;

        public Int32 Y;

        public Int64 LengthSquared { get { return (long)X * X + (long)Y * Y; } }

        public Int32 Length { get { return (Int32)Math.Sqrt(LengthSquared); } }

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
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
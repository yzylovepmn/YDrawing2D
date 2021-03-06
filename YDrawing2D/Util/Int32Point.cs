﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YDrawing2D.Util
{
    public struct Int32Point : IComparable<Int32Point>
    {
        public static readonly Int32Point Empty;

        static Int32Point()
        {
            Empty = new Int32Point();
        }

        public Int32Point(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }

        public Int32 X;

        public Int32 Y;

        public static bool operator ==(Int32Point p1, Int32Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Int32Point p1, Int32Point p2)
        {
            return !(p1 == p2);
        }

        public static Int32Vector operator -(Int32Point p1, Int32Point p2)
        {
            return new Int32Vector(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Int32Point operator +(Int32Point p, Int32Vector v)
        {
            return new Int32Point(p.X + v.X, p.Y + v.Y);
        }

        public static implicit operator Int32Vector(Int32Point p)
        {
            return new Int32Vector(p.X, p.Y);
        }

        public static implicit operator Point(Int32Point p)
        {
            return new Point(p.X, p.Y);
        }

        public int CompareTo(Int32Point other)
        {
            if (X != other.X)
                return X - other.X;
            return Y - other.Y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Int32Point)
                return this == (Int32Point)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y + (X - Y);
        }
    }
}
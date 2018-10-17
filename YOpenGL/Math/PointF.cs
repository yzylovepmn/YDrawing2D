using System;
using System.Windows;
using Float = System.Single;

namespace YOpenGL
{
    public struct PointF
    {
        public PointF(Float x, Float y)
        {
            _x = x;
            _y = y;
        }

        public void Offset(Float offsetX, Float offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        public static PointF operator +(PointF point, VectorF vector)
        {
            return new PointF(point._x + vector._x, point._y + vector._y);
        }

        public static PointF Add(PointF point, VectorF vector)
        {
            return new PointF(point._x + vector._x, point._y + vector._y);
        }

        public static PointF operator -(PointF point, VectorF vector)
        {
            return new PointF(point._x - vector._x, point._y - vector._y);
        }

        public static PointF Subtract(PointF point, VectorF vector)
        {
            return new PointF(point._x - vector._x, point._y - vector._y);
        }

        public static VectorF operator -(PointF point1, PointF point2)
        {
            return new VectorF(point1._x - point2._x, point1._y - point2._y);
        }

        public static VectorF Subtract(PointF point1, PointF point2)
        {
            return new VectorF(point1._x - point2._x, point1._y - point2._y);
        }

        public static PointF operator *(PointF point, MatrixF matrix)
        {
            return matrix.Transform(point);
        }

        public static PointF Multiply(PointF point, MatrixF matrix)
        {
            return matrix.Transform(point);
        }

        public static explicit operator SizeF(PointF point)
        {
            return new SizeF(Math.Abs(point._x), Math.Abs(point._y));
        }

        public static explicit operator VectorF(PointF point)
        {
            return new VectorF(point._x, point._y);
        }

        public static implicit operator Point(PointF point)
        {
            return new Point(point._x, point._y);
        }

        public static explicit operator PointF(Point point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static bool operator ==(PointF point1, PointF point2)
        {
            return point1.X == point2.X &&
                   point1.Y == point2.Y;
        }
        public static bool operator !=(PointF point1, PointF point2)
        {
            return !(point1 == point2);
        }

        public static bool Equals(PointF point1, PointF point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is PointF))
            {
                return false;
            }

            PointF value = (PointF)o;
            return PointF.Equals(this, value);
        }
        public bool Equals(PointF value)
        {
            return PointF.Equals(this, value);
        }
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }

        public Float X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }

        }

        public Float Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }

        }

        internal Float _x;
        internal Float _y;
    }
}
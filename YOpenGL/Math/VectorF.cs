using System;
using System.Windows.Media;
using Float = System.Single;

namespace YOpenGL
{
    public struct VectorF
    {
        public VectorF(Float x, Float y)
        {
            _x = x;
            _y = y;
        }

        public Float Length
        {
            get
            {
                return (Float)Math.Sqrt(_x * _x + _y * _y);
            }
        }

        public bool IsEmpty { get { return _x == 0f && _y == 0f; } }

        public Float LengthSquared
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }

        public void Normalize()
        {
            // Avoid overflow
            this /= Math.Max(Math.Abs(_x), Math.Abs(_y));
            this /= Length;
        }

        public static Float CrossProduct(VectorF vector1, VectorF vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static Float AngleBetween(VectorF vector1, VectorF vector2)
        {
            Float sin = vector1._x * vector2._y - vector2._x * vector1._y;
            Float cos = vector1._x * vector2._x + vector1._y * vector2._y;

            return (Float)(Math.Atan2(sin, cos) * (180 / Math.PI));
        }

        public static VectorF operator -(VectorF vector)
        {
            return new VectorF(-vector._x, -vector._y);
        }

        public void Negate()
        {
            _x = -_x;
            _y = -_y;
        }

        public static VectorF operator +(VectorF vector1, VectorF vector2)
        {
            return new VectorF(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        public static VectorF Add(VectorF vector1, VectorF vector2)
        {
            return new VectorF(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        public static VectorF operator -(VectorF vector1, VectorF vector2)
        {
            return new VectorF(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        public static VectorF Subtract(VectorF vector1, VectorF vector2)
        {
            return new VectorF(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        public static PointF operator +(VectorF vector, PointF point)
        {
            return new PointF(point._x + vector._x, point._y + vector._y);
        }

        public static PointF Add(VectorF vector, PointF point)
        {
            return new PointF(point._x + vector._x, point._y + vector._y);
        }

        public static VectorF operator *(VectorF vector, Float scalar)
        {
            return new VectorF(vector._x * scalar,
                              vector._y * scalar);
        }

        public static VectorF Multiply(VectorF vector, Float scalar)
        {
            return new VectorF(vector._x * scalar,
                              vector._y * scalar);
        }

        public static VectorF operator *(Float scalar, VectorF vector)
        {
            return new VectorF(vector._x * scalar,
                              vector._y * scalar);
        }

        public static VectorF Multiply(Float scalar, VectorF vector)
        {
            return new VectorF(vector._x * scalar,
                              vector._y * scalar);
        }

        public static VectorF operator /(VectorF vector, Float scalar)
        {
            return vector * (1.0f / scalar);
        }

        public static VectorF Divide(VectorF vector, Float scalar)
        {
            return vector * (1.0f / scalar);
        }

        public static VectorF operator *(VectorF vector, MatrixF matrix)
        {
            return matrix.Transform(vector);
        }

        public static VectorF Multiply(VectorF vector, MatrixF matrix)
        {
            return matrix.Transform(vector);
        }

        public static Float operator *(VectorF vector1, VectorF vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static Float Multiply(VectorF vector1, VectorF vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static Float Determinant(VectorF vector1, VectorF vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static explicit operator SizeF(VectorF vector)
        {
            return new SizeF(Math.Abs(vector._x), Math.Abs(vector._y));
        }

        public static explicit operator PointF(VectorF vector)
        {
            return new PointF(vector._x, vector._y);
        }

        public static bool operator ==(VectorF vector1, VectorF vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y;
        }

        public static bool operator !=(VectorF vector1, VectorF vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool Equals(VectorF vector1, VectorF vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is VectorF))
            {
                return false;
            }

            VectorF value = (VectorF)o;
            return VectorF.Equals(this, value);
        }

        public bool Equals(VectorF value)
        {
            return VectorF.Equals(this, value);
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

        public override string ToString()
        {
            return string.Format($"{_x}, {_y}");
        }
    }
}
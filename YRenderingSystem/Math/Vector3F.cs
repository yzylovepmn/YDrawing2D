using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Float = System.Single;

namespace YRenderingSystem
{
    [Serializable]
    public struct Vector3F
    {
        public Vector3F(Float x, Float y, Float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public bool IsZero { get { return _x == 0 && _y == 0 && _z == 0; } }

        public Float Length
        {
            get
            {
                return (Float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }

        public Float LengthSquared
        {
            get
            {
                return _x * _x + _y * _y + _z * _z;
            }
        }

        public void Normalize()
        {
            if (IsZero) return;

            // Computation of length can overflow easily because it
            // first computes squared length, so we first divide by
            // the largest coefficient.
            Float m = Math.Abs(_x);
            Float absy = Math.Abs(_y);
            Float absz = Math.Abs(_z);
            if (absy > m)
            {
                m = absy;
            }
            if (absz > m)
            {
                m = absz;
            }

            _x /= m;
            _y /= m;
            _z /= m;

            Float length = (Float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            this /= length;
        }

        public static Float AngleBetween(Vector3F vector1, Vector3F vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            Float ratio = DotProduct(vector1, vector2);

            // The "straight forward" method of acos(u.v) has large precision
            // issues when the dot product is near +/-1.  This is due to the
            // steep slope of the acos function as we approach +/- 1.  Slight
            // precision errors in the dot product calculation cause large
            // variation in the output value.
            //
            //        |                   |
            //         \__                |
            //            ---___          |
            //                  ---___    |
            //                        ---_|_
            //                            | ---___
            //                            |       ---___
            //                            |             ---__
            //                            |                  \
            //                            |                   |
            //       -|-------------------+-------------------|-
            //       -1                   0                   1
            //
            //                         acos(x)
            //
            // To avoid this we use an alternative method which finds the
            // angle bisector by (u-v)/2:
            //
            //                            _>
            //                       u  _-  \ (u-v)/2
            //                        _-  __-v
            //                      _=__--      
            //                    .=----------->
            //                            v
            //
            // Because u and v and unit vectors, (u-v)/2 forms a right angle
            // with the angle bisector.  The hypotenuse is 1, therefore
            // 2*asin(|u-v|/2) gives us the angle between u and v.
            //
            // The largest possible value of |u-v| occurs with perpendicular
            // vectors and is sqrt(2)/2 which is well away from extreme slope
            // at +/-1.
            //
            // (See Windows OS Bug #1706299 for details)

            double theta;

            if (ratio < 0)
            {
                theta = Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length / 2.0);
            }
            else
            {
                theta = 2.0 * Math.Asin((vector1 - vector2).Length / 2.0);
            }

            return (Float)MathUtil.RadiansToDegrees(theta);
        }

        public static Vector3F operator -(Vector3F vector)
        {
            return new Vector3F(-vector._x, -vector._y, -vector._z);
        }

        public void Negate()
        {
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        public static Vector3F operator +(Vector3F vector1, Vector3F vector2)
        {
            return new Vector3F(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        public static Vector3F Add(Vector3F vector1, Vector3F vector2)
        {
            return new Vector3F(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        public static Vector3F operator -(Vector3F vector1, Vector3F vector2)
        {
            return new Vector3F(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        public static Vector3F Subtract(Vector3F vector1, Vector3F vector2)
        {
            return new Vector3F(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        public static Point3F operator +(Vector3F vector, Point3F point)
        {
            return new Point3F(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        public static Point3F Add(Vector3F vector, Point3F point)
        {
            return new Point3F(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        public static Point3F operator -(Vector3F vector, Point3F point)
        {
            return new Point3F(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        public static Point3F Subtract(Vector3F vector, Point3F point)
        {
            return new Point3F(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        public static Vector3F operator *(Vector3F vector, Float scalar)
        {
            return new Vector3F(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3F Multiply(Vector3F vector, Float scalar)
        {
            return new Vector3F(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3F operator *(Float scalar, Vector3F vector)
        {
            return new Vector3F(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3F Multiply(Float scalar, Vector3F vector)
        {
            return new Vector3F(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3F operator /(Vector3F vector, Float scalar)
        {
            return vector * (Float)(1.0 / scalar);
        }

        public static Vector3F Divide(Vector3F vector, Float scalar)
        {
            return vector * (Float)(1.0 / scalar);
        }

        public static Vector3F operator *(Vector3F vector, Matrix3F matrix)
        {
            return matrix.Transform(vector);
        }

        public static Vector3F Multiply(Vector3F vector, Matrix3F matrix)
        {
            return matrix.Transform(vector);
        }

        public static Float DotProduct(Vector3F vector1, Vector3F vector2)
        {
            return DotProduct(ref vector1, ref vector2);
        }

        internal static Float DotProduct(ref Vector3F vector1, ref Vector3F vector2)
        {
            return vector1._x * vector2._x +
                   vector1._y * vector2._y +
                   vector1._z * vector2._z;
        }

        public static Vector3F CrossProduct(Vector3F vector1, Vector3F vector2)
        {
            Vector3F result;
            CrossProduct(ref vector1, ref vector2, out result);
            return result;
        }

        internal static void CrossProduct(ref Vector3F vector1, ref Vector3F vector2, out Vector3F result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static explicit operator Point3F(Vector3F vector)
        {
            return new Point3F(vector._x, vector._y, vector._z);
        }

        public static explicit operator VectorF(Vector3F vector)
        {
            return new VectorF(vector._x, vector._y);
        }

        public static explicit operator Size3F(Vector3F vector)
        {
            return new Size3F(Math.Abs(vector._x), Math.Abs(vector._y), Math.Abs(vector._z));
        }

        public static bool operator ==(Vector3F vector1, Vector3F vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y &&
                   vector1.Z == vector2.Z;
        }

        public static bool operator !=(Vector3F vector1, Vector3F vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool Equals(Vector3F vector1, Vector3F vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y) &&
                   vector1.Z.Equals(vector2.Z);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Vector3F))
            {
                return false;
            }

            Vector3F value = (Vector3F)o;
            return Vector3F.Equals(this, value);
        }

        public bool Equals(Vector3F value)
        {
            return Vector3F.Equals(this, value);
        }

        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode() ^
                   Z.GetHashCode();
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

        public Float Z
        {
            get
            {
                return _z;
            }

            set
            {
                _z = value;
            }

        }

        internal Float _x;
        internal Float _y;
        internal Float _z;

        public override string ToString()
        {
            return string.Format($"{_x}, {_y}, {_z}");
        }
    }
}
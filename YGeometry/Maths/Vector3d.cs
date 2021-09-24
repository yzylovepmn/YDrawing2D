using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace YGeometry.Maths
{
    [Serializable]
    public struct Vector3D : IEquatable<Vector3D>
    {
        public Vector3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public bool IsZero { get { return _x == 0 && _y == 0 && _z == 0; } }

        public double Length
        {
            get
            {
                return (double)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }

        public double LengthSquared
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
            double m = Math.Abs(_x);
            double absy = Math.Abs(_y);
            double absz = Math.Abs(_z);
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

            double length = (double)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            this /= length;
        }

        public static double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            double ratio = DotProduct(vector1, vector2);

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

            return MathUtil.RadiansToDegrees(theta);
        }

        public static Vector3D operator -(Vector3D vector)
        {
            return new Vector3D(-vector._x, -vector._y, -vector._z);
        }

        public void Negate()
        {
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        public static Vector3D Add(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        public static Vector3D Subtract(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        public static Vector3D operator *(Vector3D vector, double scalar)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3D Multiply(Vector3D vector, double scalar)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3D operator *(double scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3D Multiply(double scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        public static Vector3D operator /(Vector3D vector, double scalar)
        {
            return vector * (double)(1.0 / scalar);
        }

        public static Vector3D Divide(Vector3D vector, double scalar)
        {
            return vector * (double)(1.0 / scalar);
        }

        public static double DotProduct(Vector3D vector1, Vector3D vector2)
        {
            return DotProduct(ref vector1, ref vector2);
        }

        internal static double DotProduct(ref Vector3D vector1, ref Vector3D vector2)
        {
            return vector1._x * vector2._x +
                   vector1._y * vector2._y +
                   vector1._z * vector2._z;
        }

        public static Vector3D CrossProduct(Vector3D vector1, Vector3D vector2)
        {
            Vector3D result;
            CrossProduct(ref vector1, ref vector2, out result);
            return result;
        }

        internal static void CrossProduct(ref Vector3D vector1, ref Vector3D vector2, out Vector3D result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static bool operator ==(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y &&
                   vector1.Z == vector2.Z;
        }

        public static bool operator !=(Vector3D vector1, Vector3D vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool Equals(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y) &&
                   vector1.Z.Equals(vector2.Z);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Vector3D))
            {
                return false;
            }

            Vector3D value = (Vector3D)o;
            return Vector3D.Equals(this, value);
        }

        public bool Equals(Vector3D value)
        {
            return Vector3D.Equals(this, value);
        }

        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return _x.GetHashCode() ^
                   _y.GetHashCode() ^
                   _z.GetHashCode();
        }

        public double X
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

        public double Y
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

        public double Z
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

        private double _x;
        private double _y;
        private double _z;

        public override string ToString()
        {
            return string.Format($"{_x}, {_y}, {_z}");
        }
    }
}
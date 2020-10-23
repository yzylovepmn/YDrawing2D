using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Float = System.Single;

namespace YOpenGL
{
    [Serializable]
    public struct Point3F
    {
        public Point3F(Float x, Float y, Float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public void Offset(Float offsetX, Float offsetY, Float offsetZ)
        {
            _x += offsetX;
            _y += offsetY;
            _z += offsetZ;
        }

        public static Point3F operator +(Point3F point, Vector3F vector)
        {
            return new Point3F(point._x + vector._x,
                               point._y + vector._y,
                               point._z + vector._z);
        }

        public static Point3F Add(Point3F point, Vector3F vector)
        {
            return new Point3F(point._x + vector._x,
                               point._y + vector._y,
                               point._z + vector._z);
        }

        public static Point3F operator -(Point3F point, Vector3F vector)
        {
            return new Point3F(point._x - vector._x,
                               point._y - vector._y,
                               point._z - vector._z);
        }

        public static Point3F Subtract(Point3F point, Vector3F vector)
        {
            return new Point3F(point._x - vector._x,
                               point._y - vector._y,
                               point._z - vector._z);
        }

        public static Vector3F operator -(Point3F point1, Point3F point2)
        {
            return new Vector3F(point1._x - point2._x,
                                point1._y - point2._y,
                                point1._z - point2._z);
        }

        public static Vector3F Subtract(Point3F point1, Point3F point2)
        {
            Vector3F v = new Vector3F();
            Subtract(ref point1, ref point2, out v);
            return v;
        }

        internal static void Subtract(ref Point3F p1, ref Point3F p2, out Vector3F result)
        {
            result._x = p1._x - p2._x;
            result._y = p1._y - p2._y;
            result._z = p1._z - p2._z;
        }

        public static Point3F operator *(Point3F point, Matrix3F matrix)
        {
            return matrix.Transform(point);
        }

        public static Point3F Multiply(Point3F point, Matrix3F matrix)
        {
            return matrix.Transform(point);
        }

        public static explicit operator Vector3F(Point3F point)
        {
            return new Vector3F(point._x, point._y, point._z);
        }

        public static explicit operator PointF(Point3F point)
        {
            return new PointF(point._x, point._y);
        }

        public static explicit operator Point4F(Point3F point)
        {
            return new Point4F(point._x, point._y, point._z, (Float)1.0);
        }

        public static bool operator ==(Point3F point1, Point3F point2)
        {
            return point1.X == point2.X &&
                   point1.Y == point2.Y &&
                   point1.Z == point2.Z;
        }

        public static bool operator !=(Point3F point1, Point3F point2)
        {
            return !(point1 == point2);
        }

        public static bool Equals(Point3F point1, Point3F point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y) &&
                   point1.Z.Equals(point2.Z);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Point3F))
            {
                return false;
            }

            Point3F value = (Point3F)o;
            return Point3F.Equals(this, value);
        }

        public bool Equals(Point3F value)
        {
            return Point3F.Equals(this, value);
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
    }
}
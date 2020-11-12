using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Float = System.Single;

namespace YOpenGL
{
    public struct Rect3F
    {
        public Rect3F(Point3F location, Size3F size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = location._x;
                _y = location._y;
                _z = location._z;
                _sizeX = size._x;
                _sizeY = size._y;
                _sizeZ = size._z;
            }
        }

        public Rect3F(Float x, Float y, Float z, Float sizeX, Float sizeY, Float sizeZ)
        {
            if (sizeX < 0 || sizeY < 0 || sizeZ < 0)
            {
                throw new System.ArgumentException("");
            }

            _x = x;
            _y = y;
            _z = z;
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
        }

        internal Rect3F(Point3F point1, Point3F point2)
        {
            _x = Math.Min(point1._x, point2._x);
            _y = Math.Min(point1._y, point2._y);
            _z = Math.Min(point1._z, point2._z);
            _sizeX = Math.Max(point1._x, point2._x) - _x;
            _sizeY = Math.Max(point1._y, point2._y) - _y;
            _sizeZ = Math.Max(point1._z, point2._z) - _z;
        }

        internal Rect3F(Point3F point, Vector3F vector) : this(point, point + vector)
        {

        }

        public static Rect3F Empty
        {
            get
            {
                return s_empty;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _sizeX < 0;
            }
        }

        public bool IsVolumeEmpty
        {
            get
            {
                return _sizeX < 0 || (_sizeX == 0 && _sizeY == 0 && _sizeZ == 0);
            }
        }

        public Point3F Location
        {
            get
            {
                return new Point3F(_x, _y, _z);
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

                _x = value._x;
                _y = value._y;
                _z = value._z;
            }
        }

        public Size3F Size
        {
            get
            {
                if (IsEmpty)
                    return Size3F.Empty;
                else
                    return new Size3F(_sizeX, _sizeY, _sizeZ);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = s_empty;
                }
                else
                {
                    if (IsEmpty)
                    {
                        throw new System.InvalidOperationException("");
                    }

                    _sizeX = value._x;
                    _sizeY = value._y;
                    _sizeZ = value._z;
                }
            }
        }

        public Float SizeX
        {
            get
            {
                return _sizeX;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("");
                }

                _sizeX = value;
            }
        }

        public Float SizeY
        {
            get
            {
                return _sizeY;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("");
                }

                _sizeY = value;
            }
        }

        public Float SizeZ
        {
            get
            {
                return _sizeZ;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("");
                }

                _sizeZ = value;
            }
        }

        public Float X
        {
            get
            {
                return _x;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

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
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

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
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("");
                }

                _z = value;
            }
        }

        internal Float _x;
        internal Float _y;
        internal Float _z;
        internal Float _sizeX;
        internal Float _sizeY;
        internal Float _sizeZ;

        public bool Contains(Point3F point)
        {
            return Contains(point._x, point._y, point._z);
        }

        public bool Contains(Float x, Float y, Float z)
        {
            if (IsEmpty)
            {
                return false;
            }

            return ContainsInternal(x, y, z);
        }

        public bool Contains(Rect3F rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (_x <= rect._x &&
                    _y <= rect._y &&
                    _z <= rect._z &&
                    _x + _sizeX >= rect._x + rect._sizeX &&
                    _y + _sizeY >= rect._y + rect._sizeY &&
                    _z + _sizeZ >= rect._z + rect._sizeZ);
        }

        public bool IntersectsWith(Rect3F rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (rect._x <= (_x + _sizeX)) &&
                   ((rect._x + rect._sizeX) >= _x) &&
                   (rect._y <= (_y + _sizeY)) &&
                   ((rect._y + rect._sizeY) >= _y) &&
                   (rect._z <= (_z + _sizeZ)) &&
                   ((rect._z + rect._sizeZ) >= _z);
        }

        public void Intersect(Rect3F rect)
        {
            if (IsEmpty || rect.IsEmpty || !this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                Float x = Math.Max(_x, rect._x);
                Float y = Math.Max(_y, rect._y);
                Float z = Math.Max(_z, rect._z);
                _sizeX = Math.Min(_x + _sizeX, rect._x + rect._sizeX) - x;
                _sizeY = Math.Min(_y + _sizeY, rect._y + rect._sizeY) - y;
                _sizeZ = Math.Min(_z + _sizeZ, rect._z + rect._sizeZ) - z;

                _x = x;
                _y = y;
                _z = z;
            }
        }

        public static Rect3F Intersect(Rect3F rect1, Rect3F rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        public void Union(Rect3F rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                Float x = Math.Min(_x, rect._x);
                Float y = Math.Min(_y, rect._y);
                Float z = Math.Min(_z, rect._z);
                _sizeX = Math.Max(_x + _sizeX, rect._x + rect._sizeX) - x;
                _sizeY = Math.Max(_y + _sizeY, rect._y + rect._sizeY) - y;
                _sizeZ = Math.Max(_z + _sizeZ, rect._z + rect._sizeZ) - z;
                _x = x;
                _y = y;
                _z = z;
            }
        }

        public static Rect3F Union(Rect3F rect1, Rect3F rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        public void Union(Point3F point)
        {
            Union(new Rect3F(point, point));
        }

        public static Rect3F Union(Rect3F rect, Point3F point)
        {
            rect.Union(new Rect3F(point, point));
            return rect;
        }

        public void Offset(Vector3F offsetVector)
        {
            Offset(offsetVector._x, offsetVector._y, offsetVector._z);
        }

        public void Offset(Float offsetX, Float offsetY, Float offsetZ)
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("");
            }

            _x += offsetX;
            _y += offsetY;
            _z += offsetZ;
        }

        public static Rect3F Offset(Rect3F rect, Vector3F offsetVector)
        {
            rect.Offset(offsetVector._x, offsetVector._y, offsetVector._z);
            return rect;
        }

        public static Rect3F Offset(Rect3F rect, Float offsetX, Float offsetY, Float offsetZ)
        {
            rect.Offset(offsetX, offsetY, offsetZ);
            return rect;
        }

        internal readonly static Rect3F Infinite = CreateInfiniteRect3D();

        private bool ContainsInternal(Float x, Float y, Float z)
        {
            // We include points on the edge as "contained"
            return ((x >= _x) && (x <= _x + _sizeX) &&
                    (y >= _y) && (y <= _y + _sizeY) &&
                    (z >= _z) && (z <= _z + _sizeZ));
        }

        private static Rect3F CreateEmptyRect3D()
        {
            Rect3F empty = new Rect3F();
            empty._x = Float.PositiveInfinity;
            empty._y = Float.PositiveInfinity;
            empty._z = Float.PositiveInfinity;
            // Can't use setters because they throw on negative values
            empty._sizeX = Float.NegativeInfinity;
            empty._sizeY = Float.NegativeInfinity;
            empty._sizeZ = Float.NegativeInfinity;
            return empty;
        }

        private static Rect3F CreateInfiniteRect3D()
        {
            Rect3F infinite = new Rect3F();
            infinite._x = -float.MaxValue;
            infinite._y = -float.MaxValue;
            infinite._z = -float.MaxValue;
            infinite._sizeX = float.MaxValue;
            infinite._sizeY = float.MaxValue;
            infinite._sizeZ = float.MaxValue;
            return infinite;
        }

        private readonly static Rect3F s_empty = CreateEmptyRect3D();

        public static bool operator ==(Rect3F rect1, Rect3F rect2)
        {
            return rect1.X == rect2.X &&
                   rect1.Y == rect2.Y &&
                   rect1.Z == rect2.Z &&
                   rect1.SizeX == rect2.SizeX &&
                   rect1.SizeY == rect2.SizeY &&
                   rect1.SizeZ == rect2.SizeZ;
        }

        public static bool operator !=(Rect3F rect1, Rect3F rect2)
        {
            return !(rect1 == rect2);
        }

        public static bool Equals(Rect3F rect1, Rect3F rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            else
            {
                return rect1.X.Equals(rect2.X) &&
                       rect1.Y.Equals(rect2.Y) &&
                       rect1.Z.Equals(rect2.Z) &&
                       rect1.SizeX.Equals(rect2.SizeX) &&
                       rect1.SizeY.Equals(rect2.SizeY) &&
                       rect1.SizeZ.Equals(rect2.SizeZ);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Rect3F))
            {
                return false;
            }

            Rect3F value = (Rect3F)o;
            return Rect3F.Equals(this, value);
        }

        public bool Equals(Rect3F value)
        {
            return Rect3F.Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return X.GetHashCode() ^
                       Y.GetHashCode() ^
                       Z.GetHashCode() ^
                       SizeX.GetHashCode() ^
                       SizeY.GetHashCode() ^
                       SizeZ.GetHashCode();
            }
        }
    }
}
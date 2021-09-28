using System;
using System.Windows;
using System.Xml.Linq;
using Float = System.Single;

namespace YRenderingSystem
{
    [Serializable]
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

        public static XElement GetData(PointF p, string name)
        {
            var ele = new XElement(name);
            ele.Add(new XElement("X", p.X));
            ele.Add(new XElement("Y", p.Y));
            return ele;
        }

        public static PointF LoadData(XElement ele)
        {
            var p = new PointF();
            p.X = float.Parse(ele.Element("X").Value);
            p.Y = float.Parse(ele.Element("Y").Value);
            return p;
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

        public override string ToString()
        {
            return string.Format($"{_x}, {_y}");
        }

        public string ToString(int accuracy)
        {
            return string.Format("[{0}, {1}]", _x.ToString(string.Format("f{0}", accuracy)), _y.ToString(string.Format("f{0}", accuracy)));
        }
    }
}
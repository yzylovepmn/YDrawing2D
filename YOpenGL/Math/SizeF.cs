using System;
using System.Windows;
using Float = System.Single;

namespace YOpenGL
{
    public struct SizeF
    {
        public SizeF(Float width, Float height)
        {
            if (width < 0 || height < 0)
            {
                throw new System.ArgumentException("size can not be negetive!");
            }

            _width = width;
            _height = height;
        }

        public static SizeF Empty
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
                return _width < 0;
            }
        }

        public Float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException();
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("width can not be nagetive!");
                }

                _width = value;
            }
        }

        public Float Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException();
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("heigth can not be nagetive!");
                }

                _height = value;
            }
        }

        public static explicit operator VectorF(SizeF size)
        {
            return new VectorF(size._width, size._height);
        }

        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size._width, size._height);
        }

        static private SizeF CreateEmptySize()
        {
            SizeF size = new SizeF();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            size._width = Float.NegativeInfinity;
            size._height = Float.NegativeInfinity;
            return size;
        }
        private readonly static SizeF s_empty = CreateEmptySize();

        public static bool operator ==(SizeF size1, SizeF size2)
        {
            return size1.Width == size2.Width &&
                   size1.Height == size2.Height;
        }

        public static bool operator !=(SizeF size1, SizeF size2)
        {
            return !(size1 == size2);
        }
        public static bool Equals(SizeF size1, SizeF size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            else
            {
                return size1.Width.Equals(size2.Width) &&
                       size1.Height.Equals(size2.Height);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is SizeF))
            {
                return false;
            }

            SizeF value = (SizeF)o;
            return SizeF.Equals(this, value);
        }

        public bool Equals(SizeF value)
        {
            return SizeF.Equals(this, value);
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
                return Width.GetHashCode() ^
                       Height.GetHashCode();
            }
        }

        internal Float _width;
        internal Float _height;
    }
}
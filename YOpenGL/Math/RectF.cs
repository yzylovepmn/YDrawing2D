using System;
using System.Diagnostics;
using Float = System.Single;

namespace YOpenGL
{
    public struct RectF
    {
        public RectF(PointF location,
                    SizeF size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = location._x;
                _y = location._y;
                _width = size._width;
                _height = size._height;
            }
        }

        public RectF(Float x,
                    Float y,
                    Float width,
                    Float height)
        {
            if (width < 0 || height < 0)
            {
                throw new System.ArgumentException();
            }

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public RectF(PointF point1,
                    PointF point2)
        {
            _x = Math.Min(point1._x, point2._x);
            _y = Math.Min(point1._y, point2._y);

            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            _width = Math.Max(Math.Max(point1._x, point2._x) - _x, 0);
            _height = Math.Max(Math.Max(point1._y, point2._y) - _y, 0);
        }

        public RectF(PointF point,
                    VectorF vector) : this(point, point + vector)
        {
        }

        public RectF(SizeF size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = _y = 0;
                _width = size.Width;
                _height = size.Height;
            }
        }

        public static RectF Empty
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
                // The funny width and height tests are to handle NaNs
                Debug.Assert((!(_width < 0) && !(_height < 0)) || (this == Empty));

                return _width < 0;
            }
        }

        public PointF Location
        {
            get
            {
                return new PointF(_x, _y);
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException();
                }

                _x = value._x;
                _y = value._y;
            }
        }

        public SizeF Size
        {
            get
            {
                if (IsEmpty)
                    return SizeF.Empty;
                return new SizeF(_width, _height);
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
                        throw new System.InvalidOperationException();
                    }

                    _width = value._width;
                    _height = value._height;
                }
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
                    throw new System.InvalidOperationException();
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
                    throw new System.InvalidOperationException();
                }

                _y = value;
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
                    throw new System.ArgumentException();
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
                    throw new System.ArgumentException();
                }

                _height = value;
            }
        }

        public Float Left
        {
            get
            {
                return _x;
            }
        }

        public Float Top
        {
            get
            {
                return _y;
            }
        }

        public Float Right
        {
            get
            {
                if (IsEmpty)
                {
                    return Float.NegativeInfinity;
                }

                return _x + _width;
            }
        }

        public Float Bottom
        {
            get
            {
                if (IsEmpty)
                {
                    return Float.NegativeInfinity;
                }

                return _y + _height;
            }
        }

        public PointF TopLeft
        {
            get
            {
                return new PointF(Left, Top);
            }
        }

        public PointF TopRight
        {
            get
            {
                return new PointF(Right, Top);
            }
        }

        public PointF BottomLeft
        {
            get
            {
                return new PointF(Left, Bottom);
            }
        }

        public PointF BottomRight
        {
            get
            {
                return new PointF(Right, Bottom);
            }
        }

        public bool Contains(PointF point)
        {
            return Contains(point._x, point._y);
        }

        public bool Contains(Float x, Float y)
        {
            if (IsEmpty)
            {
                return false;
            }

            return ContainsInternal(x, y);
        }

        public bool Contains(RectF rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (_x <= rect._x &&
                    _y <= rect._y &&
                    _x + _width >= rect._x + rect._width &&
                    _y + _height >= rect._y + rect._height);
        }

        public bool IntersectsWith(RectF rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (rect.Left <= Right) &&
                   (rect.Right >= Left) &&
                   (rect.Top <= Bottom) &&
                   (rect.Bottom >= Top);
        }

        public void Intersect(RectF rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                Float left = Math.Max(Left, rect.Left);
                Float top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0);

                _x = left;
                _y = top;
            }
        }

        public static RectF Intersect(RectF rect1, RectF rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        public void Union(RectF rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                Float left = Math.Min(Left, rect.Left);
                Float top = Math.Min(Top, rect.Top);


                // We need this check so that the math does not result in NaN
                if ((rect.Width == Float.PositiveInfinity) || (Width == Float.PositiveInfinity))
                {
                    _width = Float.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                    Float maxRight = Math.Max(Right, rect.Right);
                    _width = Math.Max(maxRight - left, 0);
                }

                // We need this check so that the math does not result in NaN
                if ((rect.Height == Float.PositiveInfinity) || (Height == Float.PositiveInfinity))
                {
                    _height = Float.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                    Float maxBottom = Math.Max(Bottom, rect.Bottom);
                    _height = Math.Max(maxBottom - top, 0);
                }

                _x = left;
                _y = top;
            }
        }

        public static RectF Union(RectF rect1, RectF rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        public void Union(PointF point)
        {
            Union(new RectF(point, point));
        }

        public static RectF Union(RectF rect, PointF point)
        {
            rect.Union(new RectF(point, point));
            return rect;
        }

        public void Offset(VectorF offsetVector)
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException();
            }

            _x += offsetVector._x;
            _y += offsetVector._y;
        }

        public void Offset(Float offsetX, Float offsetY)
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException();
            }

            _x += offsetX;
            _y += offsetY;
        }

        public static RectF Offset(RectF rect, VectorF offsetVector)
        {
            rect.Offset(offsetVector.X, offsetVector.Y);
            return rect;
        }

        public static RectF Offset(RectF rect, Float offsetX, Float offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        public void Inflate(SizeF size)
        {
            Inflate(size._width, size._height);
        }

        public void Inflate(Float width, Float height)
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException();
            }

            _x -= width;
            _y -= height;

            // Do two additions rather than multiplication by 2 to avoid spurious overflow
            // That is: (A + 2 * B) != ((A + B) + B) if 2*B overflows.
            // Note that multiplication by 2 might work in this case because A should start
            // positive & be "clamped" to positive after, but consider A = Inf & B = -MAX.
            _width += width;
            _width += width;
            _height += height;
            _height += height;

            // We catch the case of inflation by less than -width/2 or -height/2 here.  This also
            // maintains the invariant that either the Rect is Empty or _width and _height are
            // non-negative, even if the user parameters were NaN, though this isn't strictly maintained
            // by other methods.
            if (!(_width >= 0 && _height >= 0))
            {
                this = s_empty;
            }
        }

        public static RectF Inflate(RectF rect, SizeF size)
        {
            rect.Inflate(size._width, size._height);
            return rect;
        }

        public static RectF Inflate(RectF rect, Float width, Float height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        public static RectF Transform(RectF rect, MatrixF matrix)
        {
            MathUtil.TransformRect(ref rect, ref matrix);
            return rect;
        }

        public void Transform(MatrixF matrix)
        {
            MathUtil.TransformRect(ref this, ref matrix);
        }

        public void Scale(Float scaleX, Float scaleY)
        {
            if (IsEmpty)
            {
                return;
            }

            _x *= scaleX;
            _y *= scaleY;
            _width *= scaleX;
            _height *= scaleY;

            // If the scale in the X dimension is negative, we need to normalize X and Width
            if (scaleX < 0)
            {
                // Make X the left-most edge again
                _x += _width;

                // and make Width positive
                _width *= -1;
            }

            // Do the same for the Y dimension
            if (scaleY < 0)
            {
                // Make Y the top-most edge again
                _y += _height;

                // and make Height positive
                _height *= -1;
            }
        }

        private bool ContainsInternal(Float x, Float y)
        {
            return ((x >= _x) && (x - _width <= _x) &&
                    (y >= _y) && (y - _height <= _y));
        }

        static private RectF CreateEmptyRect()
        {
            RectF rect = new RectF();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            rect._x = Float.PositiveInfinity;
            rect._y = Float.PositiveInfinity;
            rect._width = Float.NegativeInfinity;
            rect._height = Float.NegativeInfinity;
            return rect;
        }

        private readonly static RectF s_empty = CreateEmptyRect();

        public static bool operator ==(RectF rect1, RectF rect2)
        {
            return rect1.X == rect2.X &&
                   rect1.Y == rect2.Y &&
                   rect1.Width == rect2.Width &&
                   rect1.Height == rect2.Height;
        }

        public static bool operator !=(RectF rect1, RectF rect2)
        {
            return !(rect1 == rect2);
        }

        public static bool Equals(RectF rect1, RectF rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            else
            {
                return rect1.X.Equals(rect2.X) &&
                       rect1.Y.Equals(rect2.Y) &&
                       rect1.Width.Equals(rect2.Width) &&
                       rect1.Height.Equals(rect2.Height);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is RectF))
            {
                return false;
            }

            RectF value = (RectF)o;
            return RectF.Equals(this, value);
        }

        public bool Equals(RectF value)
        {
            return RectF.Equals(this, value);
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
                       Width.GetHashCode() ^
                       Height.GetHashCode();
            }
        }

        internal Float _x;
        internal Float _y;
        internal Float _width;
        internal Float _height;
    }
}
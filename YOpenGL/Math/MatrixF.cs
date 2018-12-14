using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using Float = System.Single;

namespace YOpenGL
{
    internal enum MatrixTypes
    {
        TRANSFORM_IS_IDENTITY = 0,
        TRANSFORM_IS_TRANSLATION = 1,
        TRANSFORM_IS_SCALING = 2,
        TRANSFORM_IS_UNKNOWN = 4
    }

    public struct MatrixF
    {
        private static MatrixF s_identity = CreateIdentity();

        public MatrixF(Float m11, Float m12,
                      Float m21, Float m22,
                      Float offsetX, Float offsetY)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;

            DeriveMatrixType();
        }

        public static MatrixF Identity
        {
            get
            {
                return s_identity;
            }
        }

        public void SetIdentity()
        {
            _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        }

        public bool IsIdentity
        {
            get
            {
                return (_type == MatrixTypes.TRANSFORM_IS_IDENTITY ||
                        (_m11 == 1 && _m12 == 0 && _m21 == 0 && _m22 == 1 && _offsetX == 0 && _offsetY == 0));
            }
        }

        public static MatrixF operator *(MatrixF trans1, MatrixF trans2)
        {
            MathUtil.MultiplyMatrix(ref trans1, ref trans2);
#if DEBUG
            trans1.Debug_CheckType();
#endif
            return trans1;
        }

        public static MatrixF Multiply(MatrixF trans1, MatrixF trans2)
        {
            MathUtil.MultiplyMatrix(ref trans1, ref trans2);
#if DEBUG
            trans1.Debug_CheckType();
#endif
            return trans1;
        }

        public void Append(MatrixF matrix)
        {
            this *= matrix;
        }

        public void Prepend(MatrixF matrix)
        {
            this = matrix * this;
        }

        public void Rotate(Float angle)
        {
            angle %= 360.0F; // Doing the modulo before converting to radians reduces total error
            this *= CreateRotationRadians(angle * (Float)(Math.PI / 180.0));
        }

        public void RotatePrepend(Float angle)
        {
            angle %= 360.0F; // Doing the modulo before converting to radians reduces total error
            this = CreateRotationRadians(angle * (Float)(Math.PI / 180.0)) * this;
        }

        public void RotateAt(Float angle, Float centerX, Float centerY)
        {
            angle %= 360.0F; // Doing the modulo before converting to radians reduces total error
            this *= CreateRotationRadians(angle * (Float)(Math.PI / 180.0), centerX, centerY);
        }

        public void RotateAtPrepend(Float angle, Float centerX, Float centerY)
        {
            angle %= 360.0F; // Doing the modulo before converting to radians reduces total error
            this = CreateRotationRadians(angle * (Float)(Math.PI / 180.0), centerX, centerY) * this;
        }

        public void Scale(Float scaleX, Float scaleY)
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        public void ScalePrepend(Float scaleX, Float scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        public void ScaleAt(Float scaleX, Float scaleY, Float centerX, Float centerY)
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        public void ScaleAtPrepend(Float scaleX, Float scaleY, Float centerX, Float centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        public void Skew(Float skewX, Float skewY)
        {
            skewX %= 360;
            skewY %= 360;
            this *= CreateSkewRadians(skewX * (Float)(Math.PI / 180.0),
                                      skewY * (Float)(Math.PI / 180.0));
        }

        public void SkewPrepend(Float skewX, Float skewY)
        {
            skewX %= 360;
            skewY %= 360;
            this = CreateSkewRadians(skewX * (Float)(Math.PI / 180.0),
                                     skewY * (Float)(Math.PI / 180.0)) * this;
        }

        public void Translate(Float offsetX, Float offsetY)
        {
            //
            // / a b 0 \   / 1 0 0 \    / a      b       0 \
            // | c d 0 | * | 0 1 0 | = |  c      d       0 |
            // \ e f 1 /   \ x y 1 /    \ e+x    f+y     1 /
            //
            // (where e = _offsetX and f == _offsetY)
            //

            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                // Values would be incorrect if matrix was created using default constructor.
                // or if SetIdentity was called on a matrix which had values.
                //
                SetMatrix(1, 0,
                          0, 1,
                          offsetX, offsetY,
                          MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else if (_type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
            }
            else
            {
                _offsetX += offsetX;
                _offsetY += offsetY;

                // If matrix wasn't unknown we added a translation
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

#if DEBUG
            Debug_CheckType();
#endif
        }

        public void TranslatePrepend(Float offsetX, Float offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        public PointF Transform(PointF point)
        {
            PointF newPoint = point;
            MultiplyPoint(ref newPoint._x, ref newPoint._y);
            return newPoint;
        }

        public void Transform(PointF[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]._x, ref points[i]._y);
                }
            }
        }

        public VectorF Transform(VectorF vector)
        {
            VectorF newVector = vector;
            MultiplyVector(ref newVector._x, ref newVector._y);
            return newVector;
        }

        public void Transform(VectorF[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
                }
            }
        }

        public Float Determinant
        {
            get
            {
                switch (_type)
                {
                    case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return 1.0F;
                    case MatrixTypes.TRANSFORM_IS_SCALING:
                    case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return (_m11 * _m22);
                    default:
                        return (_m11 * _m22) - (_m12 * _m21);
                }
            }
        }

        public bool HasInverse
        {
            get
            {
                return !MathUtil.IsZero(Determinant);
            }
        }

        public void Invert()
        {
            Float determinant = Determinant;

            if (MathUtil.IsZero(determinant))
            {
                throw new System.InvalidOperationException();
            }

            // Inversion does not change the type of a matrix.
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    {
                        _m11 = 1.0F / _m11;
                        _m22 = 1.0F / _m22;
                    }
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    _offsetX = -_offsetX;
                    _offsetY = -_offsetY;
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    {
                        _m11 = 1.0F / _m11;
                        _m22 = 1.0F / _m22;
                        _offsetX = -_offsetX * _m11;
                        _offsetY = -_offsetY * _m22;
                    }
                    break;
                default:
                    {
                        Float invdet = 1.0F / determinant;
                        SetMatrix(_m22 * invdet,
                                  -_m12 * invdet,
                                  -_m21 * invdet,
                                  _m11 * invdet,
                                  (_m21 * _offsetY - _offsetX * _m22) * invdet,
                                  (_offsetX * _m12 - _m11 * _offsetY) * invdet,
                                  MatrixTypes.TRANSFORM_IS_UNKNOWN);
                    }
                    break;
            }
        }

        public Float M11
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0F;
                }
                else
                {
                    return _m11;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(value, 0,
                              0, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    _m11 = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }

        public Float M12
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _m12;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, value,
                              0, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    _m12 = value;
                    _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }

        public Float M21
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _m21;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              value, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    _m21 = value;
                    _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }

        public Float M22
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0F;
                }
                else
                {
                    return _m22;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, value,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    _m22 = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }

        public Float OffsetX
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _offsetX;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, 1,
                              value, 0,
                              MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    _offsetX = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }

        public Float OffsetY
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _offsetY;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, 1,
                              0, value,
                              MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    _offsetY = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }

        internal void MultiplyVector(ref Float x, ref Float y)
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x *= _m11;
                    y *= _m22;
                    break;
                default:
                    Float xadd = y * _m21;
                    Float yadd = x * _m12;
                    x *= _m11;
                    x += xadd;
                    y *= _m22;
                    y += yadd;
                    break;
            }
        }

        internal void MultiplyPoint(ref Float x, ref Float y)
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    return;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x += _offsetX;
                    y += _offsetY;
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    x *= _m11;
                    y *= _m22;
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x *= _m11;
                    x += _offsetX;
                    y *= _m22;
                    y += _offsetY;
                    break;
                default:
                    Float xadd = y * _m21 + _offsetX;
                    Float yadd = x * _m12 + _offsetY;
                    x *= _m11;
                    x += xadd;
                    y *= _m22;
                    y += yadd;
                    break;
            }
        }

        internal static MatrixF CreateRotationRadians(Float angle)
        {
            return CreateRotationRadians(angle, /* centerX = */ 0, /* centerY = */ 0);
        }

        internal static MatrixF CreateRotationRadians(Float angle, Float centerX, Float centerY)
        {
            MatrixF matrix = new MatrixF();

            Float sin = (Float)Math.Sin(angle);
            Float cos = (Float)Math.Cos(angle);
            Float dx = (centerX * (1.0F - cos)) + (centerY * sin);
            Float dy = (centerY * (1.0F - cos)) - (centerX * sin);

            matrix.SetMatrix(cos, sin,
                              -sin, cos,
                              dx, dy,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);

            return matrix;
        }

        internal static MatrixF CreateScaling(Float scaleX, Float scaleY, Float centerX, Float centerY)
        {
            MatrixF matrix = new MatrixF();

            matrix.SetMatrix(scaleX, 0,
                             0, scaleY,
                             centerX - scaleX * centerX, centerY - scaleY * centerY,
                             MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION);

            return matrix;
        }

        internal static MatrixF CreateScaling(Float scaleX, Float scaleY)
        {
            MatrixF matrix = new MatrixF();
            matrix.SetMatrix(scaleX, 0,
                             0, scaleY,
                             0, 0,
                             MatrixTypes.TRANSFORM_IS_SCALING);
            return matrix;
        }

        internal static MatrixF CreateSkewRadians(Float skewX, Float skewY)
        {
            MatrixF matrix = new MatrixF();

            matrix.SetMatrix(1.0F, (Float)Math.Tan(skewY),
                             (Float)Math.Tan(skewX), 1.0F,
                             0.0F, 0.0F,
                             MatrixTypes.TRANSFORM_IS_UNKNOWN);

            return matrix;
        }

        internal static MatrixF CreateTranslation(Float offsetX, Float offsetY)
        {
            MatrixF matrix = new MatrixF();

            matrix.SetMatrix(1, 0,
                             0, 1,
                             offsetX, offsetY,
                             MatrixTypes.TRANSFORM_IS_TRANSLATION);

            return matrix;
        }

        private static MatrixF CreateIdentity()
        {
            MatrixF matrix = new MatrixF();
            matrix.SetMatrix(1, 0,
                             0, 1,
                             0, 0,
                             MatrixTypes.TRANSFORM_IS_IDENTITY);
            return matrix;
        }

        private void SetMatrix(Float m11, Float m12,
                               Float m21, Float m22,
                               Float offsetX, Float offsetY,
                               MatrixTypes type)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = type;
        }

        private void DeriveMatrixType()
        {
            _type = 0;

            // Now classify our matrix.
            if (!(_m21 == 0 && _m12 == 0))
            {
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                return;
            }

            if (!(_m11 == 1 && _m22 == 1))
            {
                _type = MatrixTypes.TRANSFORM_IS_SCALING;
            }

            if (!(_offsetX == 0 && _offsetY == 0))
            {
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

            if (0 == (_type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)))
            {
                // We have an identity matrix.
                _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
            return;
        }

        private void Debug_CheckType()
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    return;
                case MatrixTypes.TRANSFORM_IS_UNKNOWN:
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    Debug.Assert(_m21 == 0);
                    Debug.Assert(_m12 == 0);
                    Debug.Assert(_offsetX == 0);
                    Debug.Assert(_offsetY == 0);
                    return;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    Debug.Assert(_m21 == 0);
                    Debug.Assert(_m12 == 0);
                    Debug.Assert(_m11 == 1);
                    Debug.Assert(_m22 == 1);
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    Debug.Assert(_m21 == 0);
                    Debug.Assert(_m12 == 0);
                    return;
                default:
                    Debug.Assert(false);
                    return;
            }
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return _type == MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
        }

        private const int c_identityHashCode = 0;

        internal Float _m11;
        internal Float _m12;
        internal Float _m21;
        internal Float _m22;
        internal Float _offsetX;
        internal Float _offsetY;
        internal MatrixTypes _type;

        public static bool operator ==(MatrixF matrix1, MatrixF matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11 == matrix2.M11 &&
                       matrix1.M12 == matrix2.M12 &&
                       matrix1.M21 == matrix2.M21 &&
                       matrix1.M22 == matrix2.M22 &&
                       matrix1.OffsetX == matrix2.OffsetX &&
                       matrix1.OffsetY == matrix2.OffsetY;
            }
        }

        public static bool operator !=(MatrixF matrix1, MatrixF matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static implicit operator Matrix(MatrixF matrix)
        {
            return new Matrix(matrix._m11, matrix._m12, matrix._m21, matrix._m22, matrix._offsetX, matrix._offsetY);
        }

        public static explicit operator MatrixF(Matrix matrix)
        {
            return new MatrixF((Float)matrix.M11, (Float)matrix.M12, (Float)matrix.M21, (Float)matrix.M22, (Float)matrix.OffsetX, (Float)matrix.OffsetY);
        }

        public static XElement GetData(MatrixF m, string name)
        {
            var ele = new XElement(name);
            ele.Add(new XElement("M11", m.M11));
            ele.Add(new XElement("M12", m.M12));
            ele.Add(new XElement("M21", m.M21));
            ele.Add(new XElement("M22", m.M22));
            ele.Add(new XElement("OffsetX", m.OffsetX));
            ele.Add(new XElement("OffsetY", m.OffsetY));
            return ele;
        }

        public static MatrixF LoadData(XElement ele)
        {
            var m = new MatrixF();
            m.M11 = float.Parse(ele.Element("M11").Value);
            m.M12 = float.Parse(ele.Element("M12").Value);
            m.M21 = float.Parse(ele.Element("M21").Value);
            m.M22 = float.Parse(ele.Element("M22").Value);
            m.OffsetX = float.Parse(ele.Element("OffsetX").Value);
            m.OffsetY = float.Parse(ele.Element("OffsetY").Value);
            return m;
        }

        public static bool Equals(MatrixF matrix1, MatrixF matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11.Equals(matrix2.M11) &&
                       matrix1.M12.Equals(matrix2.M12) &&
                       matrix1.M21.Equals(matrix2.M21) &&
                       matrix1.M22.Equals(matrix2.M22) &&
                       matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                       matrix1.OffsetY.Equals(matrix2.OffsetY);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is MatrixF))
            {
                return false;
            }

            MatrixF value = (MatrixF)o;
            return MatrixF.Equals(this, value);
        }

        public bool Equals(MatrixF value)
        {
            return MatrixF.Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return c_identityHashCode;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return M11.GetHashCode() ^
                       M12.GetHashCode() ^
                       M21.GetHashCode() ^
                       M22.GetHashCode() ^
                       OffsetX.GetHashCode() ^
                       OffsetY.GetHashCode();
            }
        }
    }
}
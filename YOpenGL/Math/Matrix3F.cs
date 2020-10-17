using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Float = System.Single;

namespace YOpenGL
{
    /// [ m11      m12      m13      m14 ]
    /// [ m21      m22      m23      m24 ]
    /// [ m31      m32      m33      m34 ]
    /// [ offsetX  offsetY  offsetZ  m44 ]
    /// <summary>
    /// 左手坐标系
    /// </summary>
    public struct Matrix3F
    {
        public Matrix3F(Float m11, Float m12, Float m13, Float m14,
                           Float m21, Float m22, Float m23, Float m24,
                           Float m31, Float m32, Float m33, Float m34,
                           Float offsetX, Float offsetY, Float offsetZ, Float m44)
        {
            _m11 = m11;
            _m12 = m12;
            _m13 = m13;
            _m14 = m14;
            _m21 = m21;
            _m22 = m22;
            _m23 = m23;
            _m24 = m24;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
            _m34 = m34;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _offsetZ = offsetZ;
            _m44 = m44;

            // This is not known to be an identity matrix so we need
            // to change our flag from it's default value.  We use the field
            // in the ctor rather than the property because of CS0188.
            _isNotKnownToBeIdentity = true;
        }

        public static Matrix3F Identity
        {
            get
            {
                return s_identity;
            }
        }

        public void SetIdentity()
        {
            this = s_identity;
        }

        public bool IsIdentity
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }
                else
                {
                    // Otherwise check all elements one by one.
                    if (_m11 == 1.0 && _m12 == 0.0 && _m13 == 0.0 && _m14 == 0.0 &&
                        _m21 == 0.0 && _m22 == 1.0 && _m23 == 0.0 && _m24 == 0.0 &&
                        _m31 == 0.0 && _m32 == 0.0 && _m33 == 1.0 && _m34 == 0.0 &&
                        _offsetX == 0.0 && _offsetY == 0.0 && _offsetZ == 0.0 && _m44 == 1.0)
                    {
                        // If matrix is identity, cache this with the IsDistinguishedIdentity flag.
                        IsDistinguishedIdentity = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void Prepend(Matrix3F matrix)
        {
            this = matrix * this;
        }

        public void Append(Matrix3F matrix)
        {
            this *= matrix;
        }

        public void Rotate(QuaternionF quaternion)
        {
            Point3F center = new Point3F();

            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotatePrepend(QuaternionF quaternion)
        {
            Point3F center = new Point3F();

            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void RotateAt(QuaternionF quaternion, Point3F center)
        {
            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotateAtPrepend(QuaternionF quaternion, Point3F center)
        {
            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void Scale(Vector3F scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
            }
            else
            {
                _m11 *= scale.X; _m12 *= scale.Y; _m13 *= scale.Z;
                _m21 *= scale.X; _m22 *= scale.Y; _m23 *= scale.Z;
                _m31 *= scale.X; _m32 *= scale.Y; _m33 *= scale.Z;
                _offsetX *= scale.X; _offsetY *= scale.Y; _offsetZ *= scale.Z;
            }
        }

        public void ScalePrepend(Vector3F scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
            }
            else
            {
                _m11 *= scale.X; _m12 *= scale.X; _m13 *= scale.X; _m14 *= scale.X;
                _m21 *= scale.Y; _m22 *= scale.Y; _m23 *= scale.Y; _m24 *= scale.Y;
                _m31 *= scale.Z; _m32 *= scale.Z; _m33 *= scale.Z; _m34 *= scale.Z;
            }
        }

        public void ScaleAt(Vector3F scale, Point3F center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
            }
            else
            {
                Float tmp = _m14 * center.X;
                _m11 = tmp + scale.X * (_m11 - tmp);
                tmp = _m14 * center.Y;
                _m12 = tmp + scale.Y * (_m12 - tmp);
                tmp = _m14 * center.Z;
                _m13 = tmp + scale.Z * (_m13 - tmp);

                tmp = _m24 * center.X;
                _m21 = tmp + scale.X * (_m21 - tmp);
                tmp = _m24 * center.Y;
                _m22 = tmp + scale.Y * (_m22 - tmp);
                tmp = _m24 * center.Z;
                _m23 = tmp + scale.Z * (_m23 - tmp);

                tmp = _m34 * center.X;
                _m31 = tmp + scale.X * (_m31 - tmp);
                tmp = _m34 * center.Y;
                _m32 = tmp + scale.Y * (_m32 - tmp);
                tmp = _m34 * center.Z;
                _m33 = tmp + scale.Z * (_m33 - tmp);

                tmp = _m44 * center.X;
                _offsetX = tmp + scale.X * (_offsetX - tmp);
                tmp = _m44 * center.Y;
                _offsetY = tmp + scale.Y * (_offsetY - tmp);
                tmp = _m44 * center.Z;
                _offsetZ = tmp + scale.Z * (_offsetZ - tmp);
            }
        }

        public void ScaleAtPrepend(Vector3F scale, Point3F center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
            }
            else
            {
                Float csx = center.X - center.X * scale.X;
                Float csy = center.Y - center.Y * scale.Y;
                Float csz = center.Z - center.Z * scale.Z;

                // We have to set the bottom row first because it depends
                // on values that will change
                _offsetX += _m11 * csx + _m21 * csy + _m31 * csz;
                _offsetY += _m12 * csx + _m22 * csy + _m32 * csz;
                _offsetZ += _m13 * csx + _m23 * csy + _m33 * csz;
                _m44 += _m14 * csx + _m24 * csy + _m34 * csz;

                _m11 *= scale.X; _m12 *= scale.X; _m13 *= scale.X; _m14 *= scale.X;
                _m21 *= scale.Y; _m22 *= scale.Y; _m23 *= scale.Y; _m24 *= scale.Y;
                _m31 *= scale.Z; _m32 *= scale.Z; _m33 *= scale.Z; _m34 *= scale.Z;
            }
        }

        public void Translate(Vector3F offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
            }
            else
            {
                _m11 += _m14 * offset.X; _m12 += _m14 * offset.Y; _m13 += _m14 * offset.Z;
                _m21 += _m24 * offset.X; _m22 += _m24 * offset.Y; _m23 += _m24 * offset.Z;
                _m31 += _m34 * offset.X; _m32 += _m34 * offset.Y; _m33 += _m34 * offset.Z;
                _offsetX += _m44 * offset.X; _offsetY += _m44 * offset.Y; _offsetZ += _m44 * offset.Z;
            }
        }

        public void TranslatePrepend(Vector3F offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
            }
            else
            {
                _offsetX += _m11 * offset.X + _m21 * offset.Y + _m31 * offset.Z;
                _offsetY += _m12 * offset.X + _m22 * offset.Y + _m32 * offset.Z;
                _offsetZ += _m13 * offset.X + _m23 * offset.Y + _m33 * offset.Z;
                _m44 += _m14 * offset.X + _m24 * offset.Y + _m34 * offset.Z;
            }
        }

        public static Matrix3F operator *(Matrix3F matrix1, Matrix3F matrix2)
        {
            // Check if multiplying by identity.
            if (matrix1.IsDistinguishedIdentity)
                return matrix2;
            if (matrix2.IsDistinguishedIdentity)
                return matrix1;

            // Regular 4x4 matrix multiplication.
            Matrix3F result = new Matrix3F(
                matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21 +
                matrix1._m13 * matrix2._m31 + matrix1._m14 * matrix2._offsetX,
                matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22 +
                matrix1._m13 * matrix2._m32 + matrix1._m14 * matrix2._offsetY,
                matrix1._m11 * matrix2._m13 + matrix1._m12 * matrix2._m23 +
                matrix1._m13 * matrix2._m33 + matrix1._m14 * matrix2._offsetZ,
                matrix1._m11 * matrix2._m14 + matrix1._m12 * matrix2._m24 +
                matrix1._m13 * matrix2._m34 + matrix1._m14 * matrix2._m44,
                matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21 +
                matrix1._m23 * matrix2._m31 + matrix1._m24 * matrix2._offsetX,
                matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22 +
                matrix1._m23 * matrix2._m32 + matrix1._m24 * matrix2._offsetY,
                matrix1._m21 * matrix2._m13 + matrix1._m22 * matrix2._m23 +
                matrix1._m23 * matrix2._m33 + matrix1._m24 * matrix2._offsetZ,
                matrix1._m21 * matrix2._m14 + matrix1._m22 * matrix2._m24 +
                matrix1._m23 * matrix2._m34 + matrix1._m24 * matrix2._m44,
                matrix1._m31 * matrix2._m11 + matrix1._m32 * matrix2._m21 +
                matrix1._m33 * matrix2._m31 + matrix1._m34 * matrix2._offsetX,
                matrix1._m31 * matrix2._m12 + matrix1._m32 * matrix2._m22 +
                matrix1._m33 * matrix2._m32 + matrix1._m34 * matrix2._offsetY,
                matrix1._m31 * matrix2._m13 + matrix1._m32 * matrix2._m23 +
                matrix1._m33 * matrix2._m33 + matrix1._m34 * matrix2._offsetZ,
                matrix1._m31 * matrix2._m14 + matrix1._m32 * matrix2._m24 +
                matrix1._m33 * matrix2._m34 + matrix1._m34 * matrix2._m44,
                matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 +
                matrix1._offsetZ * matrix2._m31 + matrix1._m44 * matrix2._offsetX,
                matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 +
                matrix1._offsetZ * matrix2._m32 + matrix1._m44 * matrix2._offsetY,
                matrix1._offsetX * matrix2._m13 + matrix1._offsetY * matrix2._m23 +
                matrix1._offsetZ * matrix2._m33 + matrix1._m44 * matrix2._offsetZ,
                matrix1._offsetX * matrix2._m14 + matrix1._offsetY * matrix2._m24 +
                matrix1._offsetZ * matrix2._m34 + matrix1._m44 * matrix2._m44);

            return result;
        }

        public static Matrix3F Multiply(Matrix3F matrix1, Matrix3F matrix2)
        {
            return (matrix1 * matrix2);
        }

        public Point3F Transform(Point3F point)
        {
            MultiplyPoint(ref point);
            return point;
        }

        public void Transform(Point3F[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]);
                }
            }
        }

        public Point4F Transform(Point4F point)
        {
            MultiplyPoint(ref point);
            return point;
        }

        public void Transform(Point4F[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]);
                }
            }
        }

        public Vector3F Transform(Vector3F vector)
        {
            MultiplyVector(ref vector);
            return vector;
        }

        public void Transform(Vector3F[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    MultiplyVector(ref vectors[i]);
                }
            }
        }

        public bool IsAffine
        {
            get
            {
                return (IsDistinguishedIdentity ||
                        (_m14 == 0.0 && _m24 == 0.0 && _m34 == 0.0 && _m44 == 1.0));
            }
        }

        public Float Determinant
        {
            get
            {
                if (IsDistinguishedIdentity)
                    return (Float)1.0;
                if (IsAffine)
                    return GetNormalizedAffineDeterminant();

                // NOTE: The beginning of this code is duplicated between
                //       the Invert method and the Determinant property.

                // compute all six 2x2 determinants of 2nd two columns
                Float y01 = _m13 * _m24 - _m23 * _m14;
                Float y02 = _m13 * _m34 - _m33 * _m14;
                Float y03 = _m13 * _m44 - _offsetZ * _m14;
                Float y12 = _m23 * _m34 - _m33 * _m24;
                Float y13 = _m23 * _m44 - _offsetZ * _m24;
                Float y23 = _m33 * _m44 - _offsetZ * _m34;

                // Compute 3x3 cofactors for 1st the column
                Float z30 = _m22 * y02 - _m32 * y01 - _m12 * y12;
                Float z20 = _m12 * y13 - _m22 * y03 + _offsetY * y01;
                Float z10 = _m32 * y03 - _offsetY * y02 - _m12 * y23;
                Float z00 = _m22 * y23 - _m32 * y13 + _offsetY * y12;

                return _offsetX * z30 + _m31 * z20 + _m21 * z10 + _m11 * z00;
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
            if (!InvertCore())
            {
                throw new InvalidOperationException("");
            }
        }

        public Float M11
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return (Float)1.0;
                }
                else
                {
                    return _m11;
                }
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m11 = value;
            }
        }

        public Float M12
        {
            get
            {
                return _m12;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m12 = value;
            }
        }

        public Float M13
        {
            get
            {
                return _m13;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m13 = value;
            }
        }

        public Float M14
        {
            get
            {
                return _m14;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m14 = value;
            }
        }


        public Float M21
        {
            get
            {
                return _m21;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m21 = value;
            }
        }

        public Float M22
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return (Float)1.0;
                }
                else
                {
                    return _m22;
                }
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m22 = value;
            }
        }

        public Float M23
        {
            get
            {
                return _m23;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m23 = value;
            }
        }

        public Float M24
        {
            get
            {
                return _m24;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m24 = value;
            }
        }

        public Float M31
        {
            get
            {
                return _m31;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m31 = value;
            }
        }

        public Float M32
        {
            get
            {
                return _m32;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m32 = value;
            }
        }

        public Float M33
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return (Float)1.0;
                }
                else
                {
                    return _m33;
                }
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m33 = value;
            }
        }

        public Float M34
        {
            get
            {
                return _m34;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m34 = value;
            }
        }

        public Float OffsetX
        {
            get
            {
                return _offsetX;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetX = value;
            }
        }

        public Float OffsetY
        {
            get
            {
                return _offsetY;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetY = value;
            }
        }

        public Float OffsetZ
        {
            get
            {
                return _offsetZ;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _offsetZ = value;
            }
        }

        public Float M44
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return (Float)1.0;
                }
                else
                {
                    return _m44;
                }
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _m44 = value;
            }
        }

        internal void SetScaleMatrix(ref Vector3F scale)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = (Float)1.0;

            IsDistinguishedIdentity = false;
        }

        internal void SetScaleMatrix(ref Vector3F scale, ref Point3F center)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = (Float)1.0;

            _offsetX = center.X - center.X * scale.X;
            _offsetY = center.Y - center.Y * scale.Y;
            _offsetZ = center.Z - center.Z * scale.Z;

            IsDistinguishedIdentity = false;
        }

        internal void SetTranslationMatrix(ref Vector3F offset)
        {
            _m11 = _m22 = _m33 = _m44 = (Float)1.0;

            _offsetX = offset.X;
            _offsetY = offset.Y;
            _offsetZ = offset.Z;

            IsDistinguishedIdentity = false;
        }

        internal static Matrix3F CreateRotationMatrix(ref QuaternionF quaternion, ref Point3F center)
        {
            Matrix3F matrix = s_identity;
            matrix.IsDistinguishedIdentity = false; // Will be using direct member access
            Float wx, wy, wz, xx, yy, yz, xy, xz, zz, x2, y2, z2;

            x2 = quaternion.X + quaternion.X;
            y2 = quaternion.Y + quaternion.Y;
            z2 = quaternion.Z + quaternion.Z;
            xx = quaternion.X * x2;
            xy = quaternion.X * y2;
            xz = quaternion.X * z2;
            yy = quaternion.Y * y2;
            yz = quaternion.Y * z2;
            zz = quaternion.Z * z2;
            wx = quaternion.W * x2;
            wy = quaternion.W * y2;
            wz = quaternion.W * z2;

            matrix._m11 = (Float)1.0 - (yy + zz);
            matrix._m12 = xy + wz;
            matrix._m13 = xz - wy;
            matrix._m21 = xy - wz;
            matrix._m22 = (Float)1.0 - (xx + zz);
            matrix._m23 = yz + wx;
            matrix._m31 = xz + wy;
            matrix._m32 = yz - wx;
            matrix._m33 = (Float)1.0 - (xx + yy);

            if (center.X != 0 || center.Y != 0 || center.Z != 0)
            {
                matrix._offsetX = -center.X * matrix._m11 - center.Y * matrix._m21 - center.Z * matrix._m31 + center.X;
                matrix._offsetY = -center.X * matrix._m12 - center.Y * matrix._m22 - center.Z * matrix._m32 + center.Y;
                matrix._offsetZ = -center.X * matrix._m13 - center.Y * matrix._m23 - center.Z * matrix._m33 + center.Z;
            }

            return matrix;
        }

        internal void MultiplyPoint(ref Point3F point)
        {
            if (IsDistinguishedIdentity)
                return;

            Float x = point.X;
            Float y = point.Y;
            Float z = point.Z;

            point.X = x * _m11 + y * _m21 + z * _m31 + _offsetX;
            point.Y = x * _m12 + y * _m22 + z * _m32 + _offsetY;
            point.Z = x * _m13 + y * _m23 + z * _m33 + _offsetZ;

            if (!IsAffine)
            {
                Float w = x * _m14 + y * _m24 + z * _m34 + _m44;

                point.X /= w;
                point.Y /= w;
                point.Z /= w;
            }
        }

        internal void MultiplyPoint(ref Point4F point)
        {
            if (IsDistinguishedIdentity)
                return;

            Float x = point.X;
            Float y = point.Y;
            Float z = point.Z;
            Float w = point.W;

            point.X = x * _m11 + y * _m21 + z * _m31 + w * _offsetX;
            point.Y = x * _m12 + y * _m22 + z * _m32 + w * _offsetY;
            point.Z = x * _m13 + y * _m23 + z * _m33 + w * _offsetZ;
            point.W = x * _m14 + y * _m24 + z * _m34 + w * _m44;
        }

        internal void MultiplyVector(ref Vector3F vector)
        {
            if (IsDistinguishedIdentity)
                return;

            Float x = vector.X;
            Float y = vector.Y;
            Float z = vector.Z;

            // Do not apply _offset to vectors.
            vector.X = x * _m11 + y * _m21 + z * _m31;
            vector.Y = x * _m12 + y * _m22 + z * _m32;
            vector.Z = x * _m13 + y * _m23 + z * _m33;
        }

        internal Float GetNormalizedAffineDeterminant()
        {
            // NOTE: The beginning of this code is duplicated between
            //       GetNormalizedAffineDeterminant() and NormalizedAffineInvert()

            Float z20 = _m12 * _m23 - _m22 * _m13;
            Float z10 = _m32 * _m13 - _m12 * _m33;
            Float z00 = _m22 * _m33 - _m32 * _m23;

            return _m31 * z20 + _m21 * z10 + _m11 * z00;
        }

        internal bool NormalizedAffineInvert()
        {
            // NOTE: The beginning of this code is duplicated between
            //       GetNormalizedAffineDeterminant() and NormalizedAffineInvert()

            Float z20 = _m12 * _m23 - _m22 * _m13;
            Float z10 = _m32 * _m13 - _m12 * _m33;
            Float z00 = _m22 * _m33 - _m32 * _m23;
            Float det = _m31 * z20 + _m21 * z10 + _m11 * z00;

            if (MathUtil.IsZero(det))
            {
                return false;
            }

            // Compute 3x3 non-zero cofactors for the 2nd column
            Float z21 = _m21 * _m13 - _m11 * _m23;
            Float z11 = _m11 * _m33 - _m31 * _m13;
            Float z01 = _m31 * _m23 - _m21 * _m33;

            // Compute all six 2x2 determinants of 1st two columns
            Float y01 = _m11 * _m22 - _m21 * _m12;
            Float y02 = _m11 * _m32 - _m31 * _m12;
            Float y03 = _m11 * _offsetY - _offsetX * _m12;
            Float y12 = _m21 * _m32 - _m31 * _m22;
            Float y13 = _m21 * _offsetY - _offsetX * _m22;
            Float y23 = _m31 * _offsetY - _offsetX * _m32;

            // Compute all non-zero and non-one 3x3 cofactors for 2nd
            // two columns
            Float z23 = _m23 * y03 - _offsetZ * y01 - _m13 * y13;
            Float z13 = _m13 * y23 - _m33 * y03 + _offsetZ * y02;
            Float z03 = _m33 * y13 - _offsetZ * y12 - _m23 * y23;
            Float z22 = y01;
            Float z12 = -y02;
            Float z02 = y12;

            Float rcp = (Float)1.0 / det;

            // Multiply all 3x3 cofactors by reciprocal & transpose
            _m11 = z00 * rcp;
            _m12 = z10 * rcp;
            _m13 = z20 * rcp;

            _m21 = z01 * rcp;
            _m22 = z11 * rcp;
            _m23 = z21 * rcp;

            _m31 = z02 * rcp;
            _m32 = z12 * rcp;
            _m33 = z22 * rcp;

            _offsetX = z03 * rcp;
            _offsetY = z13 * rcp;
            _offsetZ = z23 * rcp;

            return true;
        }

        internal bool InvertCore()
        {
            if (IsDistinguishedIdentity)
                return true;

            if (IsAffine)
            {
                return NormalizedAffineInvert();
            }

            // NOTE: The beginning of this code is duplicated between
            //       the Invert method and the Determinant property.

            // compute all six 2x2 determinants of 2nd two columns
            Float y01 = _m13 * _m24 - _m23 * _m14;
            Float y02 = _m13 * _m34 - _m33 * _m14;
            Float y03 = _m13 * _m44 - _offsetZ * _m14;
            Float y12 = _m23 * _m34 - _m33 * _m24;
            Float y13 = _m23 * _m44 - _offsetZ * _m24;
            Float y23 = _m33 * _m44 - _offsetZ * _m34;

            // Compute 3x3 cofactors for 1st the column
            Float z30 = _m22 * y02 - _m32 * y01 - _m12 * y12;
            Float z20 = _m12 * y13 - _m22 * y03 + _offsetY * y01;
            Float z10 = _m32 * y03 - _offsetY * y02 - _m12 * y23;
            Float z00 = _m22 * y23 - _m32 * y13 + _offsetY * y12;

            // Compute 4x4 determinant
            Float det = _offsetX * z30 + _m31 * z20 + _m21 * z10 + _m11 * z00;

            if (MathUtil.IsZero(det))
            {
                return false;
            }

            // Compute 3x3 cofactors for the 2nd column
            Float z31 = _m11 * y12 - _m21 * y02 + _m31 * y01;
            Float z21 = _m21 * y03 - _offsetX * y01 - _m11 * y13;
            Float z11 = _m11 * y23 - _m31 * y03 + _offsetX * y02;
            Float z01 = _m31 * y13 - _offsetX * y12 - _m21 * y23;

            // Compute all six 2x2 determinants of 1st two columns
            y01 = _m11 * _m22 - _m21 * _m12;
            y02 = _m11 * _m32 - _m31 * _m12;
            y03 = _m11 * _offsetY - _offsetX * _m12;
            y12 = _m21 * _m32 - _m31 * _m22;
            y13 = _m21 * _offsetY - _offsetX * _m22;
            y23 = _m31 * _offsetY - _offsetX * _m32;

            // Compute all 3x3 cofactors for 2nd two columns
            Float z33 = _m13 * y12 - _m23 * y02 + _m33 * y01;
            Float z23 = _m23 * y03 - _offsetZ * y01 - _m13 * y13;
            Float z13 = _m13 * y23 - _m33 * y03 + _offsetZ * y02;
            Float z03 = _m33 * y13 - _offsetZ * y12 - _m23 * y23;
            Float z32 = _m24 * y02 - _m34 * y01 - _m14 * y12;
            Float z22 = _m14 * y13 - _m24 * y03 + _m44 * y01;
            Float z12 = _m34 * y03 - _m44 * y02 - _m14 * y23;
            Float z02 = _m24 * y23 - _m34 * y13 + _m44 * y12;

            Float rcp = (Float)1.0 / det;

            // Multiply all 3x3 cofactors by reciprocal & transpose
            _m11 = z00 * rcp;
            _m12 = z10 * rcp;
            _m13 = z20 * rcp;
            _m14 = z30 * rcp;

            _m21 = z01 * rcp;
            _m22 = z11 * rcp;
            _m23 = z21 * rcp;
            _m24 = z31 * rcp;

            _m31 = z02 * rcp;
            _m32 = z12 * rcp;
            _m33 = z22 * rcp;
            _m34 = z32 * rcp;

            _offsetX = z03 * rcp;
            _offsetY = z13 * rcp;
            _offsetZ = z23 * rcp;
            _m44 = z33 * rcp;

            return true;
        }

        private static Matrix3F CreateIdentity()
        {
            // Don't call this function, use s_identity.
            Matrix3F matrix = new Matrix3F(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            matrix.IsDistinguishedIdentity = true;
            return matrix;
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotKnownToBeIdentity;
            }

            set
            {
                _isNotKnownToBeIdentity = !value;
            }
        }

        private Float _m11;
        private Float _m12;
        private Float _m13;
        private Float _m14;

        private Float _m21;
        private Float _m22;
        private Float _m23;
        private Float _m24;

        private Float _m31;
        private Float _m32;
        private Float _m33;
        private Float _m34;

        private Float _offsetX;
        private Float _offsetY;
        private Float _offsetZ;

        private Float _m44;

        // Internal matrix representation
        private bool _isNotKnownToBeIdentity;

        private static readonly Matrix3F s_identity = CreateIdentity();

        private const int c_identityHashCode = 0;

        public static bool operator ==(Matrix3F matrix1, Matrix3F matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11 == matrix2.M11 &&
                       matrix1.M12 == matrix2.M12 &&
                       matrix1.M13 == matrix2.M13 &&
                       matrix1.M14 == matrix2.M14 &&
                       matrix1.M21 == matrix2.M21 &&
                       matrix1.M22 == matrix2.M22 &&
                       matrix1.M23 == matrix2.M23 &&
                       matrix1.M24 == matrix2.M24 &&
                       matrix1.M31 == matrix2.M31 &&
                       matrix1.M32 == matrix2.M32 &&
                       matrix1.M33 == matrix2.M33 &&
                       matrix1.M34 == matrix2.M34 &&
                       matrix1.OffsetX == matrix2.OffsetX &&
                       matrix1.OffsetY == matrix2.OffsetY &&
                       matrix1.OffsetZ == matrix2.OffsetZ &&
                       matrix1.M44 == matrix2.M44;
            }
        }

        public static bool operator !=(Matrix3F matrix1, Matrix3F matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static bool Equals(Matrix3F matrix1, Matrix3F matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11.Equals(matrix2.M11) &&
                       matrix1.M12.Equals(matrix2.M12) &&
                       matrix1.M13.Equals(matrix2.M13) &&
                       matrix1.M14.Equals(matrix2.M14) &&
                       matrix1.M21.Equals(matrix2.M21) &&
                       matrix1.M22.Equals(matrix2.M22) &&
                       matrix1.M23.Equals(matrix2.M23) &&
                       matrix1.M24.Equals(matrix2.M24) &&
                       matrix1.M31.Equals(matrix2.M31) &&
                       matrix1.M32.Equals(matrix2.M32) &&
                       matrix1.M33.Equals(matrix2.M33) &&
                       matrix1.M34.Equals(matrix2.M34) &&
                       matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                       matrix1.OffsetY.Equals(matrix2.OffsetY) &&
                       matrix1.OffsetZ.Equals(matrix2.OffsetZ) &&
                       matrix1.M44.Equals(matrix2.M44);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Matrix3F))
            {
                return false;
            }

            Matrix3F value = (Matrix3F)o;
            return Matrix3F.Equals(this, value);
        }

        public bool Equals(Matrix3F value)
        {
            return Matrix3F.Equals(this, value);
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
                       M13.GetHashCode() ^
                       M14.GetHashCode() ^
                       M21.GetHashCode() ^
                       M22.GetHashCode() ^
                       M23.GetHashCode() ^
                       M24.GetHashCode() ^
                       M31.GetHashCode() ^
                       M32.GetHashCode() ^
                       M33.GetHashCode() ^
                       M34.GetHashCode() ^
                       OffsetX.GetHashCode() ^
                       OffsetY.GetHashCode() ^
                       OffsetZ.GetHashCode() ^
                       M44.GetHashCode();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Float = System.Single;

namespace YOpenGL
{
    public struct QuaternionF
    {
        public QuaternionF(Float x, Float y, Float z, Float w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
            _isNotDistinguishedIdentity = true;
        }

        public QuaternionF(Vector3F axisOfRotation, Float angleInDegrees)
        {
            angleInDegrees %= (Float)360.0; // Doing the modulo before converting to radians reduces total error
            Float angleInRadians = angleInDegrees * (Float)(Math.PI / 180.0);
            Float length = axisOfRotation.Length;
            if (length == 0)
                throw new System.InvalidOperationException("");
            Vector3F v = (axisOfRotation / length) * (Float)Math.Sin(0.5 * angleInRadians);
            _x = v.X;
            _y = v.Y;
            _z = v.Z;
            _w = (Float)Math.Cos(0.5 * angleInRadians);
            _isNotDistinguishedIdentity = true;
        }

        public static QuaternionF Identity
        {
            get
            {
                return s_identity;
            }
        }

        public Vector3F Axis
        {
            // q = M [cos(Q/2), sin(Q /2)v]
            // axis = sin(Q/2)v
            // angle = cos(Q/2)
            // M is magnitude
            get
            {
                // Handle identity (where axis is indeterminate) by
                // returning arbitrary axis.
                if (IsDistinguishedIdentity || (_x == 0 && _y == 0 && _z == 0))
                {
                    return new Vector3F(0, 1, 0);
                }
                else
                {
                    Vector3F v = new Vector3F(_x, _y, _z);
                    v.Normalize();
                    return v;
                }
            }
        }

        public Float Angle
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 0;
                }

                // Magnitude of quaternion times sine and cosine
                double msin = Math.Sqrt(_x * _x + _y * _y + _z * _z);
                double mcos = _w;

                if (!(msin <= Float.MaxValue))
                {
                    // Overflowed probably in squaring, so let's scale
                    // the values.  We don't need to include _w in the
                    // scale factor because we're not going to square
                    // it.
                    double maxcoeff = Math.Max(Math.Abs(_x), Math.Max(Math.Abs(_y), Math.Abs(_z)));
                    double x = _x / maxcoeff;
                    double y = _y / maxcoeff;
                    double z = _z / maxcoeff;
                    msin = Math.Sqrt(x * x + y * y + z * z);
                    // Scale mcos too.
                    mcos = _w / maxcoeff;
                }

                // Atan2 is better than acos.  (More precise and more efficient.)
                return (Float)(Math.Atan2(msin, mcos) * (360.0 / Math.PI));
            }
        }

        public bool IsNormalized
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }
                Float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
                return MathUtil.IsOne(norm2);
            }
        }

        public bool IsIdentity
        {
            get
            {
                return IsDistinguishedIdentity || (_x == 0 && _y == 0 && _z == 0 && _w == 1);
            }
        }

        public void Conjugate()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            // Conjugate([x,y,z,w]) = [-x,-y,-z,w]
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        public void Invert()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            // Inverse = Conjugate / Norm Squared
            Conjugate();
            Float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            _x /= norm2;
            _y /= norm2;
            _z /= norm2;
            _w /= norm2;
        }

        public void Normalize()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            Float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            if (norm2 > Float.MaxValue)
            {
                // Handle overflow in computation of norm2
                Float rmax = (Float)1.0 / Max(Math.Abs(_x),
                                      Math.Abs(_y),
                                      Math.Abs(_z),
                                      Math.Abs(_w));

                _x *= rmax;
                _y *= rmax;
                _z *= rmax;
                _w *= rmax;
                norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            }
            Float normInverse = (Float)(1.0 / Math.Sqrt(norm2));
            _x *= normInverse;
            _y *= normInverse;
            _z *= normInverse;
            _w *= normInverse;
        }

        public static QuaternionF operator +(QuaternionF left, QuaternionF right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new QuaternionF(0, 0, 0, 2);
                }
                else
                {
                    // We know left is not distinguished identity here.                    
                    left._w += 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                right._w += 1;
                return right;
            }
            else
            {
                return new QuaternionF(left._x + right._x,
                                      left._y + right._y,
                                      left._z + right._z,
                                      left._w + right._w);
            }
        }

        public static QuaternionF Add(QuaternionF left, QuaternionF right)
        {
            return (left + right);
        }

        public static QuaternionF operator -(QuaternionF left, QuaternionF right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new QuaternionF(0, 0, 0, 0);
                }
                else
                {
                    // We know left is not distinguished identity here.
                    left._w -= 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                return new QuaternionF(-right._x, -right._y, -right._z, 1 - right._w);
            }
            else
            {
                return new QuaternionF(left._x - right._x,
                                      left._y - right._y,
                                      left._z - right._z,
                                      left._w - right._w);
            }
        }

        public static QuaternionF Subtract(QuaternionF left, QuaternionF right)
        {
            return (left - right);
        }

        public static QuaternionF operator *(QuaternionF left, QuaternionF right)
        {
            if (left.IsDistinguishedIdentity)
            {
                return right;
            }
            if (right.IsDistinguishedIdentity)
            {
                return left;
            }

            Float x = left._w * right._x + left._x * right._w + left._y * right._z - left._z * right._y;
            Float y = left._w * right._y + left._y * right._w + left._z * right._x - left._x * right._z;
            Float z = left._w * right._z + left._z * right._w + left._x * right._y - left._y * right._x;
            Float w = left._w * right._w - left._x * right._x - left._y * right._y - left._z * right._z;
            QuaternionF result = new QuaternionF(x, y, z, w);
            return result;

        }

        public static QuaternionF Multiply(QuaternionF left, QuaternionF right)
        {
            return left * right;
        }

        private void Scale(Float scale)
        {
            if (IsDistinguishedIdentity)
            {
                _w = scale;
                IsDistinguishedIdentity = false;
                return;
            }
            _x *= scale;
            _y *= scale;
            _z *= scale;
            _w *= scale;
        }

        private Float Length()
        {
            if (IsDistinguishedIdentity)
            {
                return 1;
            }

            Float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            if (!(norm2 <= Float.MaxValue))
            {
                // Do this the slow way to avoid squaring large
                // numbers since the length of many quaternions is
                // representable even if the squared length isn't.  Of
                // course some lengths aren't representable because
                // the length can be up to twice as big as the largest
                // coefficient.

                Float max = Math.Max(Math.Max(Math.Abs(_x), Math.Abs(_y)),
                                      Math.Max(Math.Abs(_z), Math.Abs(_w)));

                Float x = _x / max;
                Float y = _y / max;
                Float z = _z / max;
                Float w = _w / max;

                Float smallLength = (Float)Math.Sqrt(x * x + y * y + z * z + w * w);
                // Return length of this smaller vector times the scale we applied originally.
                return smallLength * max;
            }
            return (Float)Math.Sqrt(norm2);
        }

        public static QuaternionF Slerp(QuaternionF from, QuaternionF to, Float t)
        {
            return Slerp(from, to, t, /* useShortestPath = */ true);
        }

        public static QuaternionF Slerp(QuaternionF from, QuaternionF to, Float t, bool useShortestPath)
        {
            if (from.IsDistinguishedIdentity)
            {
                from._w = 1;
            }
            if (to.IsDistinguishedIdentity)
            {
                to._w = 1;
            }

            Float cosOmega;
            Float scaleFrom, scaleTo;

            // Normalize inputs and stash their lengths
            Float lengthFrom = from.Length();
            Float lengthTo = to.Length();
            from.Scale(1 / lengthFrom);
            to.Scale(1 / lengthTo);

            // Calculate cos of omega.
            cosOmega = from._x * to._x + from._y * to._y + from._z * to._z + from._w * to._w;

            if (useShortestPath)
            {
                // If we are taking the shortest path we flip the signs to ensure that
                // cosOmega will be positive.
                if (cosOmega < 0.0)
                {
                    cosOmega = -cosOmega;
                    to._x = -to._x;
                    to._y = -to._y;
                    to._z = -to._z;
                    to._w = -to._w;
                }
            }
            else
            {
                // If we are not taking the UseShortestPath we clamp cosOmega to
                // -1 to stay in the domain of Math.Acos below.
                if (cosOmega < -1.0)
                {
                    cosOmega = -(Float)1.0;
                }
            }

            // Clamp cosOmega to [-1,1] to stay in the domain of Math.Acos below.
            // The logic above has either flipped the sign of cosOmega to ensure it
            // is positive or clamped to -1 aready.  We only need to worry about the
            // upper limit here.
            if (cosOmega > 1.0)
            {
                cosOmega = (Float)1.0;
            }

            // The mainline algorithm doesn't work for extreme
            // cosine values.  For large cosine we have a better
            // fallback hence the asymmetric limits.
            const Float maxCosine = (Float)(1.0 - 1e-6);
            const Float minCosine = (Float)(1e-10 - 1.0);

            // Calculate scaling coefficients.
            if (cosOmega > maxCosine)
            {
                // Quaternions are too close - use linear interpolation.
                scaleFrom = (Float)1.0 - t;
                scaleTo = t;
            }
            else if (cosOmega < minCosine)
            {
                // Quaternions are nearly opposite, so we will pretend to 
                // is exactly -from.
                // First assign arbitrary perpendicular to "to".
                to = new QuaternionF(-from.Y, from.X, -from.W, from.Z);

                Float theta = t * (Float)Math.PI;

                scaleFrom = (Float)Math.Cos(theta);
                scaleTo = (Float)Math.Sin(theta);
            }
            else
            {
                // Standard case - use SLERP interpolation.
                Float omega = (Float)Math.Acos(cosOmega);
                Float sinOmega = (Float)Math.Sqrt(1.0 - cosOmega * cosOmega);
                scaleFrom = (Float)Math.Sin((1.0 - t) * omega) / sinOmega;
                scaleTo = (Float)Math.Sin(t * omega) / sinOmega;
            }

            // We want the magnitude of the output quaternion to be
            // multiplicatively interpolated between the input
            // magnitudes, i.e. lengthOut = lengthFrom * (lengthTo/lengthFrom)^t
            //                            = lengthFrom ^ (1-t) * lengthTo ^ t

            Float lengthOut = lengthFrom * (Float)Math.Pow(lengthTo / lengthFrom, t);
            scaleFrom *= lengthOut;
            scaleTo *= lengthOut;

            return new QuaternionF(scaleFrom * from._x + scaleTo * to._x,
                                  scaleFrom * from._y + scaleTo * to._y,
                                  scaleFrom * from._z + scaleTo * to._z,
                                  scaleFrom * from._w + scaleTo * to._w);
        }

        static private Float Max(Float a, Float b, Float c, Float d)
        {
            if (b > a)
                a = b;
            if (c > a)
                a = c;
            if (d > a)
                a = d;
            return a;
        }

        public Float X
        {
            get
            {
                return _x;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
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
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
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
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _z = value;
            }
        }

        public Float W
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return (Float)1.0;
                }
                else
                {
                    return _w;
                }
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _w = value;
            }
        }

        internal Float _x;
        internal Float _y;
        internal Float _z;
        internal Float _w;

        private bool _isNotDistinguishedIdentity;

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotDistinguishedIdentity;
            }
            set
            {
                _isNotDistinguishedIdentity = !value;
            }
        }

        private static int GetIdentityHashCode()
        {
            // This code is called only once.
            Float zero = 0;
            Float one = 1;
            // return zero.GetHashCode() ^ zero.GetHashCode() ^ zero.GetHashCode() ^ one.GetHashCode();
            // But this expression can be simplified because the first two hash codes cancel.
            return zero.GetHashCode() ^ one.GetHashCode();
        }

        private static QuaternionF GetIdentity()
        {
            // This code is called only once.
            QuaternionF q = new QuaternionF(0, 0, 0, 1);
            q.IsDistinguishedIdentity = true;
            return q;
        }


        // Hash code for identity.
        private static int c_identityHashCode = GetIdentityHashCode();

        // Default identity
        private static QuaternionF s_identity = GetIdentity();

        public static bool operator ==(QuaternionF quaternion1, QuaternionF quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }
            else
            {
                return quaternion1.X == quaternion2.X &&
                       quaternion1.Y == quaternion2.Y &&
                       quaternion1.Z == quaternion2.Z &&
                       quaternion1.W == quaternion2.W;
            }
        }

        public static bool operator !=(QuaternionF quaternion1, QuaternionF quaternion2)
        {
            return !(quaternion1 == quaternion2);
        }

        public static bool Equals(QuaternionF quaternion1, QuaternionF quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }
            else
            {
                return quaternion1.X.Equals(quaternion2.X) &&
                       quaternion1.Y.Equals(quaternion2.Y) &&
                       quaternion1.Z.Equals(quaternion2.Z) &&
                       quaternion1.W.Equals(quaternion2.W);
            }
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is QuaternionF))
            {
                return false;
            }

            QuaternionF value = (QuaternionF)o;
            return QuaternionF.Equals(this, value);
        }

        public bool Equals(QuaternionF value)
        {
            return QuaternionF.Equals(this, value);
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
                return X.GetHashCode() ^
                       Y.GetHashCode() ^
                       Z.GetHashCode() ^
                       W.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format($"{_x}, {_y}, {_z}, {_w}");
        }
    }
}
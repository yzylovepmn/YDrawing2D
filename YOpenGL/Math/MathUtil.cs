using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public static class MathUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016;

        public static void Switch(ref float a, ref float b)
        {
            var c = a;
            a = b;
            b = c;
        }

        public static void Switch(ref PointF p1, ref PointF p2)
        {
            var p = p1;
            p1 = p2;
            p2 = p;
        }

        public static bool IsSameSymbol(float f1, float f2)
        {
            if (f1 == 0 || f2 == 0)
                return true;
            return (f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0);
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        internal static void TransformRect(ref RectF rect, ref MatrixF matrix)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            MatrixTypes matrixType = matrix._type;

            // If the matrix is identity, don't worry.
            if (matrixType == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // Scaling
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_SCALING))
            {
                rect._x *= matrix._m11;
                rect._y *= matrix._m22;
                rect._width *= matrix._m11;
                rect._height *= matrix._m22;

                // Ensure the width is always positive.  For example, if there was a reflection about the
                // y axis followed by a translation into the visual area, the width could be negative.
                if (rect._width < 0.0)
                {
                    rect._x += rect._width;
                    rect._width = -rect._width;
                }

                // Ensure the height is always positive.  For example, if there was a reflection about the
                // x axis followed by a translation into the visual area, the height could be negative.
                if (rect._height < 0.0F)
                {
                    rect._y += rect._height;
                    rect._height = -rect._height;
                }
            }

            // Translation
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_TRANSLATION))
            {
                // X
                rect._x += matrix._offsetX;

                // Y
                rect._y += matrix._offsetY;
            }

            if (matrixType == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                // Al Bunny implementation.
                PointF point0 = matrix.Transform(rect.TopLeft);
                PointF point1 = matrix.Transform(rect.TopRight);
                PointF point2 = matrix.Transform(rect.BottomRight);
                PointF point3 = matrix.Transform(rect.BottomLeft);

                // Width and height is always positive here.
                rect._x = Math.Min(Math.Min(point0.X, point1.X), Math.Min(point2.X, point3.X));
                rect._y = Math.Min(Math.Min(point0.Y, point1.Y), Math.Min(point2.Y, point3.Y));

                rect._width = Math.Max(Math.Max(point0.X, point1.X), Math.Max(point2.X, point3.X)) - rect._x;
                rect._height = Math.Max(Math.Max(point0.Y, point1.Y), Math.Max(point2.Y, point3.Y)) - rect._y;
            }
        }

        internal static void MultiplyMatrix(ref MatrixF matrix1, ref MatrixF matrix2)
        {
            MatrixTypes type1 = matrix1._type;
            MatrixTypes type2 = matrix2._type;

            // Check for idents

            // If the second is ident, we can just return
            if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // If the first is ident, we can just copy the memory across.
            if (type1 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix1 = matrix2;
                return;
            }

            // Optimize for translate case, where the second is a translate
            if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // 2 additions
                matrix1._offsetX += matrix2._offsetX;
                matrix1._offsetY += matrix2._offsetY;

                // If matrix 1 wasn't unknown we added a translation
                if (type1 != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }

                return;
            }

            // Check for the first value being a translate
            if (type1 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // Save off the old offsets
                float offsetX = matrix1._offsetX;
                float offsetY = matrix1._offsetY;

                // Copy the matrix
                matrix1 = matrix2;

                matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;

                if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
                else
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
                return;
            }

            // The following code combines the type of the transformations so that the high nibble
            // is "this"'s type, and the low nibble is mat's type.  This allows for a switch rather
            // than nested switches.

            // trans1._type |  trans2._type
            //  7  6  5  4   |  3  2  1  0
            int combinedType = ((int)type1 << 4) | (int)type2;

            switch (combinedType)
            {
                case 34:  // S * S
                    // 2 multiplications
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    return;

                case 35:  // S * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._offsetX;
                    matrix1._offsetY = matrix2._offsetY;

                    // Transform set to Translate and Scale
                    matrix1._type = MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING;
                    return;

                case 50: // S|T * S
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX *= matrix2._m11;
                    matrix1._offsetY *= matrix2._m22;
                    return;

                case 51: // S|T * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                    matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                    return;
                case 36: // S * U
                case 52: // S|T * U
                case 66: // U * S
                case 67: // U * S|T
                case 68: // U * U
                    matrix1 = new MatrixF(
                        matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                        matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,

                        matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                        matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,

                        matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                        matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                    return;
            }
        }
    }
}
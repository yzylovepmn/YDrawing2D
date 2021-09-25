using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGeometry.Maths
{
    public static class MathUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016;

        internal static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        internal static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        internal static void Clamp(ref double value, double min, double max)
        {
            value = Math.Max(Math.Min(value, max), min);
        }

        internal static void Clamp(ref float value, float min, float max)
        {
            value = Math.Max(Math.Min(value, max), min);
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
        }

        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        // Assume v is perpendicular to normal and normal is unit vector(right hand)
        public static Vector3D Rotate(Vector3D v, Vector3D normal, double radian)
        {
            var z = normal;
            var x = v;
            var y = Vector3D.CrossProduct(z, x);
            var s = Math.Sin(radian);
            var c = Math.Cos(radian);
            return c * x + s * y;
        }
    }
}
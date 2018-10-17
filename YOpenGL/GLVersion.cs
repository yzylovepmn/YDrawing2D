using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public struct GLVersion : IComparable<GLVersion>
    {
        public GLVersion(int major, int minor)
        {
            _major = major;
            _minor = minor;
        }

        public int Major { get { return _major; } }
        internal int _major;

        public int Minor { get { return _minor; } }
        internal int _minor;

        public static GLVersion FromNumber(int major, int minor)
        {
            return new GLVersion(major, minor);
        }

        public int CompareTo(int major, int minor)
        {
            if (_major != major)
                return _major - major;
            return _minor - minor;
        }

        public int CompareTo(GLVersion other)
        {
            return CompareTo(other._major, other._minor);
        }

        public static bool operator ==(GLVersion version1, GLVersion version2)
        {
            return version1.CompareTo(version2) == 0;
        }

        public static bool operator >=(GLVersion version1, GLVersion version2)
        {
            return version1.CompareTo(version2) >= 0;
        }

        public static bool operator >(GLVersion version1, GLVersion version2)
        {
            return version1.CompareTo(version2) > 0;
        }

        public static bool operator <=(GLVersion version1, GLVersion version2)
        {
            return version1.CompareTo(version2) <= 0;
        }

        public static bool operator <(GLVersion version1, GLVersion version2)
        {
            return version1.CompareTo(version2) < 0;
        }

        public static bool operator !=(GLVersion version1, GLVersion version2)
        {
            return !(version1 == version2);
        }

        public override bool Equals(object obj)
        {
            if (obj is GLVersion)
                return this == (GLVersion)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return _major.GetHashCode() ^ _minor.GetHashCode();
        }
    }
}
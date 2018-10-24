using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal struct ContextHandle : IComparable<ContextHandle>, IEquatable<ContextHandle>
    {
        public static readonly ContextHandle Zero = new ContextHandle(IntPtr.Zero);

        public IntPtr Handle { get { return handle; } }
        private IntPtr handle;

        public ContextHandle(IntPtr h) { handle = h; }

        public override string ToString()
        {
            return Handle.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is ContextHandle)
            {
                return this.Equals((ContextHandle)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public static explicit operator IntPtr(ContextHandle c)
        {
            return c != ContextHandle.Zero ? c.handle : IntPtr.Zero;
        }

        public static explicit operator ContextHandle(IntPtr p)
        {
            return new ContextHandle(p);
        }

        public static bool operator ==(ContextHandle left, ContextHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContextHandle left, ContextHandle right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ContextHandle other)
        {
            unsafe { return (int)((int*)other.handle.ToPointer() - (int*)this.handle.ToPointer()); }
        }

        public bool Equals(ContextHandle other)
        {
            return Handle == other.Handle;
        }
    }
}
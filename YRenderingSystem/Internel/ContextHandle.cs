using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem
{
    public struct ContextHandle : IEquatable<ContextHandle>
    {
        public static readonly ContextHandle Zero = new ContextHandle(IntPtr.Zero, IntPtr.Zero);

        public IntPtr HDC { get { return _hdc; } }
        private IntPtr _hdc;

        public IntPtr Handle { get { return _handle; } }
        private IntPtr _handle;

        public ContextHandle(IntPtr hdc, IntPtr handle) { _hdc = hdc; _handle = handle; }

        public override string ToString()
        {
            return string.Format("HDC:{0} Handle:{1}", _hdc, _handle);
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
            return _hdc.GetHashCode() ^ _handle.GetHashCode();
        }

        public static bool operator ==(ContextHandle left, ContextHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContextHandle left, ContextHandle right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ContextHandle other)
        {
            return _hdc == other._hdc && _handle == other._handle;
        }
    }
}
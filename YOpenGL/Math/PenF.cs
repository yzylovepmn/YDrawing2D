using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class PenF
    {
        public PenF(float thickness, Color color, byte[] dashes = null)
        {
            _thickness = thickness;
            _color = color;
            if (dashes != null)
                _data = _GenData(dashes);
        }

        /// <summary>
        /// Thickness of the pen
        /// </summary>
        public float Thickness { get { return _thickness; } }
        private float _thickness;

        /// <summary>
        /// Color of the pen
        /// </summary>
        public Color Color { get { return _color; } }
        private Color _color;

        public byte[] Data { get { return _data; } }
        private byte[] _data;

        private byte[] _GenData(byte[] dashes)
        {
            var data = new List<byte>();

            var flag = true;
            foreach (var value in dashes)
            {
                for (int i = value; i > 0; i--)
                    data.Add(flag ? (byte)255 : (byte)0);
                flag = !flag;
            }

            return data.ToArray();
        }

        public static bool operator ==(PenF pen1, PenF pen2)
        {
            if (ReferenceEquals(pen1, pen2)) return true;
            if (pen1 is null || pen2 is null) return false;
            if (pen1._thickness == pen2._thickness && pen1._color == pen2._color)
            {
                if (pen1._data == null && pen2._data == null)
                    return true;
                if (pen1._data != null && pen2._data != null && pen1._data.SequenceEqual(pen2._data))
                    return true;
            }
            return false;
        }

        public static bool operator !=(PenF pen1, PenF pen2)
        {
            return !(pen1 == pen2);
        }

        public override bool Equals(object obj)
        {
            if (obj is PenF)
                return this == (PenF)obj;
            return false;
        }

        public override int GetHashCode()
        {
            var code = _thickness.GetHashCode() ^ _color.GetHashCode();
            if (_data != null)
                foreach (var dash in _data)
                    code ^= dash.GetHashCode();
            return code;
        }
    }
}
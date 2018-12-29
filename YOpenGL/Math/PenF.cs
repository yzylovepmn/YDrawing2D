using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public struct PenF : IComparable<PenF>
    {
        public static readonly PenF NULL;

        static PenF()
        {
            NULL = new PenF(-1, Colors.Transparent, null);
        }

        public PenF(float thickness, Color color, byte[] dashes = null)
        {
            _thickness = thickness;
            _color = color;
            _data = null;
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
        public Color Color { get { return _color; } set { _color = value; } }
        private Color _color;

        public bool IsNULL { get { return _thickness < 0; } }

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
            if (pen1._thickness == pen2._thickness && pen1.Color == pen2.Color)
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

        public int CompareTo(PenF other)
        {
            if (_thickness > other._thickness)
                return -1;
            if (_thickness < other._thickness)
                return 1;
            var v1 = _color.GetValue();
            var v2 = other._color.GetValue();
            if (v1 > v2)
                return -1;
            if (v1 < v2)
                return 1;
            if (_data == null && other._data == null)
                return 0;
            if (_data != null && other._data != null)
            {
                if (_data.Length != other._data.Length)
                    return _data.Length - other._data.Length;
                else
                {
                    for (int i = 0; i < _data.Length; i++)
                        if (_data[i] != other._data[i])
                            return _data[i] - other._data[i];
                    return 0;
                }
            }
            else
            {
                if (_data != null)
                    return 1;
                else return -1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PenF)
                return this == (PenF)obj;
            return false;
        }

        public override int GetHashCode()
        {
            var code = _thickness.GetHashCode() ^ Color.GetHashCode();
            if (_data != null)
                foreach (var dash in _data)
                    code ^= dash.GetHashCode();
            return code;
        }
    }
}
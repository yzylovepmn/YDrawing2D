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
        public PenF(float thickness, Color color, float[] dashes = null)
        {
            _thickness = thickness;
            _color = color;
            _dashes = dashes;
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

        public float[] Dashes { get { return _dashes; } }
        private float[] _dashes;

        /// <summary>
        /// Not compare dash
        /// </summary>
        public static bool operator ==(PenF pen1, PenF pen2)
        {
            if (ReferenceEquals(pen1, pen2)) return true;
            return pen1._thickness == pen2._thickness && pen1._color == pen2._color;
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
            if (_dashes != null)
                foreach (var dash in _dashes)
                    code ^= dash.GetHashCode();
            return code;
        }
    }
}
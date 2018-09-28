using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YDrawing2D.Util
{
    public struct DrawingPen
    {
        public static readonly DrawingPen EMPTY = new DrawingPen(-1, Colors.Transparent, null);

        public DrawingPen(double thickness, Color color, double[] dashes = null)
        {
            _thickness = thickness;
            _color = color;
            _dashes = dashes;
        }

        /// <summary>
        /// Thickness of the pen
        /// </summary>
        public double Thickness { get { return _thickness; } }
        private double _thickness;

        /// <summary>
        /// Color of the pen
        /// </summary>
        public Color Color { get { return _color; } }
        private Color _color;

        public double[] Dashes { get { return _dashes; } }
        private double[] _dashes;
    }

    public struct _DrawingPen
    {
        internal static readonly _DrawingPen EMPTY = new _DrawingPen(-1, null, null);

        internal _DrawingPen(Int32 thickness, byte[] color, Int32[] dashes = null)
        {
            _thickness = thickness;
            _color = color;
            _dashes = dashes;
        }

        /// <summary>
        /// Thickness of the pen
        /// </summary>
        public Int32 Thickness { get { return _thickness; } }
        private Int32 _thickness;

        /// <summary>
        /// Color of the pen
        /// </summary>
        public byte[] Color { get { return _color; } }
        private byte[] _color;

        public Int32[] Dashes { get { return _dashes; } }
        private Int32[] _dashes;
    }
}
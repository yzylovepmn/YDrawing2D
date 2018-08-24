using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace YDrawing2D.Model
{
    public struct PrimitiveProperty
    {
        public PrimitiveProperty(Int32Rect bounds, int thickness, int color)
        {
            _bounds = bounds;
            _thickness = thickness;
            _color = color;
        }

        /// <summary>
        /// The boundary of the primitive
        /// </summary>
        public Int32Rect Bounds { get { return _bounds; } }
        private Int32Rect _bounds;

        /// <summary>
        /// Brush thickness of the primitive
        /// </summary>
        public int Thickness { get { return _thickness; } }
        private int _thickness;

        /// <summary>
        /// Brush color of the primitive
        /// </summary>
        public int Color { get { return _color; } }
        private int _color;
    }
}
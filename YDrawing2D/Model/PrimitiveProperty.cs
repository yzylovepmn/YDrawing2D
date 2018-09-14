using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct PrimitiveProperty
    {
        internal PrimitiveProperty(_DrawingPen pen, Int32Rect bounds)
        {
            _pen = pen;
            _bounds = bounds;
        }

        /// <summary>
        /// The boundary of the primitive
        /// </summary>
        public Int32Rect Bounds { get { return _bounds; } internal set { _bounds = value; } }
        private Int32Rect _bounds;

        /// <summary>
        /// Pen of the primitive
        /// </summary>
        public _DrawingPen Pen { get { return _pen; } }
        private _DrawingPen _pen;
    }
}
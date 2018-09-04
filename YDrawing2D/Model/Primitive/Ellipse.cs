using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Ellipse : IPrimitive
    {
        public Ellipse(Int32Point center, Int32 radiusX, Int32 radiusY, _DrawingPen pen)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            RadiusXSquared = RadiusX * RadiusX;
            RadiusYSquared = RadiusY * RadiusY;
            SplitX = (Int32)(RadiusXSquared / Math.Sqrt(RadiusXSquared + RadiusYSquared));
            var _bounds = GeometryHelper.CalcBounds(center, radiusX, RadiusY, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Ellipse; } }

        internal Int32Point Center;
        internal Int32 RadiusX;
        internal Int32 RadiusY;
        internal Int32 RadiusXSquared;
        internal Int32 RadiusYSquared;
        internal Int32 SplitX;

        public bool HitTest(Int32Point p)
        {
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            return true;
        }
    }
}
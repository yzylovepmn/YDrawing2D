using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct CustomGeometry : IPrimitive
    {
        public static readonly CustomGeometry Empty;

        static CustomGeometry()
        {
            Empty = new CustomGeometry();
        }

        public CustomGeometry(_DrawingPen pen, bool isClosed)
        {
            _property = new PrimitiveProperty(pen, Int32Rect.Empty);
            _isClosed = isClosed;
            _stream = new List<IPrimitive>();
            UnClosedLine = null;
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Geometry; } }

        internal Line? UnClosedLine;

        internal bool IsClosed { get { return _isClosed; } }
        private bool _isClosed;

        internal IEnumerable<IPrimitive> Stream { get { return _stream; } }
        private List<IPrimitive> _stream;

        internal void StreamTo(IPrimitive primitive)
        {
            _stream.Add(primitive);
            _property.Bounds = GeometryHelper.ExtendBounds(_property.Bounds, primitive.Property.Bounds);
        }

        public bool HitTest(Int32Point p)
        {
            foreach (var primitive in _stream)
                if (primitive.HitTest(p))
                    return true;
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            foreach (var primitive in _stream)
                if (primitive.IsIntersect(other))
                    return true;
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct CustomGeometry : IPrimitive, ICanFilledPrimitive
    {
        public static readonly CustomGeometry Empty;

        static CustomGeometry()
        {
            Empty = new CustomGeometry();
        }

        public CustomGeometry(byte[] fillColor, _DrawingPen pen, bool isClosed)
        {
            _fillColor = fillColor;
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

        public byte[] FillColor { get { return _fillColor; } }
        private byte[] _fillColor;

        internal void StreamTo(IPrimitive primitive)
        {
            _stream.Add(primitive);
            if (_property.Bounds == Int32Rect.Empty)
                _property.Bounds = primitive.Property.Bounds;
            else _property.Bounds = GeometryHelper.ExtendBounds(_property.Bounds, primitive.Property.Bounds);
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
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;

            foreach (var primitive in _stream)
                if (primitive.IsIntersect(other))
                    return true;

            return false;
        }

        public IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths)
        {
            var region = new List<Int32Point>();
            if (_fillColor != null)
            {

            }
            return region;
        }
    }
}
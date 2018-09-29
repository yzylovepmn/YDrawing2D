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
    public enum Shape
    {
        Custom,
        Rect
    }

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
            _shape = Shape.Custom;
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Geometry; } }

        internal Shape Shape { get { return _shape; } set { _shape = value; } }
        private Shape _shape;

        internal Line? UnClosedLine;

        internal bool IsClosed { get { return _isClosed; } set { _isClosed = value; } }
        private bool _isClosed;

        internal IEnumerable<IPrimitive> Stream { get { return _stream; } }
        private List<IPrimitive> _stream;

        public byte[] FillColor { get { return _fillColor; } }
        private byte[] _fillColor;

        internal void SetPen(_DrawingPen pen)
        {
            _property.Pen = pen;
        }

        internal void SetFillColor(byte[] fillColor)
        {
            _fillColor = fillColor;
        }

        internal void StreamTo(IPrimitive primitive)
        {
            _stream.Add(primitive);
            if (_property.Bounds == Int32Rect.Empty)
                _property.Bounds = primitive.Property.Bounds;
            else _property.Bounds = GeometryHelper.ExtendBounds(_property.Bounds, primitive.Property.Bounds);
        }

        public bool HitTest(Int32Point p)
        {
            if (_fillColor == null && _property.Pen.Thickness > 0)
            {
                foreach (var primitive in _stream)
                    if (primitive.Property.Bounds.Contains(p) && primitive.HitTest(p))
                        return true;
            }
            else
            {
                switch (_shape)
                {
                    case Shape.Rect:
                        return _property.Bounds.Contains(p);
                }

                return GeometryHelper.Contains(GeometryHelper._GetCustomGeometryPrimitives(this), p);
            }
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;

            if (_fillColor == null && _property.Pen.Thickness > 0)
            {
                foreach (var primitive in _stream)
                    if (primitive.IsIntersect(other))
                        return true;
            }
            else return true;

            return false;
        }

        public IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths, Int32Rect bounds)
        {
            var region = new List<Int32Point>();
            if (_fillColor != null)
            {
                var curx = bounds.X;
                var cury = bounds.Y;
                var right = curx + bounds.Width;
                var bottom = cury + bounds.Height;
                switch (_shape)
                {
                    case Shape.Rect:
                        for (int i = curx + 1; i < right; i++)
                            for (int j = cury + 1; j < bottom; j++)
                                region.Add(new Int32Point(i, j));
                        break;
                    default:
                        var primitives = GeometryHelper._GetCustomGeometryPrimitives(this);
                        for (int i = curx + 1; i < right; i++)
                        {
                            for (int j = cury + 1; j < bottom; j++)
                            {
                                var p = new Int32Point(i, j);
                                if (GeometryHelper.Contains(primitives, p))
                                    region.Add(p);
                            }
                        }
                        break;
                }
            }
            return region;
        }
    }
}
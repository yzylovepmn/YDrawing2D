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
            if (_fillColor == null)
            {
                foreach (var primitive in _stream)
                    if (primitive.HitTest(p))
                        return true;
            }
            else return GeometryHelper.Contains(this, p);
            return false;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;

            if (_fillColor == null)
            {
                foreach (var primitive in _stream)
                    if (primitive.IsIntersect(other))
                        return true;
            }
            else return true;

            return false;
        }

        public IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths)
        {
            var region = new List<Int32Point>();
            var delta = _property.Pen.Thickness / 2;
            if (_fillColor != null)
            {
                var flag = false;
                Int32Point startp = default(Int32Point), endp = default(Int32Point);
                var right = _property.Bounds.Width + _property.Bounds.X;
                for (int i = _property.Bounds.X; i <= right; i++)
                {
                    flag = false;
                    foreach (var point in GeometryHelper.GetVerticalPoints(paths, i))
                    {
                        if (!flag)
                            startp = point;
                        else
                        {
                            if ((point.Y - startp.Y) > 1)
                            {
                                endp = point;
                                region.AddRange(GeometryHelper.GenScanPoints(startp, endp, delta));
                            }
                            else flag = !flag;
                        }
                        flag = !flag;
                    }
                }
            }
            return region;
        }
    }
}
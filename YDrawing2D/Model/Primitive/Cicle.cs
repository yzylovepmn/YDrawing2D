using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YDrawing2D.Extensions;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public struct Cicle : IPrimitive, ICanFilledPrimitive
    {
        internal Cicle(byte[] fillColor, Int32Point center, Int32 radius, _DrawingPen pen)
        {
            _fillColor = fillColor;
            Center = center;
            Radius = radius;
            var _bounds = GeometryHelper.CalcBounds(center, radius, pen.Thickness);
            _property = new PrimitiveProperty(pen, _bounds);
        }

        public PrimitiveProperty Property { get { return _property; } }
        private PrimitiveProperty _property;

        public PrimitiveType Type { get { return PrimitiveType.Cicle; } }

        internal readonly Int32Point Center;
        internal readonly Int32 Radius;

        public byte[] FillColor { get { return _fillColor; } }
        private byte[] _fillColor;

        public bool HitTest(Int32Point p)
        {
            var len = (p - Center).Length;
            if (_fillColor == null)
                return Math.Abs(len - Radius) <= _property.Pen.Thickness;
            else return len < Radius + _property.Pen.Thickness;
        }

        public bool IsIntersect(IPrimitive other)
        {
            if (!other.Property.Bounds.IsIntersectWith(_property.Bounds)) return false;
            switch (other.Type)
            {
                case PrimitiveType.Line:
                    return GeometryHelper.IsIntersect((Line)other, this);
                case PrimitiveType.Cicle:
                    return GeometryHelper.IsIntersect(this, (Cicle)other);
                case PrimitiveType.Arc:
                    return GeometryHelper.IsIntersect(this, (Arc)other);
                case PrimitiveType.Ellipse:
                    return GeometryHelper.IsIntersect((Ellipse)other, this);
            }
            return true;
        }

        public IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths)
        {
            var region = new List<Int32Point>();
            var delta = _property.Pen.Thickness / 2;
            if (_fillColor != null)
            {
                var flag = false;
                var isfirst = true;
                var skip = false;
                int y = 0, cnt = 0;
                Int32Point startp = default(Int32Point), endp = default(Int32Point);
                foreach (var path in paths)
                {
                    var res = path.Path.Count() % 8;
                    foreach (var point in path.Path.Take(res))
                    {
                        if (!flag)
                        {
                            isfirst = false;
                            startp = point;
                            y = startp.Y;
                        }
                        else
                        {
                            endp = point;
                            region.AddRange(GeometryHelper.GenScanPoints(startp, endp, delta));
                        }
                        flag = !flag;
                        if (!flag) break;
                    }
                    foreach (var point in path.Path.Skip(res))
                    {
                        if (!flag)
                        {
                            startp = point;
                            if (cnt == 0)
                            {
                                skip = false;
                                if (isfirst)
                                {
                                    y = startp.Y;
                                    isfirst = false;
                                }
                                else
                                {
                                    if (y == startp.Y)
                                        skip = true;
                                    y = startp.Y;
                                }
                            }
                        }
                        else
                        {
                            endp = point;
                            if ((!skip || cnt < 4))
                                region.AddRange(GeometryHelper.GenScanPoints(startp, endp, delta));
                        }
                        flag = !flag;
                        if (++cnt == 8)
                            cnt = 0;
                    }
                }
            }
            return region;
        }
    }
}
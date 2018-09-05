using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Model;
using YDrawing2D.Util;
using YDrawing2D.View;

namespace YDrawing2D.Extensions
{
    public static class Extensions
    {
        public static bool Contains(this Int32Rect rect, Int32Point p)
        {
            return !(p.X < rect.X || p.Y < rect.Y || p.X >= (rect.X + rect.Width) || p.Y >= (rect.Y + rect.Height));
        }

        public static bool IsIntersectWith(this Int32Rect rect, Int32Rect other)
        {
            var right1 = rect.X + rect.Width;
            var bottom1 = rect.Y + rect.Height;
            if (right1 < other.X || bottom1 < other.Y)
                return false;

            var right2 = other.X + other.Width;
            var bottom2 = other.Y + other.Height;
            if (right2 < rect.X || bottom2 < rect.Y)
                return false;

            return true;
        }

        public static bool Contains(this Int32Rect rect, Int32Rect other)
        {
            var right1 = rect.X + rect.Width;
            var bottom1 = rect.Y + rect.Height;

            var right2 = other.X + other.Width;
            var bottom2 = other.Y + other.Height;

            return rect._Contains(right1, bottom1, other, right2, bottom2);
        }

        private static bool _Contains(this Int32Rect rect, Int32 right1, Int32 bottom1, Int32Rect other, Int32 right2, Int32 bottom2)
        {
            return rect.X < other.X && rect.Y < other.Y && right1 > right2 && bottom1 > bottom2;
        }

        public static bool IsIntersectWith(this Int32Rect rect, IPrimitive primitive)
        {
            var other = primitive.Property.Bounds;
            var right1 = rect.X + rect.Width;
            var bottom1 = rect.Y + rect.Height;
            if (right1 < other.X || bottom1 < other.Y)
                return false;

            var right2 = other.X + other.Width;
            var bottom2 = other.Y + other.Height;
            if (right2 < rect.X || bottom2 < rect.Y)
                return false;

            if (other._Contains(right2, bottom2, rect, right1, bottom1))
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Cicle:
                    case PrimitiveType.Arc:
                        Int32Point center;
                        Int32 radius;
                        if (primitive.Type == PrimitiveType.Cicle)
                        {
                            var cicle = (Cicle)primitive;
                            radius = cicle.Radius;
                            center = cicle.Center;
                        }
                        else
                        {
                            var arc = (Arc)primitive;
                            radius = arc.Radius;
                            center = arc.Center;
                        }
                        Int64 radiusSquared = radius * radius;
                        var _v1 = (Int32)Math.Sqrt(radiusSquared - (rect.Y - center.Y) * (rect.Y - center.Y));
                        var _v2 = (Int32)Math.Sqrt(radiusSquared - (bottom1 - center.Y) * (bottom1 - center.Y));
                        var _x1 = center.X - Math.Min(_v1, _v2);
                        var _x2 = center.X + Math.Min(_v1, _v2);
                        return rect.X <= _x1 || rect.X >= _x2 || right1 >= _x2;
                    case PrimitiveType.Ellipse:
                        var ellipse = (Ellipse)primitive;
                        _v1 = (Int32)(ellipse.RadiusX * (Math.Sqrt(ellipse.RadiusYSquared - (rect.Y - ellipse.Center.Y) * (rect.Y - ellipse.Center.Y)) / ellipse.RadiusY));
                        _v2 = (Int32)(ellipse.RadiusX * (Math.Sqrt(ellipse.RadiusYSquared - (bottom1 - ellipse.Center.Y) * (bottom1 - ellipse.Center.Y)) / ellipse.RadiusY));
                        _x1 = ellipse.Center.X - Math.Min(_v1, _v2);
                        _x2 = ellipse.Center.X + Math.Min(_v1, _v2);
                        return rect.X <= _x1 || rect.X >= _x2 || right1 >= _x2;
                }
                return true;
            }

            return true;
        }

        public static bool IsIntersectWith(this IPrimitive self, IPrimitive other)
        {
            return self.Property.Bounds.IsIntersectWith(other.Property.Bounds);
        }

        public static bool Contains(this IPrimitive primitive, Int32Rect rect)
        {
            var other = primitive.Property.Bounds;
            var right1 = other.X + other.Width;
            var bottom1 = other.Y + other.Height;
            var right2 = rect.X + rect.Width;
            var bottom2 = rect.Y + rect.Height;

            if (other._Contains(right1, bottom1, rect, right2, bottom2))
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Cicle:
                    case PrimitiveType.Arc:
                        Int32Point center;
                        Int32 radius;
                        if (primitive.Type == PrimitiveType.Cicle)
                        {
                            var cicle = (Cicle)primitive;
                            radius = cicle.Radius;
                            center = cicle.Center;
                        }
                        else
                        {
                            var arc = (Arc)primitive;
                            radius = arc.Radius;
                            center = arc.Center;
                        }
                        Int64 radiusSquared = radius * radius;
                        var _v1 = (Int32)Math.Sqrt(radiusSquared - (rect.Y - center.Y) * (rect.Y - center.Y));
                        var _v2 = (Int32)Math.Sqrt(radiusSquared - (bottom2 - center.Y) * (bottom2 - center.Y));
                        var _x1 = center.X - Math.Min(_v1, _v2);
                        var _x2 = center.X + Math.Min(_v1, _v2);
                        return !(rect.X <= _x1 || rect.X >= _x2 || right2 >= _x2);
                    case PrimitiveType.Ellipse:
                        var ellipse = (Ellipse)primitive;
                        _v1 = (Int32)(ellipse.RadiusX * (Math.Sqrt(ellipse.RadiusYSquared - (rect.Y - ellipse.Center.Y) * (rect.Y - ellipse.Center.Y)) / ellipse.RadiusY));
                        _v2 = (Int32)(ellipse.RadiusX * (Math.Sqrt(ellipse.RadiusYSquared - (bottom2 - ellipse.Center.Y) * (bottom2 - ellipse.Center.Y)) / ellipse.RadiusY));
                        _x1 = ellipse.Center.X - Math.Min(_v1, _v2);
                        _x2 = ellipse.Center.X + Math.Min(_v1, _v2);
                        return !(rect.X <= _x1 || rect.X >= _x2 || right2 >= _x2);
                }
                return true;
            }

            return false;
        }

        public static bool IsIntersectWith(this PresentationVisual self, PresentationVisual other)
        {
            if (self == other) return true;
            foreach (var primitive1 in self.Context.Primitives)
                foreach (var primitive2 in other.Context.Primitives)
                    if (primitive1 != null && primitive2 != null && primitive1.IsIntersect(primitive2))
                        return true;
            return false;
        }
    }
}
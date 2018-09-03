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

            if (right2 > right1 && bottom2 > bottom1 && other.X < rect.X && other.Y < rect.Y)
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
                        var p = new Int32Point(rect.X + rect.Width, rect.Y + rect.Height);
                        var v = radius - primitive.Property.Pen.Thickness;
                        return !(((p - center).Length < v) && ((new Int32Point(rect.X, rect.Y) - center).Length < v)
                            && ((new Int32Point(rect.X + rect.Width, rect.Y) - center).Length < v) && ((new Int32Point(rect.X, rect.Y + rect.Height) - center).Length < v));
                }
                return true;
            }

            return true;
        }

        public static bool IsIntersectWith(this IPrimitive self, IPrimitive other)
        {
            return self.Property.Bounds.IsIntersectWith(other.Property.Bounds);
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
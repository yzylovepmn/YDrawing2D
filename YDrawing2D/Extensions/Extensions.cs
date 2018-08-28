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
            var right = rect.X + rect.Width;
            var bottom = rect.Y + rect.Height;
            if (right < other.X || bottom < other.Y)
                return false;

            right = other.X + other.Width;
            bottom = other.Y + other.Height;
            if (right < rect.X || bottom < rect.Y)
                return false;

            return true;
        }

        public static bool IsIntersectWith(this IPrimitive self, IPrimitive other)
        {
            return self.Property.Bounds.IsIntersectWith(other.Property.Bounds);
        }

        public static bool IsIntersectWith(this PresentationVisual self, PresentationVisual other)
        {
            foreach (var primitive1 in self.Context.Primitives)
                foreach (var primitive2 in other.Context.Primitives)
                    if (primitive1.IsIntersect(primitive2))
                        return true;
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Util;

namespace YDrawing2D.Extensions
{
    public static class Extensions
    {
        public static bool Contains(this Int32Rect rect, Int32Point p)
        {
            return !(p.X < rect.X || p.Y < rect.Y || p.X >= (rect.X + rect.Width) || p.Y >= (rect.Y + rect.Height));
        }
    }
}
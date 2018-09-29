using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Util;

namespace YDrawing2D.Model
{
    public interface ICanFilledPrimitive
    {
        byte[] FillColor { get; }

        IEnumerable<Int32Point> GenFilledRegion(IEnumerable<PrimitivePath> paths, Int32Rect bounds);
    }
}
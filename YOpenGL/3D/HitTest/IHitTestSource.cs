using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public interface IHitTestSource
    {
        IHitTestSource Parent { get; }

        Rect3F Bounds { get; }

        GLPrimitiveMode Mode { get; set; }

        float PointSize { get; set; }

        float LineWidth { get; set; }

        IEnumerable<DataPair> Pairs { get; }

        IEnumerable<Point3F> GetHitTestPoints();

        int GetIndex(int index);
    }
}
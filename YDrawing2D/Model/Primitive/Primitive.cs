using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDrawing2D.Model
{
    public interface IPrimitive
    {
        PrimitiveProperty Property { get; }
        PrimitiveType Type { get; }
    }

    public enum PrimitiveType
    {
        Line,
        Cicle,
        Arc
    }
}
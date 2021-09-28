using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YRenderingSystem._3D
{
    public enum GLPrimitiveMode : uint
    {
        GL_POINTS = 0x0000,
        GL_LINES = 0x0001,
        GL_LINE_LOOP = 0x0002,
        GL_LINE_STRIP = 0x0003,
        GL_TRIANGLES = 0x0004,
        GL_TRIANGLE_STRIP = 0x0005,
        GL_TRIANGLE_FAN = 0x0006
    }
}
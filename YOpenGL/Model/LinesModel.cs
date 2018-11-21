using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;

namespace YOpenGL
{
    public class LinesModel : MeshModel
    {
        internal LinesModel() { }

        internal override void Draw(Shader shader)
        {
            BindVertexArray(_vao[0]);
            DrawArrays(GL_LINES, 0, _pointCount);
        }
    }
}
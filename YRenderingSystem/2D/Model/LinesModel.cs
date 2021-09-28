using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YRenderingSystem.GLFunc;
using static YRenderingSystem.GLConst;

namespace YRenderingSystem
{
    public class LinesModel : MeshModel
    {
        internal LinesModel() { }

        internal override void Draw(Shader shader)
        {
            if (!_hasInit) return;
            BindVertexArray(_vao[0]);
            DrawArrays(GL_LINES, 0, _pointCount);
        }
    }
}
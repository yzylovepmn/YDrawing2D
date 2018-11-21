using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public class LinesModel : MeshModel
    {
        internal LinesModel() { }

        internal override void Draw(Shader shader)
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_LINES, 0, _pointCount);
        }
    }
}
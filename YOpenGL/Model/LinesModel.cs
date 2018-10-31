using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class LinesModel : MeshModel
    {
        internal override void Draw(Shader shader)
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_LINES, 0, _points.Count);
        }
    }
}
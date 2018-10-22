using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class LinesModel : MeshModel
    {
        internal override void Draw(Shader shader, params object[] param)
        {
            var pen = param[0] as PenF;
            shader.SetVec4("color", pen.Color.GetData());
            //Set line width
            GLFunc.LineWidth(pen.Thickness / (float)param[1]);
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_LINES, 0, _points.Count);
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive)
        {
            if (primitive.Points.Count < Capacity && _points.Count + primitive.Points.Count > Capacity)
                return false;
            _points.AddRange(primitive.Points);
            return true;
        }
    }
}
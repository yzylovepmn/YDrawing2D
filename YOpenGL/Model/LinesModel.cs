using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class LinesModel : MeshModel
    {
        public override void Draw(Shader shader, params object[] param)
        {
            var pen = param[0] as PenF;
            shader.SetVec3("color", pen.Color.GetData());
            //Set line width
            GLFunc.LineWidth(pen.Thickness / (float)param[1]);
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_LINES, 0, _points.Count);
        }

        public override bool TryAttachPrimitive(IPrimitive primitive)
        {
            switch (primitive.Type)
            {
                case PrimitiveType.Line:
                    var line = (Line)primitive;
                    if (_points.Count + 2 > Capacity)
                        return false;
                    _points.Add(line.Start);
                    _points.Add(line.End);
                    return true;
                case PrimitiveType.Cicle:
                    break;
                case PrimitiveType.Arc:
                    break;
                case PrimitiveType.Ellipse:
                    break;
                case PrimitiveType.Spline:
                    break;
                case PrimitiveType.Bezier:
                    break;
            }
            return true;
        }
    }
}
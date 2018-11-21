using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public class ArcModel : MeshModel
    {
        internal ArcModel() { }

        protected override void _BindData()
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.BindBuffer(GLConst.GL_ARRAY_BUFFER, _vbo[0]);
            GLFunc.BufferData(GLConst.GL_ARRAY_BUFFER, (_pointCount * 2 + _primitives.Count * 3) * sizeof(float), default(float[]), GLConst.GL_STATIC_DRAW);
            GLFunc.BufferSubData(GLConst.GL_ARRAY_BUFFER, 0, _pointCount * 2 * sizeof(float), GenVertice());
            GLFunc.BufferSubData(GLConst.GL_ARRAY_BUFFER, _pointCount * 2 * sizeof(float), _primitives.Count * 3 * sizeof(float), GenAttribute());
            GLFunc.EnableVertexAttribArray(0);
            GLFunc.VertexAttribPointer(0, 2, GLConst.GL_FLOAT, GLConst.GL_FALSE, 2 * sizeof(float), 0);
            GLFunc.EnableVertexAttribArray(1);
            GLFunc.VertexAttribPointer(1, 3, GLConst.GL_FLOAT, GLConst.GL_FALSE, 3 * sizeof(float), _pointCount * 2 * sizeof(float));
        }

        private float[] GenAttribute()
        {
            var attributes = new List<float>();

            foreach (var tuple in _primitives)
            {
                var arc = (_Arc)tuple.Item1;
                attributes.Add(arc.Radius);
                if (arc.IsCicle)
                {
                    // The startRadian and endRadian of cicle is zero!
                    attributes.Add(0);
                    attributes.Add(0);
                }
                else
                {
                    attributes.Add(arc.StartRadian);
                    attributes.Add(arc.EndRadian);
                }
            }

            return attributes.ToArray();
        }

        internal override void Draw(Shader shader)
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_POINTS, 0, _pointCount);
        }
    }
}
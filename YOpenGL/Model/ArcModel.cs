using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class ArcModel : MeshModel
    {
        private List<float> _attribute;

        internal override void BeginInit()
        {
            base.BeginInit();
            _attribute = new List<float>();
        }

        internal override void EndInit()
        {
            //base.EndInit();
            if (_hasInit) return;
            _hasInit = true;
            _vao = new uint[1];
            _vbo = new uint[1];
            GLFunc.GenVertexArrays(1, _vao);
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.GenBuffers(1, _vbo);
            GLFunc.BindBuffer(GLConst.GL_ARRAY_BUFFER, _vbo[0]);
            GLFunc.BufferData(GLConst.GL_ARRAY_BUFFER, (_points.Count * 2 + _attribute.Count) * sizeof(float), default(float[]), GLConst.GL_STATIC_DRAW);
            GLFunc.BufferSubData(GLConst.GL_ARRAY_BUFFER, 0, _points.Count * 2 * sizeof(float), _points.GetData());
            GLFunc.BufferSubData(GLConst.GL_ARRAY_BUFFER, _points.Count * 2 * sizeof(float), _attribute.Count * sizeof(float), _attribute.ToArray());
            GLFunc.EnableVertexAttribArray(0);
            GLFunc.VertexAttribPointer(0, 2, GLConst.GL_FLOAT, GLConst.GL_FALSE, 2 * sizeof(float), 0);
            GLFunc.EnableVertexAttribArray(1);
            GLFunc.VertexAttribPointer(1, 3, GLConst.GL_FLOAT, GLConst.GL_FALSE, 3 * sizeof(float), _points.Count * 2 * sizeof(float));
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var ret = base.TryAttachPrimitive(primitive, isOutline);
            if (ret)
            {
                var arc = (_Arc)primitive;
                _attribute.Add(arc.Radius);
                if (arc.IsCicle)
                {
                    // The startRadian and endRadian of cicle is zero!
                    _attribute.Add(0);
                    _attribute.Add(0);
                }
                else
                {
                    _attribute.Add(arc.StartRadian);
                    _attribute.Add(arc.EndRadian);
                }
                return true;
            }
            return false;
        }

        internal override void Draw()
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawArrays(GLConst.GL_POINTS, 0, _points.Count);
        }
    }
}
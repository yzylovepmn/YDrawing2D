using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class StreamModel : MeshModel
    {
        private Dictionary<int, int> _indices;

        internal override void BeginInit()
        {
            base.BeginInit();
            _indices = new Dictionary<int, int>();
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var points = primitive[isOutline];
            var cnt = points.Count();
            if (cnt < Capacity && _points.Count + cnt > Capacity)
                return false;
            _indices.Add(_points.Count, cnt + 1);
            _points.Add(new PointF());
            _points.AddRange(points);
            return true;
        }

        internal override void Draw()
        {
            GLFunc.BindVertexArray(_vao[0]);

            foreach (var key in _indices.Keys)
            {
                GLFunc.Clear(GLConst.GL_STENCIL_BUFFER_BIT);

                GLFunc.ColorMask(GLConst.GL_FALSE, GLConst.GL_FALSE, GLConst.GL_FALSE, GLConst.GL_FALSE);
                GLFunc.StencilFunc(GLConst.GL_ALWAYS, 0, 1);
                GLFunc.StencilOp(GLConst.GL_KEEP, GLConst.GL_KEEP, GLConst.GL_INVERT);
                GLFunc.DrawArrays(GLConst.GL_TRIANGLE_FAN, key, _indices[key]);

                GLFunc.ColorMask(GLConst.GL_TRUE, GLConst.GL_TRUE, GLConst.GL_TRUE, GLConst.GL_TRUE);
                GLFunc.StencilFunc(GLConst.GL_EQUAL, 1, 1);
                GLFunc.StencilOp(GLConst.GL_KEEP, GLConst.GL_KEEP, GLConst.GL_KEEP);
                GLFunc.DrawArrays(GLConst.GL_TRIANGLE_FAN, key, _indices[key]);
            }
        }

        protected override void _Dispose()
        {
            base._Dispose();
            _indices?.Clear();
            _indices = null;
        }
    }
}
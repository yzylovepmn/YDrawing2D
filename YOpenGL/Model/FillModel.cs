using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal class FillModel : MeshModel
    {
        /// <summary>
        /// EBO
        /// </summary>
        private uint[] _ebo;
        private List<uint> _indices;

        internal override void BeginInit()
        {
            base.BeginInit();
            _indices = new List<uint>();
        }

        internal override void EndInit()
        {
            base.EndInit();
            _ebo = new uint[1];
            GLFunc.GenBuffers(1, _ebo);
            GLFunc.BindBuffer(GLConst.GL_ELEMENT_ARRAY_BUFFER, _ebo[0]);
            GLFunc.BufferData(GLConst.GL_ELEMENT_ARRAY_BUFFER, _indices.Count * sizeof(uint), _indices.ToArray(), GLConst.GL_STATIC_DRAW);
        }

        internal override bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var oldCount = (uint)_points.Count;
            var ret = base.TryAttachPrimitive(primitive, isOutline);
            if (ret)
            {
                _indices.AddRange(GeometryHelper.GenIndices(primitive, oldCount));
                return true;
            }
            return false;
        }

        internal override void Draw(Shader shader)
        {
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.DrawElements(GLConst.GL_TRIANGLES, _indices.Count, GLConst.GL_UNSIGNED_INT, 0);
        }

        protected override void _Dispose()
        {
            base._Dispose();
            if (_ebo != null)
                GLFunc.DeleteBuffers(1, _ebo);
            _ebo = null;
        }
    }
}
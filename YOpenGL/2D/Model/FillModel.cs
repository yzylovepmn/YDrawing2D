using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;

namespace YOpenGL
{
    public class FillModel : MeshModel
    {
        public FillModel() { }

        /// <summary>
        /// EBO
        /// </summary>
        private uint[] _ebo;
        private int _indexCount;

        protected override void _BeforeEnd()
        {
            base._BeforeEnd();
            _indices = new List<uint>();
        }

        protected override void _AfterEnd()
        {
            _indexCount = _indices.Count;
            _indices = null;
        }

        protected override void _GenData()
        {
            base._GenData();
            _ebo = new uint[1];
            GenBuffers(1, _ebo);
        }

        protected override void _BindData()
        {
            base._BindData();
            BindBuffer(GL_ELEMENT_ARRAY_BUFFER, _ebo[0]);
            BufferData(GL_ELEMENT_ARRAY_BUFFER, _indices.Count * sizeof(uint), _indices.ToArray(), GL_STATIC_DRAW);
        }

        internal override void Draw(Shader shader)
        {
            if (!_hasInit) return;
            BindVertexArray(_vao[0]);
            DrawElements(GL_TRIANGLES, _indexCount, GL_UNSIGNED_INT, 0);
        }

        protected override void _Dispose()
        {
            if (_ebo != null)
                DeleteBuffers(1, _ebo);
            _ebo = null;
            base._Dispose();
        }
    }
}
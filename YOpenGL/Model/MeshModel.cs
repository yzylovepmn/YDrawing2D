using System;
using System.Collections.Generic;

namespace YOpenGL
{
    internal abstract class MeshModel : IDisposable
    {
        public const int Capacity = 4096;

        public MeshModel()
        {
            _hasInit = false;
        }

        internal IEnumerable<PointF> Points { get { return _points; } }
        protected List<PointF> _points;

        protected uint[] _vao;
        protected uint[] _vbo;
        private bool _hasInit;

        internal abstract void Draw(Shader shader, params object[] param);

        internal abstract bool TryAttachPrimitive(IPrimitive primitive);

        internal void BeginInit()
        {
            _Dispose();
            _points = new List<PointF>();
        }

        internal void EndInit()
        {
            if (_hasInit) return;
            _hasInit = true;
            _vao = new uint[1];
            _vbo = new uint[1];
            GLFunc.GenVertexArrays(1, _vao);
            GLFunc.BindVertexArray(_vao[0]);
            GLFunc.GenBuffers(1, _vbo);
            GLFunc.BindBuffer(GLConst.GL_ARRAY_BUFFER, _vbo[0]);
            GLFunc.BufferData(GLConst.GL_ARRAY_BUFFER, _points.Count * 2 * sizeof(float), _points.GetData(), GLConst.GL_STATIC_DRAW);
            GLFunc.EnableVertexAttribArray(0);
            GLFunc.VertexAttribPointer(0, 2, GLConst.GL_FLOAT, GLConst.GL_FALSE, 2 * sizeof(float), 0);
        }

        private void _Dispose()
        {
            if (_hasInit)
            {
                _hasInit = false;
                GLFunc.DeleteVertexArrays(1, _vao);
                GLFunc.DeleteBuffers(1, _vbo);
            }
        }

        public void Dispose()
        {
            _Dispose();
            _points = null;
        }
    }
}
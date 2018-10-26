using System;
using System.Linq;
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
        protected bool _hasInit;

        internal abstract void Draw();

        internal virtual bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var cnt = primitive[isOutline].Count();
            if (cnt < Capacity && _points.Count + cnt > Capacity)
                return false;
            _points.AddRange(primitive[isOutline]);
            return true;
        }

        internal virtual void BeginInit()
        {
            _Dispose();
            _points = new List<PointF>();
        }

        internal virtual void EndInit()
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

        protected virtual void _Dispose()
        {
            if (_vao != null)
                GLFunc.DeleteVertexArrays(1, _vao);
            if (_vbo != null)
                GLFunc.DeleteBuffers(1, _vbo);
            _vao = null;
            _vbo = null;
        }

        public void Dispose()
        {
            if (_hasInit)
            {
                _hasInit = false;
                _Dispose();
            }
            _points = null;
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;

namespace YOpenGL
{
    public abstract class MeshModel : IDisposable
    {
        public const int Capacity = 4096;

        internal MeshModel()
        {
            _hasInit = false;
        }

        internal bool NeedDisposed { get { return _primitives != null && _primitives.Count == 0; } }

        protected List<Tuple<IPrimitive, bool, int>> _primitives;
        protected int _pointCount;
        protected uint[] _vao;
        protected uint[] _vbo;
        protected List<uint> _indices;
        protected bool _hasInit;

        public bool NeedUpdate { get { return !_hasInit || _needUpdate; } }
        protected bool _needUpdate;

        internal abstract void Draw(Shader shader);

        internal virtual bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var cnt = primitive[isOutline].Count();
            if (cnt < Capacity && _pointCount + cnt > Capacity)
                return false;
            _pointCount += cnt;

            _primitives.Add(new Tuple<IPrimitive, bool, int>(primitive, isOutline, cnt));
            _needUpdate = true;
            return true;
        }

        internal void DetachPrimitive(IPrimitive primitive)
        {
            var tuple = GetTuple(primitive);
            _DetachBefore(tuple);
            _primitives.Remove(tuple);
            if (_primitives.Count != 0)
                _needUpdate = true;
        }

        protected virtual void _DetachBefore(Tuple<IPrimitive, bool, int> tuple)
        {
            _pointCount -= tuple.Item3;
        }

        protected Tuple<IPrimitive, bool, int> GetTuple(IPrimitive primitive)
        {
            foreach (var tuple in _primitives)
            {
                if (tuple.Item1 == primitive)
                    return tuple;
            }
            return null;
        }

        internal virtual void BeginInit()
        {
            _Dispose();
            _pointCount = 0;
            _needUpdate = false;
            _primitives = new List<Tuple<IPrimitive, bool, int>>();
        }

        internal void EndInit()
        {
            _BeforeEnd();

            if (_hasInit)
            {
                _Update();
                return;
            }
            _EndInit();

            _AfterEnd();
        }

        protected void _EndInit()
        {
            _needUpdate = false;
            _hasInit = true;
            _GenData();
            _BindData();
        }

        private void _Update()
        {
            if (!_needUpdate) return;
            _needUpdate = false;

            _BindData();
        }

        protected virtual void _BeforeEnd()
        {

        }

        protected virtual void _AfterEnd()
        {

        }

        protected virtual void _GenData()
        {
            _vao = new uint[1];
            _vbo = new uint[1];
            GenVertexArrays(1, _vao);
            GenBuffers(1, _vbo);
        }

        protected virtual void _BindData()
        {
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            BufferData(GL_ARRAY_BUFFER, _pointCount * 2 * sizeof(float), GenVertice(_indices), GL_STATIC_DRAW);
            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), 0);
        }

        protected virtual float[] GenVertice(List<uint> indices = null)
        {
            var points = new List<PointF>();

            foreach (var tuple in _primitives)
            {
                indices?.AddRange(GeometryHelper.GenIndices(tuple.Item1, (uint)points.Count));
                points.AddRange(tuple.Item1[tuple.Item2]);
            }

            return points.GetData();
        }

        protected virtual void _Dispose()
        {
            if (_vao != null)
                DeleteVertexArrays(1, _vao);
            if (_vbo != null)
                DeleteBuffers(1, _vbo);
            _vao = null;
            _vbo = null;

            _primitives?.Clear();
            _primitives = null;
        }

        public void Dispose()
        {
            if (_hasInit)
            {
                _hasInit = false;
                _Dispose();
            }
        }
    }
}
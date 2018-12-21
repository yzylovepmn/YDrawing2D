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

        protected Dictionary<IPrimitive, Tuple<bool, int>> _primitives;
        protected int _pointCount;
        protected uint[] _vao;
        protected uint[] _vbo;
        protected List<uint> _indices;
        protected bool _hasInit;

        public bool NeedUpdate { get { return !_hasInit || _needUpdate; } }
        protected bool _needUpdate;

        internal abstract void Draw(Shader shader);

        internal virtual void InvalidatePrimitives()
        {

        }

        internal virtual bool TryAttachPrimitive(IPrimitive primitive, bool isOutline = true)
        {
            var cnt = primitive[isOutline].Count();
            if (cnt < Capacity && _pointCount + cnt > Capacity)
                return false;
            _pointCount += cnt;

            if (primitive.Type == PrimitiveType.Line || primitive.Type == PrimitiveType.Point)
                _primitives.Add(primitive, null);
            else _primitives.Add(primitive, new Tuple<bool, int>(isOutline, cnt));
            _needUpdate = true;
            return true;
        }

        internal void DetachPrimitive(IPrimitive primitive)
        {
            var pair = GetPair(primitive);
            if (primitive.Type == PrimitiveType.Line || primitive.Type == PrimitiveType.Point)
                _pointCount -= primitive.Type == PrimitiveType.Line ? 2 : 1;
            else _pointCount -= pair.Value.Item2;
            _primitives.Remove(pair.Key);
            if (_primitives.Count != 0)
                _needUpdate = true;
        }

        protected KeyValuePair<IPrimitive, Tuple<bool, int>> GetPair(IPrimitive primitive)
        {
            return new KeyValuePair<IPrimitive, Tuple<bool, int>>(primitive, _primitives[primitive]);
        }

        internal virtual void BeginInit()
        {
            _Dispose();
            _pointCount = 0;
            _needUpdate = false;
            _primitives = new Dictionary<IPrimitive, Tuple<bool, int>>();
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
            _AfterEnd();
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
            var vertice = GenVertice();
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            BufferData(GL_ARRAY_BUFFER, _pointCount * 2 * sizeof(float), vertice, GL_STATIC_DRAW);
            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), 0);
        }

        protected virtual float[] GenVertice()
        {
            var points = new List<PointF>();

            foreach (var pair in _primitives)
            {
                _indices?.AddRange(GeometryHelper.GenIndices(pair.Key, (uint)points.Count));
                if (pair.Key.Type == PrimitiveType.Line || pair.Key.Type == PrimitiveType.Point)
                    points.AddRange(pair.Key[true]);
                else points.AddRange(pair.Key[pair.Value.Item1]);
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
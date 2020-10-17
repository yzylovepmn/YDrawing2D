using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using static YOpenGL.GL;

namespace YOpenGL._3D
{
    public class GLModel3D : IDisposable
    {
        public GLModel3D()
        {
        }

        public GLModel3D(IEnumerable<Point3F> points, IEnumerable<Vector3F> normals, IEnumerable<PointF> textureCoordinates, IEnumerable<uint> triangleIndices)
        {
            _points = points.ToList();
            _normals = normals.ToList();
            _textureCoordinates = textureCoordinates.ToList();
            _triangleIndices = triangleIndices.ToList();
        }

        public GLPanel3D Viewport
        {
            get { return _viewport; }
            internal set { _viewport = value; }
        }
        private GLPanel3D _viewport;

        public GLPrimitiveMode Mode 
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                    _mode = value;
            }
        }
        private GLPrimitiveMode _mode;

        public Rect3F Bounds { get { return _bounds; } }
        private Rect3F _bounds;

        public Shader Shader 
        { 
            get { return _shader; }
            set { _shader = value; }
        }
        private Shader _shader;

        internal bool HasInit { get { return _vao != null; } }

        private uint[] _vao;
        private uint[] _vbo;
        private uint[] _ebo;

        public IEnumerable<Point3F> Points { get { return _points; } }
        private List<Point3F> _points;

        public IEnumerable<Vector3F> Normals { get { return _normals; } }
        private List<Vector3F> _normals;

        public IEnumerable<PointF> TextureCoordinates { get { return _textureCoordinates; } }
        private List<PointF> _textureCoordinates;

        public IEnumerable<uint> TriangleIndices { get { return _triangleIndices; } }
        private List<uint> _triangleIndices;

        internal void Init()
        {
            if (HasInit) return;

            _vao = new uint[1];
            _vbo = new uint[1];
            _ebo = new uint[1];
            GenVertexArrays(1, _vao);
            GenBuffers(1, _vbo);
            GenBuffers(1, _ebo);

            _DataBinding();
            _IndicesBinding();
        }

        internal void Clean()
        {
            if (!HasInit) return;

            if (_vao != null)
                DeleteVertexArrays(1, _vao);
            if (_vbo != null)
                DeleteBuffers(1, _vbo);
            if (_ebo != null)
                DeleteBuffers(1, _ebo);
            _vao = null;
            _vbo = null;
            _ebo = null;
        }

        public void SetPoints(IEnumerable<Point3F> points)
        {
            _points = points.ToList();
            _UpdateBounds();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _DataBinding();
            }
        }

        public void SetNormals(IEnumerable<Vector3F> normals)
        {
            _normals = normals.ToList();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _DataBinding();
            }
        }

        public void SetTextureCoordinates(IEnumerable<PointF> textureCoordinates)
        {
            _textureCoordinates = textureCoordinates.ToList();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _DataBinding();
            }
        }

        public void SetTriangleIndices(IEnumerable<uint> triangleIndices)
        {
            _triangleIndices = triangleIndices.ToList();
            if (HasInit)
            {
                _viewport.MakeSureCurrentContext();
                _IndicesBinding();
            }
        }

        private void _DataBinding()
        {
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            var size = _CalcDataSize();
            BufferData(GL_ARRAY_BUFFER, size * sizeof(float), default(float[]), GL_STATIC_DRAW);
            var offset = 0;

            if (_points != null)
            {
                size = _points.Count * 3 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _points.GetData());

                EnableVertexAttribArray(0);
                VertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(0);

            if (_normals != null)
            {
                size = _normals.Count * 3 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _normals.GetData());

                EnableVertexAttribArray(1);
                VertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), offset);

                offset += size;
            }
            else DisableVertexAttribArray(1);

            if (_textureCoordinates != null)
            {
                size = _textureCoordinates.Count * 2 * sizeof(float);
                BufferSubData(GL_ARRAY_BUFFER, offset, size, _textureCoordinates.GetData());

                EnableVertexAttribArray(2);
                VertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), offset);
            }
            else DisableVertexAttribArray(2);
        }

        private void _IndicesBinding()
        {
            BindVertexArray(_vao[0]);
            
            if (_triangleIndices == null)
                BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
            else
            {
                BindBuffer(GL_ELEMENT_ARRAY_BUFFER, _ebo[0]);
                BufferData(GL_ELEMENT_ARRAY_BUFFER, _triangleIndices.Count * sizeof(uint), _triangleIndices.ToArray(), GL_STATIC_DRAW);
            }
        }

        private int _CalcDataSize()
        {
            return (_points == null ? 0 : _points.Count * 3) + (_normals == null ? 0 : _normals.Count * 3) + (_textureCoordinates == null ? 0 : _textureCoordinates.Count * 2);
        }

        private void _UpdateBounds()
        {
            _bounds = Rect3F.Empty;
            if (_points != null)
                foreach (var point in _points)
                    _bounds.Union(point);
        }

        #region Draw
        protected internal virtual void OnRender(Shader shader)
        {
            BindVertexArray(_vao[0]);
            var hasPoints = _points != null;
            var hasIndices = _triangleIndices != null;
            var mode = (uint)_mode;
            if (hasIndices)
                DrawElements(mode, hasPoints ? _triangleIndices.Count : 0, GL_UNSIGNED_INT, 0);
            else DrawArrays(mode, 0, hasPoints ? _points.Count : 0);
        }
        #endregion

        public void Dispose()
        {
            _viewport = null;
        }
    }
}
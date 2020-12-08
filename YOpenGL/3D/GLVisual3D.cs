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
    public class GLVisual3D : IDisposable
    {
        public GLVisual3D() 
        {
            _isHitTestVisible = true;
            _isVisible = true;
        }

        public bool IsHitTestVisible { get { return _isHitTestVisible; } set { _isHitTestVisible = value; } }
        private bool _isHitTestVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    _viewport?.Refresh();
                }
            }
        }
        private bool _isVisible;

        public GLPanel3D Viewport
        {
            get { return _viewport; }
            internal set { _viewport = value; }
        }
        private GLPanel3D _viewport;

        public GLModel3D Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    if (_model != null)
                        _model.Visual = null;
                    _model = value;
                    if (_model != null)
                    {
                        if (_model.Visual != null)
                            throw new InvalidOperationException("The model has associated visual");
                        _model.Visual = this;
                    }
                    InvalidateData();
                }
            }
        }
        private GLModel3D _model;

        #region Data
        internal bool IsInit { get { return _isInit; } }
        private bool _isInit;

        internal int PointCount { get { return _pointCount; } }
        private int _pointCount;

        private uint[] _vao;
        private uint[] _vbo;
        private uint[] _ebo;

        internal void Init()
        {
            if (_isInit) return;
            _isInit = true;

            _vao = new uint[1];
            _vbo = new uint[1];
            _ebo = new uint[1];
            GenVertexArrays(1, _vao);
            GenBuffers(1, _vbo);
            GenBuffers(1, _ebo);

            InvalidateData();
        }

        internal void Clean()
        {
            if (!_isInit) return;
            _isInit = false;

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
        #endregion

        internal void BufferBinding()
        {
            if (!_isInit) return;
            _viewport.MakeSureCurrentContext();
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            BindBuffer(GL_ELEMENT_ARRAY_BUFFER, _ebo[0]);
        }

        internal void InvalidateData()
        {
            if (!_isInit) return;
            _pointCount = _model == null ? 0 : _model.UpdateDataIndex(0);
            BufferBinding();
            BufferData(GL_ARRAY_BUFFER, _pointCount * 9 * sizeof(float), default(float[]), GL_DYNAMIC_DRAW);

            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), 0);
            EnableVertexAttribArray(1);
            VertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), _pointCount * 3 * sizeof(float));
            EnableVertexAttribArray(2);
            VertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), _pointCount * 6 * sizeof(float));
            EnableVertexAttribArray(3);
            VertexAttribPointer(3, 1, GL_FLOAT, GL_FALSE, 1 * sizeof(float), _pointCount * 8 * sizeof(float));
            _model?.BindingData();

            _viewport.Refresh();
        }

        internal void OnRender(Shader shader)
        {
            if (!_isVisible || _model == null) return;
            BufferBinding();
            _model.OnRender(shader);
        }

        public virtual void Dispose()
        {
            _model?.Dispose();
            Model = null;
            _viewport = null;
            Clean();
        }
    }
}
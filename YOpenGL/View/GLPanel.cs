﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace YOpenGL
{
    /// <summary>
    /// The positive direction of the X axis is right(→) and the positive direction of the Y axis is up(↑);
    /// </summary>
    public class GLPanel : UIElement, IDisposable
    {
        private static readonly string[] _shaders = new string[] { "Internal.vert", "Internal.frag" };

        public GLPanel(PointF origin, Color color)
        {
            Origin = origin;
            Color = color;
            _isInit = false;
            _isDisposed = false;
            _view = new MatrixF();
            _viewResverse = new MatrixF();
            _visuals = new List<GLVisual>();
            _fillModels = new Dictionary<Color, List<MeshModel>>();
            _lineModels = new Dictionary<PenF, List<MeshModel>>();
        }

        #region Private Field
        private bool _isDisposed;
        private bool _isInit;
        private bool _needUpdate;
        private HwndSource _source;
        private Shader _shader;

        private MatrixF _transformToDevice;
        private MatrixF _worldToNDC;
        private MatrixF _view;
        private MatrixF _viewResverse;

        private float _red;
        private float _green;
        private float _blue;
        #endregion

        #region Property
        public IEnumerable<GLVisual> Visuals { get { return _visuals; } }
        protected List<GLVisual> _visuals;

        private Dictionary<Color, List<MeshModel>> _fillModels;
        private Dictionary<PenF, List<MeshModel>> _lineModels;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _brush = new SolidColorBrush(_color);
                _red = _color.R / (float)byte.MaxValue;
                _green = _color.G / (float)byte.MaxValue;
                _blue = _color.B / (float)byte.MaxValue;
            }
        }
        private Color _color;
        private SolidColorBrush _brush;

        public PointF Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                    _origin = value;
            }
        }
        private PointF _origin;
        #endregion

        #region Transform
        public PointF ViewToWorld(PointF p)
        {
            p = new PointF(p.X - _origin.X, p.Y - _origin.Y);
            p = _viewResverse.Transform(p);
            return p;
        }

        public void Scale(float scaleX, float scaleY)
        {
            _view.Scale(scaleX, scaleY);
            _SaveViewResverse();
        }

        public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
        {
            _view.ScaleAt(scaleX, scaleY, centerX, centerY);
            _SaveViewResverse();
        }

        public void ScaleAt(PointF p, float scaleX, float scaleY)
        {
            ScaleAt(scaleX, scaleY, p.X, p.Y);
        }

        public void Translate(VectorF vector)
        {
            Translate(vector.X, vector.Y);
        }

        public void Translate(float offsetX, float offsetY)
        {
            _view.Translate(offsetX, offsetY);
            _SaveViewResverse();
        }

        private void _SaveViewResverse()
        {
            var view = _view;
            _view.Invert();
            _viewResverse = _view;
            _view = view;
        }
        #endregion

        #region Visual
        public GLVisual HitTest(PointF point, float sensitive = 3)
        {
            point = ViewToWorld(point);
            foreach (var visual in _visuals)
                if (visual.HitTest(point, sensitive * _viewResverse.M11))
                    return visual;
            return null;
        }

        public void AddVisual(GLVisual visual, bool isUpdate = false)
        {
            _visuals.Add(visual);
            if (isUpdate)
                Update(visual);
        }

        public void RemoveVisual(GLVisual visual)
        {
            _visuals.Remove(visual);
            _needUpdate = true;
        }

        public void Update(GLVisual visual)
        {
            _Update(visual);
            _needUpdate = true;
        }

        private void _Update(GLVisual visual)
        {
            visual.Update();
        }

        public void UpdateAll()
        {
            foreach (var visual in _visuals)
                _Update(visual);
            _needUpdate = true;
        }
        #endregion

        #region Override
        protected override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);
            if (!_isInit)
                Init();

            var width = Math.Max(1, (float)RenderSize.Width);
            var height = Math.Max(1, (float)RenderSize.Height);
            _worldToNDC = new MatrixF();
            _worldToNDC.Translate(_origin.X, _origin.Y);
            _worldToNDC.Scale(2 / width, 2 / height);
            _worldToNDC.Translate(-1, -1);
            GLFunc.Viewport(0, 0, (int)(width * _transformToDevice.M11), (int)(height * _transformToDevice.M22));
        }
        #endregion

        #region RenderFrame
        private void OnDispatchFrame(object sender, EventArgs e)
        {
            GLFunc.ClearColor(_red, _green, _blue, 1.0f);
            GLFunc.Clear(GLConst.GL_COLOR_BUFFER_BIT | GLConst.GL_DEPTH_BUFFER_BIT | GLConst.GL_STENCIL_BUFFER_BIT);

            _shader.SetMat3("worldToNDC", _worldToNDC);
            _shader.SetMat3("view", _view);
            _InvalidateModel();
            _DrawModels();

            GLFunc.SwapBuffers();
        }
        #endregion

        #region EventHook
        /// <summary>
        /// Make sure to capture mouse events
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(_brush, null, new Rect(RenderSize));
        }
        #endregion

        #region Internel
        internal void Init()
        {
            if (_isInit) return;
            _isInit = true;
            _source = (HwndSource)PresentationSource.FromVisual(this);
            _transformToDevice = (MatrixF)_source.CompositionTarget.TransformToDevice;

            GL.MakeContextCurrent(_source.Handle);
            GLFunc.Init();
            _shader = GenShader();

            GLFunc.Enable(GLConst.GL_DEPTH_TEST);
            GLFunc.Enable(GLConst.GL_CULL_FACE);
            GLFunc.Enable(GLConst.GL_LINE_SMOOTH);
            GLFunc.Enable(GLConst.GL_LINE_WIDTH);
            GLFunc.Hint(GLConst.GL_LINE_SMOOTH_HINT, GLConst.GL_FASTEST);
            _shader.Use();

            GL.DispatchFrame += OnDispatchFrame;
            GL.Start(60);
        }
        #endregion

        #region Private
        private void _InvalidateModel()
        {
            if (_needUpdate)
            {
                _needUpdate = false;
                _DisposeModels();
                foreach (var visual in _visuals)
                {
                    foreach (var primitive in visual.Context.Primitives)
                    {
                        _AttachLinesModel(primitive);
                        switch (primitive.Type)
                        {
                            case PrimitiveType.Line:
                                break;
                            case PrimitiveType.Cicle:
                                break;
                            case PrimitiveType.Arc:
                                break;
                            case PrimitiveType.Ellipse:
                                break;
                            case PrimitiveType.Spline:
                                break;
                            case PrimitiveType.Bezier:
                                break;
                            case PrimitiveType.Geometry:
                                break;
                        }
                    }
                }
                _EndInitModels();
            }
        }

        private void _AttachLinesModel(IPrimitive primitive)
        {
            var model = default(MeshModel);
            if (_lineModels.ContainsKey(primitive.Pen))
                model = _lineModels[primitive.Pen].Last();
            else
            {
                model = new LinesModel();
                model.BeginInit();
                _lineModels.Add(primitive.Pen, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive))
            {
                model = new LinesModel();
                model.BeginInit();
                _lineModels[primitive.Pen].Add(model);
                model.TryAttachPrimitive(primitive);
            }
        }

        private void _EndInitModels()
        {
            foreach (var value in _lineModels.Values)
                foreach (var model in value)
                    model.EndInit();

            foreach (var value in _fillModels.Values)
                foreach (var model in value)
                    model.EndInit();
        }

        private void _DrawModels()
        {
            foreach (var pair in _fillModels)
                foreach (var model in pair.Value)
                    model.Draw(_shader, pair.Key);

            foreach (var pair in _lineModels)
                foreach (var model in pair.Value)
                    model.Draw(_shader, pair.Key);
        }

        private static Shader GenShader()
        {
            var header = GL.CreateGLSLHeader();
            var source = new List<ShaderSource>();
            string code;
            foreach (var shader in _shaders)
            {
                using (var stream = new StreamReader(Resources.OpenStream(shader)))
                {
                    code = stream.ReadToEnd();
                    var type = default(ShaderType);
                    if (shader.EndsWith("vert"))
                        type = ShaderType.Vert;
                    if (shader.EndsWith("frag"))
                        type = ShaderType.Frag;
                    if (shader.EndsWith("geo"))
                        type = ShaderType.Geo;
                    source.Add(new ShaderSource(type, string.Concat(header, code)));
                }
            }
            return Shader.CreateShader(source);
        }
        #endregion

        private void _Dispose()
        {
            _DisposeVisuals();
            _visuals = null;

            _DisposeModels();
            _fillModels = null;
            _lineModels = null;
        }

        private void _DisposeVisuals()
        {
            _visuals.Dispose();
            _visuals.Clear();
        }

        private void _DisposeModels()
        {
            foreach (var item in _fillModels.Values)
                item?.Dispose();
            foreach (var item in _lineModels.Values)
                item?.Dispose();

            _fillModels.Clear();
            _lineModels.Clear();
        }

        public virtual void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            GL.DispatchFrame -= OnDispatchFrame;
            GL.Stop();

            _Dispose();

            GL.DeleteContext();
            GLFunc.Dispose();
            _source.Dispose();
        }
    }
}
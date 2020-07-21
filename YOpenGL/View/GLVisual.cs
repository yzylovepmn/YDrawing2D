using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using static YOpenGL.GL;

namespace YOpenGL
{
    public abstract class GLVisual
    {
        public GLVisual()
        {
            _context1 = new GLDrawContext(this);
            _context2 = new GLDrawContext(this);
            _bounds = RectF.Empty;
            _hitTestVisible = true;
            _isDeleted = true;

            _fillModels = new Dictionary<Color, List<MeshModel>>();
            _arrowModels = new Dictionary<Color, List<MeshModel>>();
            _streamModels = new List<MeshModel>();
            _lineModels = new SortedDictionary<PenF, List<MeshModel>>();
            _arcModels = new SortedDictionary<PenF, List<MeshModel>>();
            _pointModels = new SortedDictionary<PointPair, List<MeshModel>>();
        }

        #region Resource
        private Dictionary<Color, List<MeshModel>> _fillModels;
        private Dictionary<Color, List<MeshModel>> _arrowModels;
        private List<MeshModel> _streamModels;
        private SortedDictionary<PenF, List<MeshModel>> _lineModels;
        private SortedDictionary<PenF, List<MeshModel>> _arcModels;
        private SortedDictionary<PointPair, List<MeshModel>> _pointModels;
        #endregion

        public GLPanel Panel { get { return _panel; } internal set { _panel = value; } }
        protected GLPanel _panel;

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public bool HitTestVisible { get { return _hitTestVisible; } set { _hitTestVisible = value; } }
        protected bool _hitTestVisible;

        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; } }
        protected bool _isDeleted;

        #region Context
        internal GLDrawContext CurrentContext 
        { 
            get 
            {
                if (_flag == 0)
                    return _context1;
                else return _context2;
            }
        }

        internal GLDrawContext BackContext
        {
            get
            {
                if (_flag == 0)
                    return _context2;
                else return _context1;
            }
        }

        /******************* 双缓冲 ******************/
        private GLDrawContext _context1;
        private GLDrawContext _context2;
        private volatile int _flag;
        #endregion

        #region Async
        private int _async;

        internal bool IsUpdating { get { return Volatile.Read(ref _async) > 0; } }

        internal bool TryEnterUpdate()
        {
            return Interlocked.Increment(ref _async) == 1;
        }

        internal bool TryExitUpdate()
        {
            var ret = Interlocked.CompareExchange(ref _async, 0, 1) == 1;
            if (!ret)
                Volatile.Write(ref _async, 0);
            return ret;
        }

        internal void ExitUpdate()
        {
            Volatile.Write(ref _async, 0);
        }
        #endregion

        #region Data
        internal void AttachData()
        {
            var drawContext = CurrentContext;
            MakeSureCurrentContext(_panel.Context);

            foreach (var primitive in drawContext.Primitives)
            {
                if (!primitive.Pen.IsNULL || primitive.Type == PrimitiveType.ComplexGeometry)
                    _AttachModel(primitive, primitive.Pen);
                if (primitive.Filled)
                {
                    if (primitive.Type == PrimitiveType.ComplexGeometry)
                        _AttachStreamModel(primitive);
                    else if (primitive.Type == PrimitiveType.Arrow)
                        _AttachArrowModels(primitive);
                    else if (primitive.Type == PrimitiveType.Point)
                        _AttachPointModels(primitive);
                    else _AttachFillModels(primitive);
                }
            }
        }

        internal void DetachData()
        {
            _DisposeModels();
        }

        private void _AttachModel(IPrimitive primitive, PenF pen)
        {
            if (primitive.Type == PrimitiveType.ComplexGeometry)
            {
                foreach (var child in ((_ComplexGeometry)primitive).Children.Where(c => !c.Pen.IsNULL))
                    _AttachModel(child, child.Pen);
                return;
            }

            if (primitive.Type == PrimitiveType.SimpleGeometry)
            {
                foreach (var child in ((_SimpleGeometry)primitive).Stream.Where(c => !c.Pen.IsNULL))
                    _AttachModel(child, pen);
                return;
            }

            var model = default(MeshModel);
            var models = default(SortedDictionary<PenF, List<MeshModel>>);

            if (primitive.Type == PrimitiveType.Arc)
                models = _arcModels;
            else models = _lineModels;

            if (models.ContainsKey(pen))
                model = models[pen].Last();
            else
            {
                model = _GreateModel(primitive);
                models.Add(pen, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive))
            {
                model = _GreateModel(primitive);
                models[pen].Add(model);
                model.TryAttachPrimitive(primitive);
            }
            primitive.Model = model;
        }

        private void _AttachStreamModel(IPrimitive primitive)
        {
            var model = default(MeshModel);
            if (_streamModels.Count == 0)
            {
                model = _GreateModel(primitive);
                _streamModels.Add(model);
            }
            else model = _streamModels.Last();
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(primitive);
                _streamModels.Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private void _AttachFillModels(IPrimitive primitive)
        {
            var model = default(MeshModel);
            var models = _fillModels;

            if (models.ContainsKey(primitive.FillColor.Value))
                model = models[primitive.FillColor.Value].Last();
            else
            {
                model = _GreateModel(null, true);
                models.Add(primitive.FillColor.Value, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(null, true);
                models[primitive.FillColor.Value].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private void _AttachPointModels(IPrimitive primitive)
        {
            var point = primitive as _Point;
            var model = default(MeshModel);
            var models = _pointModels;
            var pair = new PointPair(point.FillColor.Value, point.PointSize);

            if (models.ContainsKey(pair))
                model = models[pair].Last();
            else
            {
                model = _GreateModel(point, true);
                models.Add(pair, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(point, true);
                models[pair].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private void _AttachArrowModels(IPrimitive primitive)
        {
            var model = default(MeshModel);
            var models = _arrowModels;

            if (models.ContainsKey(primitive.FillColor.Value))
                model = models[primitive.FillColor.Value].Last();
            else
            {
                model = _GreateModel(primitive, true);
                models.Add(primitive.FillColor.Value, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(primitive, true);
                models[primitive.FillColor.Value].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private MeshModel _GreateModel(IPrimitive primitive, bool isFilled = false)
        {
            var model = default(MeshModel);
            if (isFilled)
            {
                if (primitive == null)
                    model = new FillModel();
                else if (primitive.Type == PrimitiveType.Point)
                    model = new PointsModel();
                else model = new ArrowModel();
            }
            else if (primitive.Type == PrimitiveType.ComplexGeometry)
                model = new StreamModel();
            else if (primitive.Type == PrimitiveType.Arc)
                model = new ArcModel();
            else model = new LinesModel();
            model.BeginInit();
            return model;
        }

        internal void EndInitModels()
        {
            MakeSureCurrentContext(_panel.Context);

            foreach (var pair in _lineModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _lineModels.Remove(pair.Key);
            }

            foreach (var pair in _arcModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _arcModels.Remove(pair.Key);
            }

            foreach (var pair in _fillModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _fillModels.Remove(pair.Key);
            }

            foreach (var pair in _arrowModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _arrowModels.Remove(pair.Key);
            }

            foreach (var pair in _pointModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _pointModels.Remove(pair.Key);
            }

            foreach (var model in _streamModels.ToList())
            {
                if (model.NeedDisposed)
                {
                    model.Dispose();
                    _streamModels.Remove(model);
                }
                else model.EndInit();
            }
        }

        internal void DrawModels()
        {
            Enable(GL_CULL_FACE);
            foreach (var pair in _lineModels)
                _DrawModelHandle(pair, _panel.Lineshader);

            foreach (var pair in _arcModels)
                _DrawModelHandle(pair, _panel.Arcshader);

            foreach (var pair in _fillModels)
                _DrawFilledModelHandle(pair, _panel.Fillshader);

            foreach (var pair in _arrowModels)
                _DrawFilledModelHandle(pair, _panel.ArrowShader);

            foreach (var pair in _pointModels)
                _DrawPointModelsHandle(pair, _panel.Fillshader);
            Disable(GL_CULL_FACE);

            Enable(GL_STENCIL_TEST);
            _DrawSteramModelHandle(_streamModels, _panel.Fillshader);
            Disable(GL_STENCIL_TEST);
        }

        private void _DrawModelHandle(KeyValuePair<PenF, List<MeshModel>> pair, Shader shader)
        {
            //Set line width
            LineWidth(pair.Key.Thickness / _panel.TransformToDevice.M11);

            shader.Use();
            shader.SetBool("dashed", pair.Key.Data != null);
            shader.SetVec4("color", 1, pair.Key.Color.GetData());

            if (pair.Key.Data != null)
            {
                // Set line pattern
                BindTexture(GL_TEXTURE_1D, _panel.Texture_Dash[0]);
                TexImage1D(GL_TEXTURE_1D, 0, GL_RED, pair.Key.Data.Length, 0, GL_RED, GL_UNSIGNED_BYTE, pair.Key.Data);

                foreach (var model in pair.Value)
                    model.Draw(shader);
            }
            else foreach (var model in pair.Value)
                    model.Draw(shader);
        }

        private void _DrawFilledModelHandle(KeyValuePair<Color, List<MeshModel>> pair, Shader shader)
        {
            shader.Use();
            shader.SetVec4("color", 1, pair.Key.GetData());
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawPointModelsHandle(KeyValuePair<PointPair, List<MeshModel>> pair, Shader shader)
        {
            shader.Use();
            shader.SetVec4("color", 1, pair.Key.Color.GetData());
            PointSize(pair.Key.PointSize);
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawSteramModelHandle(List<MeshModel> models, Shader shader)
        {
            shader.Use();
            foreach (var model in models)
                model.Draw(shader);
        }
        #endregion

        private GLDrawContext RenderOpen()
        {
            // Reset context
            var back = BackContext;
            return back;
        }

        /// <summary>
        /// 交换前后缓冲
        /// </summary>
        private void _SwapBuffer()
        {
            _flag ^= 1;
        }

        private void _UpdateBounds(GLDrawContext context)
        {
            var scale = _panel.ScaleX;
            _bounds = RectF.Empty;
            if (context.HasPrimitives)
                foreach (var primitive in context.Primitives)
                    _bounds.Union(primitive.GetBounds(scale));
        }

        internal void Update()
        {
            var context = RenderOpen();
            Draw(context);
            _UpdateBounds(context);
            _SwapBuffer();
        }

        internal async Task UpdateAsync()
        {
            var context = RenderOpen();
            await Task.Factory.StartNew(() => 
            {
                do
                {
                    // 重置标志位
                    Volatile.Write(ref _async, 1);
                    // 重置上下文
                    context.Clear();
                    Draw(context);
                }
                while (Interlocked.CompareExchange(ref _async, 1, 1) > 1);
                _UpdateBounds(context);
                _SwapBuffer();
            });
        }

        internal void Reset()
        {
            if (_isDisposed) return;
            _context1.Clear();
            _context2.Clear();
            _bounds = RectF.Empty;
        }

        internal void Detach(GLDrawContext context)
        {
            foreach (var primitive in context.Primitives)
                _Deatch(primitive);
        }

        private void _Deatch(IPrimitive primitive)
        {
            if (primitive.Type == PrimitiveType.ComplexGeometry)
            {
                var geo = (_ComplexGeometry)primitive;
                foreach (var subgeo in geo.Children)
                    foreach (var item in subgeo.Stream)
                        _Deatch(item);
            }
            if (primitive.FillModel != null && !primitive.FillModel.IsDisposed)
            {
                primitive.FillModel.DetachPrimitive(primitive);
                primitive.FillModel = null;
            }
            if (primitive.Model != null && !primitive.Model.IsDisposed)
            {
                primitive.Model.DetachPrimitive(primitive);
                primitive.Model = null;
            }
        }

        internal bool HitTest(PointF p, float sensitive)
        {
            var scale = _panel.ScaleX;
            foreach (var primitive in CurrentContext.Primitives)
                if (primitive.GetBounds(scale).Contains(p, sensitive) && primitive.HitTest(p, sensitive, scale))
                    return true;
            return false;
        }

        internal bool HitTest(RectF rect, bool isFullContain = false)
        {
            var ret = rect.Contains(_bounds);
            if (!ret && !isFullContain)
            {
                var scale = _panel.ScaleX;
                foreach (var primitive in CurrentContext.Primitives)
                    if (primitive.GetBounds(scale).IntersectsWith(rect) && primitive.HitTest(rect, scale))
                        return true;
            }
            return ret;
        }

        /// <summary>
        /// Actual drawing logic
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Draw(GLDrawContext context);

        protected bool _isDisposed;
        internal bool NeedDispose;

        private void _DisposeModels()
        {
            MakeSureCurrentContext(_panel.Context);

            foreach (var item in _fillModels.Values)
                item?.DisposeInner();
            foreach (var item in _arrowModels.Values)
                item?.DisposeInner();
            _streamModels?.DisposeInner();
            foreach (var item in _lineModels.Values)
                item?.DisposeInner();
            foreach (var item in _arcModels.Values)
                item?.DisposeInner();
            foreach (var item in _pointModels.Values)
                item?.DisposeInner();

            _fillModels.Clear();
            _arrowModels.Clear();
            _streamModels.Clear();
            _lineModels.Clear();
            _arcModels.Clear();
            _pointModels.Clear();
        }

        public virtual bool Dispose()
        {
            if (IsUpdating)
            {
                NeedDispose = true;
                return false;
            }

            if (_isDisposed) return false;
            _isDisposed = true;
            NeedDispose = false;

            _DisposeModels();
            _context1?.Dispose();
            _context1 = null;
            _context2?.Dispose();
            _context2 = null;
            _panel = null;
            return true;
        }
    }
}
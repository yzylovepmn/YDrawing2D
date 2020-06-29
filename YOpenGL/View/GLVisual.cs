using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        }

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

            _context1?.Dispose();
            _context1 = null;
            _context2?.Dispose();
            _context2 = null;
            _panel = null;
            return true;
        }
    }
}
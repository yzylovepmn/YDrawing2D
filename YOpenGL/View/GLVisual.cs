using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public abstract class GLVisual : IDisposable
    {
        public GLVisual()
        {
            _context = new GLDrawContext(this);
            _bounds = RectF.Empty;
            _hitTestVisible = true;
        }

        public GLPanel Panel { get { return _panel; } internal set { _panel = value; } }
        protected GLPanel _panel;

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        internal GLDrawContext Context { get { return _context; } }
        private GLDrawContext _context;

        public bool HitTestVisible { get { return _hitTestVisible; } set { _hitTestVisible = value; } }
        protected bool _hitTestVisible;

        private GLDrawContext RenderOpen()
        {
            // Reset context
            _context.Reset();
            return _context;
        }

        private void _UpdateBounds()
        {
            _bounds = RectF.Empty;
            if (_context.HasPrimitives)
                foreach (var primitive in _context.Primitives)
                    _bounds.Union(primitive.Bounds);
        }

        internal void Update()
        {
            var context = RenderOpen();
            Draw(context);
            _UpdateBounds();
        }

        internal void Reset()
        {
            foreach (var primitive in _context.Primitives)
                _Deatch(primitive, true);
        }

        internal void Detach()
        {
            foreach (var primitive in _context.Primitives)
                _Deatch(primitive);
        }

        private void _Deatch(IPrimitive primitive, bool isReset = false)
        {
            if (primitive.Type == PrimitiveType.ComplexGeometry)
            {
                var geo = (_ComplexGeometry)primitive;
                foreach (var subgeo in geo.Children)
                    foreach (var item in subgeo.Stream)
                        _Deatch(item, isReset);
            }
            if (primitive.FillModel != null)
            {
                if (!isReset)
                    primitive.FillModel.DetachPrimitive(primitive);
                primitive.FillModel = null;
            }
            if (primitive.Model != null)
            {
                if (!isReset)
                    primitive.Model.DetachPrimitive(primitive);
                primitive.Model = null;
            }
        }

        internal bool HitTest(PointF p, float sensitive)
        {
            foreach (var primitive in _context.Primitives)
                if (primitive.Bounds.Contains(p, sensitive) && primitive.HitTest(p, sensitive))
                    return true;
            return false;
        }

        internal bool HitTest(RectF rect, bool isFullContain = false)
        {
            var ret = rect.Contains(_bounds);
            if (!ret && !isFullContain)
            {
                foreach (var primitive in _context.Primitives)
                    if (primitive.Bounds.IntersectsWith(rect) && primitive.HitTest(rect))
                        return true;
            }
            return ret;
        }

        /// <summary>
        /// Actual drawing logic
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Draw(GLDrawContext context);

        public virtual void Dispose()
        {
            _context.Dispose();
            _context = null;
            _panel = null;
        }
    }
}
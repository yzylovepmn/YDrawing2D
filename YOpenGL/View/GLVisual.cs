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
            _hitTestVisible = true;
        }

        public GLPanel Panel { get { return _panel; } internal set { _panel = value; } }
        protected GLPanel _panel;

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

        internal void Update()
        {
            var context = RenderOpen();
            Draw(context);
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
                        _Deatch(item);
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
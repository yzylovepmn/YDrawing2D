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
        }

        public GLPanel Panel { get { return _panel; } internal set { _panel = value; } }
        protected GLPanel _panel;

        internal GLDrawContext Context { get { return _context; } }
        private GLDrawContext _context;

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
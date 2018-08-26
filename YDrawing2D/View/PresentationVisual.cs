using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Model;

namespace YDrawing2D.View
{
    public abstract class PresentationVisual : IDisposable
    {
        public PresentationVisual()
        {
            _context = new PresentationContext(this);
        }

        internal PresentationPanel Panel { get { return _panel; } set { _panel = value; } }
        private PresentationPanel _panel;

        internal PresentationContext Context { get { return _context; } }
        private PresentationContext _context;

        private IContext RenderOpen()
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

        /// <summary>
        /// Actual drawing logic
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Draw(IContext context);

        public void Dispose()
        {
            _context?.Dispose();
            _panel = null;
        }
    }
}
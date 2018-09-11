using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YDrawing2D.Extensions;
using YDrawing2D.Model;
using YDrawing2D.Util;

namespace YDrawing2D.View
{
    internal enum Mode
    {
        Normal,
        WatingForUpdate,
        Updating,
        Completed
    }

    public abstract class PresentationVisual : IDisposable
    {
        public PresentationVisual()
        {
            _context = new PresentationContext(this);
            _transform = new StackTransform();
        }

        public PresentationPanel Panel { get { return _panel; } internal set { _panel = value; } }
        private PresentationPanel _panel;

        internal PresentationContext Context { get { return _context; } }
        private PresentationContext _context;

        internal StackTransform Transform { get { return _transform; } }
        private StackTransform _transform;

        internal Mode Mode { get { return _mode; } set { _mode = value; } }
        private Mode _mode;

        #region Transform
        public void Translate(double offsetX, double offsetY, bool toUpdate = true)
        {
            _transform.PushTranslate(offsetX, offsetY);
            if (toUpdate)
                _panel.Update(this);
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY, bool toUpdate = true)
        {
            _transform.PushScaleAt(scaleX, scaleY, centerX, centerY);
            if (toUpdate)
                _panel.Update(this);
        }

        public void Scale(double scaleX, double scaleY, bool toUpdate = true)
        {
            _transform.PushScale(scaleX, scaleY);
            if (toUpdate)
                _panel.Update(this);
        }

        public void Rotate(double angle, bool toUpdate = true)
        {
            _transform.PushRotate(angle);
            if (toUpdate)
                _panel.Update(this);
        }

        public void RotateAt(double angle, double centerX, double centerY, bool toUpdate = true)
        {
            _transform.PushRotateAt(angle, centerX, centerY);
            if (toUpdate)
                _panel.Update(this);
        }

        public void Pop(bool toUpdate = true)
        {
            _transform.Pop();
            if (toUpdate)
                _panel.Update(this);
        }
        #endregion

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

        internal bool Contains(Int32Point p, Int32 color)
        {
            foreach (var primitive in _context.Primitives)
                if (primitive.Property.Pen.Color == color
                    && primitive.Property.Bounds.Contains(p)
                    && primitive.HitTest(p))
                    return true;
            return false;
        }

        public void Dispose()
        {
            _transform.Dispose();
            _transform = null;
            _context.Dispose();
            _context = null;
            _panel = null;
        }
    }
}
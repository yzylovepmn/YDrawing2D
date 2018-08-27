﻿using System;
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

        internal bool Contains(Int32Point p, Int32 color)
        {
            foreach (var primitive in _context.Primitives)
                if (primitive.Property.Color == color
                    && primitive.Property.Bounds.Contains(p)
                    && primitive.HitTest(p))
                    return true;
            return false;
        }

        public void Dispose()
        {
            _context?.Dispose();
            _panel = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YDrawing2D.Model;
using YDrawing2D.Util;
using YDrawing2D.View;

namespace YDrawing2D
{
    /// <summary>
    /// The panel with the origin in the upper left corner
    /// </summary>
    public class PresentationPanel : UIElement, IDisposable
    {
        public PresentationPanel(int pixelWidth, int pixelHeight, double dpiX, double dpiY, Color backColor)
        {
            DPIRatio = dpiX / GeometryHelper.SysDPI;
            _image = new WriteableBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Bgra32, null);
            _bounds = new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            _visuals = new List<PresentationVisual>();

            BackColor = backColor;
        }

        internal readonly double DPIRatio;

        public Int32Rect Bounds { get { return _bounds; } }
        private Int32Rect _bounds;

        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    UpdateAll();
                }
            }
        }
        private Color _backColor;

        internal WriteableBitmap Image { get { return _image; } }
        private WriteableBitmap _image;

        public IEnumerable<PresentationVisual> Visuals { get { return _visuals; } }
        private List<PresentationVisual> _visuals;

        public void AddVisual(PresentationVisual visual)
        {
            if (!_visuals.Contains(visual))
            {
                _visuals.Add(visual);
                visual.Panel = this;
                Update(visual);
            }
        }

        public void RemoveVisual(PresentationVisual visual)
        {
            if (_visuals.Contains(visual))
            {
                _visuals.Remove(visual);
                visual.Panel = null;
                UpdateAll();
            }
        }

        #region Render
        internal void Update(PresentationVisual visual)
        {
            EnterRender();
            _Update(visual);
            ExitRender();
            InvalidateVisual();
        }

        internal void UpdateAll()
        {
            var color = Helper.CalcColor(_backColor);
            ClearBuffer(color);
            if (_visuals.Count > 0)
            {
                EnterRender();
                foreach (var visual in _visuals)
                    _Update(visual);
                ExitRender();
                InvalidateVisual();
            }
        }

        internal void _Update(PresentationVisual visual)
        {
            visual.Update();
            // TODO
            foreach (var primitive in visual.Context.Primitives)
            {
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                        var line = (Line)primitive;
                        DrawLine(line.Start, line.End, line.Property.Color, line.Property.Thickness);
                        _image.AddDirtyRect(GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds));
                        break;
                }
            }
        }

        internal void ClearBuffer(int color)
        {
            EnterRender();
            var start = _image.BackBuffer;
            for (int i = 0; i < _image.PixelHeight; i++)
            {
                for (int j = 0; j < _image.PixelWidth; j++)
                {
                    DrawPixel(start, color);
                    start += GeometryHelper.PixelByteLength;
                }
            }
            _image.AddDirtyRect(new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight));
            ExitRender();
        }

        /// <summary>
        /// must call this method before do any render method.
        /// </summary>
        internal void EnterRender()
        {
            _image.Lock();
        }

        /// <summary>
        /// must call this method after any render method done.
        /// </summary>
        internal void ExitRender()
        {
            _image.Unlock();
        }

        internal void DrawLine(Int32Point start, Int32Point end, int color, int thickness = 1)
        {
            var line = GeometryHelper.CalcLinePoints(start, end);

            foreach (var point in line)
                DrawPoint(point, color, thickness);
        }

        internal void DrawPoint(Int32Point pos, int color, int thickness = 1)
        {
            if (pos.X >= _image.PixelWidth
                || pos.Y >= _image.PixelHeight
                || pos.X < 0 || pos.Y < 0)
                return;

            var points = GeometryHelper.CalcPoint(pos.X, pos.Y, _image.BackBuffer, _image.BackBufferStride, thickness, _bounds);

            foreach (var point in points)
                DrawPixel(point, color);
        }

        unsafe private void DrawPixel(IntPtr ptr, int color)
        {
            *((int*)ptr) = color;
        }
        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_image == null) return;
            drawingContext.DrawImage(_image, new Rect(new Point(), new Size(_image.Width, _image.Height)));
        }

        public void Dispose()
        {
            foreach (var visual in _visuals)
                visual.Dispose();
            _visuals.Clear();
            _visuals = null;

            _image = null;
        }
    }
}
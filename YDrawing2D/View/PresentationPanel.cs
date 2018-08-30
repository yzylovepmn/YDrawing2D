using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YDrawing2D.Extensions;
using YDrawing2D.Model;
using YDrawing2D.Util;
using YDrawing2D.View;

namespace YDrawing2D
{
    /// <summary>
    /// The world coordinates are lower left
    /// </summary>
    public class PresentationPanel : UIElement, IDisposable
    {
        public PresentationPanel(double width, double height, double dpiX, double dpiY, Color backColor)
        {
            DPIRatio = dpiX / GeometryHelper.SysDPI;
            VisualHelper.HitTestThickness = (int)(VisualHelper.HitTestThickness * DPIRatio);
            VisualHelper.HitTestThickness = Math.Max(1, VisualHelper.HitTestThickness);
            _image = new WriteableBitmap((int)(width * DPIRatio), (int)(height * DPIRatio), dpiX, dpiY, PixelFormats.Bgra32, null);
            _bounds = new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            _visuals = new ObservableCollection<PresentationVisual>();
            _transform = new MatrixTransform();

            BackColor = backColor;
        }

        internal readonly double DPIRatio;

        public Int32Rect Bounds { get { return _bounds; } }
        private Int32Rect _bounds;

        internal IntPtr Offset { get { return _image.BackBuffer; } }

        internal int Stride { get { return _image.BackBufferStride; } }

        internal double ImageHeight { get { return _image.Height; } }

        /// <summary>
        /// The background color used by the panel
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    _backColorValue = Helper.CalcColor(_backColor);
                    UpdateAll();
                }
            }
        }
        private Color _backColor;

        public int BackColorValue { get { return _backColorValue; } }
        private int _backColorValue;

        /// <summary>
        /// Internally used bitmap
        /// </summary>
        internal WriteableBitmap Image { get { return _image; } }
        private WriteableBitmap _image;

        public IEnumerable<PresentationVisual> Visuals { get { return _visuals; } }
        private ObservableCollection<PresentationVisual> _visuals;

        internal MatrixTransform Transform { get { return _transform; } }
        private MatrixTransform _transform;

        public double ScaleX { get { return _scaleX; } }
        private double _scaleX = 1;

        public double ScaleY { get { return _scaleY; } }
        private double _scaleY = 1;

        public void Translate(double offsetX, double offsetY, bool toUpdate = true)
        {
            var m = _transform.Matrix;
            m.Translate(offsetX, offsetY);
            _transform.Matrix = m;
            if (toUpdate)
                UpdateAll();
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY, bool toUpdate = true)
        {
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            var m = _transform.Matrix;
            m.ScaleAt(scaleX, scaleY, centerX, centerY);
            _transform.Matrix = m;
            if (toUpdate)
                UpdateAll();
        }

        public void Scale(double scaleX, double scaleY, bool toUpdate = true)
        {
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            var m = _transform.Matrix;
            m.Scale(scaleX, scaleY);
            _transform.Matrix = m;
            if (toUpdate)
                UpdateAll();
        }

        /// <summary>
        /// You must call <see cref="UpdateAll"/> or <see cref="Update(PresentationVisual)"/> after calling this method,
        /// Make sure the panel is updated!
        /// </summary>
        public void AddVisual(PresentationVisual visual)
        {
            if (!_visuals.Contains(visual))
            {
                _visuals.Add(visual);
                visual.Panel = this;
                //Update(visual);
            }
        }

        /// <summary>
        /// You must call <see cref="UpdateAll"/> or <see cref="Update(PresentationVisual)"/> after calling this method,
        /// Make sure the panel is updated!
        /// </summary>
        public void RemoveVisual(PresentationVisual visual)
        {
            if (_visuals.Contains(visual))
            {
                _visuals.Remove(visual);
                visual.Panel = null;
                //Update(visual);
            }
        }

        /// <summary>
        /// Move the visual to the top of the Z-order
        /// </summary>
        /// <param name="visual">The visual to move</param>
        /// <param name="needUpdate">Whether to refresh the Visual</param>
        public void MoveToTop(PresentationVisual visual, bool needUpdate = false)
        {
            _visuals.Move(_visuals.IndexOf(visual), _visuals.Count - 1);
            if (needUpdate)
                Update(visual);
        }

        #region Render
        /// <summary>
        /// Update visual object;
        /// </summary>
        /// <param name="visual">the visual need to update</param>
        public void Update(PresentationVisual visual, bool isSingle = false)
        {
            if (visual == null) return;
            EnterRender();
            _ClearVisual(visual);

            if (!isSingle)
                foreach (var _visual in _visuals.Where(other => other != visual && other.IsIntersectWith(visual)))
                    _Update(_visual);

            _Update(visual);
            ExitRender();
        }

        /// <summary>
        /// Update the entire panel
        /// </summary>
        public void UpdateAll()
        {
            EnterRender();
            _ClearBuffer(_backColorValue);
            foreach (var visual in _visuals)
                _Update(visual);
            ExitRender();
        }

        internal void _Update(PresentationVisual visual)
        {
            visual.Update();
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!_bounds.IsIntersectWith(primitive.Property.Bounds)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds);
                _UpdateBounds(bounds);
            }
        }

        internal void _ClearVisual(PresentationVisual visual)
        {
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!_bounds.IsIntersectWith(primitive.Property.Bounds)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds, true);
                _UpdateBounds(bounds);
            }
        }

        /// <summary>
        /// Clear panel buffer with the specified color
        /// </summary>
        /// <param name="color">The color to clear</param>
        public void ClearBuffer(int color)
        {
            EnterRender();
            _ClearBuffer(color);
            ExitRender();
        }

        /// <summary>
        /// Clear panel buffer with the specified color(Internal call)
        /// </summary>
        /// <param name="color">The color to clear</param>
        internal void _ClearBuffer(int color)
        {
            var start = _image.BackBuffer;
            for (int i = 0; i < _bounds.Height; i++)
            {
                for (int j = 0; j < _bounds.Width; j++)
                {
                    _DrawPixel(start, color);
                    start += GeometryHelper.PixelByteLength;
                }
            }
            _UpdateBounds(_bounds);
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

        private void _UpdateBounds(Int32Rect bounds)
        {
            _image.AddDirtyRect(bounds);
        }

        private void _DrawPrimitive(IPrimitive primitive, Int32Rect bounds, bool isClear = false)
        {
            IEnumerable<Int32Point> points;
            if (primitive.Property.Pen.Dashes == null)
                points = GeometryHelper.CalcPrimitivePoints(primitive).Where(p => bounds.Contains(p));
            else points = Helper.FilterWithDashes(GeometryHelper.CalcPrimitivePoints(primitive), primitive.Property.Pen.Dashes).Where(p => bounds.Contains(p));

            if (isClear)
                foreach (var point in points)
                    _DrawPoint(point, _backColorValue, primitive.Property.Pen.Thickness);
            else foreach (var point in points)
                    _DrawPoint(point, primitive.Property.Pen.Color, primitive.Property.Pen.Thickness);
        }

        private void _DrawPoint(Int32Point pos, int color, int thickness = 1)
        {
            var points = GeometryHelper.CalcPositions(pos.X, pos.Y, Offset, Stride, thickness, _bounds);

            foreach (var point in points)
                _DrawPixel(point, color);
        }

        unsafe private void _DrawPixel(IntPtr ptr, int color)
        {
            *((int*)ptr) = color;
        }

        internal int GetColor(Int32 x, Int32 y)
        {
            var start = _image.BackBuffer;
            start += y * Stride;
            start += x * GeometryHelper.PixelByteLength;
            return _GetPixel(start);
        }

        unsafe private int _GetPixel(IntPtr ptr)
        {
            return *((int*)ptr);
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
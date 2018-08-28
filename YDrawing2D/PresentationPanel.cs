using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public void Translate(double offsetX, double offsetY)
        {
            var m = _transform.Matrix;
            m.Translate(offsetX, offsetY);
            _transform.Matrix = m;
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            var m = _transform.Matrix;
            m.ScaleAt(scaleX, scaleY, centerX, centerY);
            _transform.Matrix = m;
        }

        public void Scale(double scaleX, double scaleY)
        {
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            var m = _transform.Matrix;
            m.Scale(scaleX, scaleY);
            _transform.Matrix = m;
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
        /// This method can only be called when adding this <param name="visual"/>, otherwise call <see cref="UpdateAll"/>
        /// </summary>
        /// <param name="visual">the visual need to update</param>
        public void Update(PresentationVisual visual, bool isSingle = false)
        {
            if (visual == null) return;
            EnterRender();
            _ClearVisual(visual);

            if (!isSingle)
                foreach (var _visual in _visuals.Where(other => other.IsIntersectWith(visual)))
                    if (_visual != visual)
                        _Update(_visual);

            _Update(visual);
            ExitRender();
            InvalidateVisual();
        }

        /// <summary>
        /// Update the entire panel
        /// </summary>
        public void UpdateAll()
        {
            ClearBuffer(_backColorValue);
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
                if (!_bounds.IsIntersectWith(primitive.Property.Bounds)) continue;
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                        var line = (Line)primitive;
                        DrawLine(line.Start, line.End, line.Property);
                        break;
                    case PrimitiveType.Cicle:
                        var cicle = (Cicle)primitive;
                        DrawCicle(cicle.Center, cicle.Radius, cicle.Property);
                        break;
                }
                UpdateBounds(GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds));
            }
        }

        internal void _ClearVisual(PresentationVisual visual)
        {
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!_bounds.IsIntersectWith(primitive.Property.Bounds)) continue;
                switch (primitive.Type)
                {
                    case PrimitiveType.Line:
                        var line = (Line)primitive;
                        DrawLine(line.Start, line.End, line.Property, true);
                        break;
                    case PrimitiveType.Cicle:
                        var cicle = (Cicle)primitive;
                        DrawCicle(cicle.Center, cicle.Radius, cicle.Property, true);
                        break;
                }
                UpdateBounds(GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds));
            }
        }

        internal void ClearBuffer(int color)
        {
            EnterRender();
            var start = _image.BackBuffer;
            for (int i = 0; i < _bounds.Height; i++)
            {
                for (int j = 0; j < _bounds.Width; j++)
                {
                    DrawPixel(start, color);
                    start += GeometryHelper.PixelByteLength;
                }
            }
            UpdateBounds(_bounds);
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

        internal void UpdateBounds(Int32Rect bounds)
        {
            _image.AddDirtyRect(bounds);
        }

        internal void DrawLine(Int32Point start, Int32Point end, PrimitiveProperty property, bool useBackColor = false)
        {
            var line = GeometryHelper.CalcLinePoints(start, end);

            foreach (var point in line)
                DrawPoint(point, useBackColor ? _backColorValue : property.Color, property.Thickness);
        }

        internal void DrawCicle(Int32Point center, Int32 radius, PrimitiveProperty property, bool useBackColor = false)
        {
            var cicle = GeometryHelper.CalcCiclePoints(center, radius);

            foreach (var point in cicle)
                DrawPoint(point, useBackColor ? _backColorValue : property.Color, property.Thickness);
        }

        internal void DrawPoint(Int32Point pos, int color, int thickness = 1)
        {
            if (!_bounds.Contains(pos))
                return;

            var points = GeometryHelper.CalcPositions(pos.X, pos.Y, Offset, Stride, thickness, _bounds);

            foreach (var point in points)
                DrawPixel(point, color);
        }

        unsafe private void DrawPixel(IntPtr ptr, int color)
        {
            *((int*)ptr) = color;
        }

        unsafe internal int GetColor(Int32 x, Int32 y)
        {
            var start = _image.BackBuffer;
            start += y * Stride;
            start += x * GeometryHelper.PixelByteLength;
            return GetColor(start);
        }

        unsafe internal int GetColor(IntPtr ptr)
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
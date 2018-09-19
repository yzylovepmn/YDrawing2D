using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using YDrawing2D.Extensions;
using YDrawing2D.Model;
using YDrawing2D.Util;
using YDrawing2D.View;

namespace YDrawing2D
{
    public enum RenderMode
    {
        Sync,
        Async
    }

    /// <summary>
    /// The world coordinates are left-bottom
    /// </summary>
    public class PresentationPanel : UIElement, IDisposable
    {
        public PresentationPanel(double width, double height, double dpiX, double dpiY, Color backColor, RenderMode renderMode)
        {
            _renderMode = renderMode;

            DPIRatio = dpiX / GeometryHelper.SysDPI;
            VisualHelper.HitTestThickness = (int)(VisualHelper.HitTestThickness * DPIRatio);
            VisualHelper.HitTestThickness = Math.Max(1, VisualHelper.HitTestThickness);

            _image = new WriteableBitmap((int)(width * DPIRatio), (int)(height * DPIRatio), dpiX, dpiY, PixelFormats.Bgra32, null);
            _bounds = new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight);
            _offset = _image.BackBuffer;
            _stride = _image.BackBufferStride;
            _imageHeight = _image.Height;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            _visuals = new List<PresentationVisual>();
            _transform = new Matrix();

            #region Async
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(5) };
            _timer.Tick += _UpdateSample;
            #endregion

            BackColor = backColor;
        }

        public RenderMode RenderMode { get { return _renderMode; } }
        private RenderMode _renderMode;

        internal readonly double DPIRatio;

        public Int32Rect Bounds { get { return _bounds; } }
        private Int32Rect _bounds;

        internal IntPtr Offset { get { return _offset; } }
        private IntPtr _offset;

        internal int Stride { get { return _stride; } }
        private int _stride;

        internal double ImageHeight { get { return _imageHeight; } }
        private double _imageHeight;

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

        public byte[] BackColorValue { get { return _backColorValue; } }
        private byte[] _backColorValue;

        /// <summary>
        /// Internally used bitmap
        /// </summary>
        internal WriteableBitmap Image { get { return _image; } }
        private WriteableBitmap _image;

        public IEnumerable<PresentationVisual> Visuals { get { return _visuals;} }
        private List<PresentationVisual> _visuals;

        internal Matrix Transform { get { return _transform; } }
        private Matrix _transform;

        public double ScaleX { get { return _scaleX; } }
        private double _scaleX = 1;

        public double ScaleY { get { return _scaleY; } }
        private double _scaleY = 1;

        #region Async
        DispatcherTimer _timer;
        bool _isUpdatingAll = false;
        bool _needUpdate = false;
        bool _completeLoop = false;
        int _cnt = 0;
        object _loopLock = new object();

        /// <summary>
        /// Update the entire panel async
        /// </summary>
        internal void _UpdateAllAsync()
        {
            Monitor.Enter(_loopLock);
            if (_isUpdatingAll)
            {
                _needUpdate = true;
                Monitor.Exit(_loopLock);
                return;
            }
            _isUpdatingAll = true;
            foreach (var visual in _visuals)
                visual.Mode = Mode.WatingForUpdate;
            ClearBuffer(_backColorValue);
            _cnt = 0;
            _timer.Start();
            ThreadPool.QueueUserWorkItem(e =>
            {
                _completeLoop = false;

                var visuals = default(List<PresentationVisual>);
                lock (this)
                    visuals = _visuals.ToList();

                foreach (var visual in visuals)
                {
                    if (_needUpdate) break;
                    _UpdateAsync(visual);
                }
                _completeLoop = true;
            });
            Monitor.Exit(_loopLock);
        }

        private void _UpdateSample(object sender, EventArgs e)
        {
            Monitor.Enter(_loopLock);
            UpdateBounds(_bounds);
            if (_needUpdate)
            {
                //Waiting for the last round of updates to end
                while (true)
                {
                    if (!_completeLoop)
                        continue;
                    break;
                }
                _cnt = 0;
                _needUpdate = false;
                _isUpdatingAll = false;
                _timer.Stop();
                Monitor.Exit(_loopLock);
                // Restart
                _UpdateAllAsync();
            }
            else
            {
                foreach (var visual in _visuals.Where(v => v.Mode == Mode.Completed))
                {
                    visual.Mode = Mode.Normal;
                    _cnt++;
                }
                if (_cnt == _visuals.Count)
                {
                    _timer.Stop();
                    _isUpdatingAll = false;
                }
                Monitor.Exit(_loopLock);
            }
        }

        internal void _UpdateAsync(PresentationVisual visual)
        {
            if (visual.Mode != Mode.WatingForUpdate) return;
            visual.Mode = Mode.Updating;
            visual.Update();
            foreach (var primitive in visual.Context.Primitives)
            {
                if (primitive == null || !_bounds.IsIntersectWith(primitive)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds);
            }
            visual.Mode = Mode.Completed;
        }
        #endregion

        #region Sync
        /// <summary>
        /// Update the entire panel sync
        /// </summary>
        internal void _UpdateAllSync()
        {
            EnterRender();
            _ClearBuffer(_backColorValue);
            foreach (var visual in _visuals)
                _UpdateSync(visual);
            ExitRender();
        }

        internal void _UpdateSync(PresentationVisual visual)
        {
            visual.Update();
            foreach (var primitive in visual.Context.Primitives)
            {
                if (primitive == null || !_bounds.IsIntersectWith(primitive)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds);
                _UpdateBounds(bounds);
            }
        }

        internal void _ClearVisual(PresentationVisual visual)
        {
            foreach (var primitive in visual.Context.Primitives)
            {
                if (primitive == null || !_bounds.IsIntersectWith(primitive)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds, true);
                _UpdateBounds(bounds);
            }
        }
        #endregion

        #region Transform
        public void Translate(double offsetX, double offsetY, bool toUpdate = true)
        {
            _transform.Translate(offsetX, offsetY);
            if (toUpdate)
                UpdateAll();
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY, bool toUpdate = true)
        {
            _transform.ScaleAt(scaleX, scaleY, centerX, centerY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            if (toUpdate)
                UpdateAll();
        }

        public void Scale(double scaleX, double scaleY, bool toUpdate = true)
        {
            _transform.Scale(scaleX, scaleY);
            _scaleX *= scaleX;
            _scaleY *= scaleY;
            if (toUpdate)
                UpdateAll();
        }
        #endregion

        /// <summary>
        /// If param "isUpdate" is false, You must call <see cref="UpdateAll"/> or <see cref="Update(PresentationVisual)"/> after calling this method,
        /// Make sure the panel is updated!
        /// </summary>
        /// <param name="visual">The visual to add</param>
        /// <param name="isUpdate">Whether to refresh the panel immediately</param>
        public void AddVisual(PresentationVisual visual, bool isUpdate = false)
        {
            Monitor.Enter(_loopLock);
            if (!_visuals.Contains(visual))
            {
                visual.Panel = this;
                if (_isUpdatingAll)
                    _cnt++;
                lock (this)
                    _visuals.Add(visual);

                if (isUpdate)
                    Update(visual, true);
            }
            Monitor.Exit(_loopLock);
        }

        /// <summary>
        /// If param "isUpdate" is false, You must call <see cref="UpdateAll"/> or <see cref="Update(PresentationVisual)"/> after calling this method,
        /// Make sure the panel is updated!
        /// </summary>
        /// <param name="visual">The visual to remove</param>
        /// <param name="isUpdate">Whether to refresh the panel immediately</param>
        public void RemoveVisual(PresentationVisual visual, bool isUpdate = true)
        {
            Monitor.Enter(_loopLock);
            if (_visuals.Contains(visual))
            {
                visual.Panel = null;
                // It has been updated before deletion, so the count is reduced by one here.
                if (_isUpdatingAll && visual.Mode == Mode.Normal)
                    _cnt--;
                else visual.Mode = Mode.Normal;
                lock (this)
                    _visuals.Remove(visual);

                if (isUpdate)
                    ClearVisual(visual);
            }
            Monitor.Exit(_loopLock);
        }

        #region Render
        /// <summary>
        /// Update the entire panel
        /// </summary>
        public void UpdateAll()
        {
            if (_renderMode == RenderMode.Sync)
                _UpdateAllSync();
            else _UpdateAllAsync();
        }

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
            {
                foreach (var _visual in _visuals.Where(other => other != visual && other.IsIntersectWith(visual)))
                {
                    if (_needUpdate) break;
                    _ClearVisual(_visual);
                    _UpdateSync(_visual);
                }
            }
            _UpdateSync(visual);

            ExitRender();
        }

        internal void ClearVisual(PresentationVisual visual)
        {
            if (visual == null) return;
            EnterRender();
            _ClearVisual(visual);

            foreach (var _visual in _visuals.Where(other => other != visual && other.IsIntersectWith(visual)))
                _UpdateSync(_visual);
            ExitRender();
        }

        /// <summary>
        /// Clear panel buffer with the specified color
        /// </summary>
        /// <param name="color">The color to clear</param>
        public void ClearBuffer(byte[] color)
        {
            EnterRender();
            _ClearBuffer(color);
            ExitRender();
        }

        /// <summary>
        /// Clear panel buffer with the specified color(Internal call)
        /// </summary>
        /// <param name="color">The color to clear</param>
        internal void _ClearBuffer(byte[] color)
        {
            var start = _offset;
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

        internal void UpdateBounds(Int32Rect bounds)
        {
            EnterRender();
            _image.AddDirtyRect(bounds);
            ExitRender();
        }

        private void _UpdateBounds(Int32Rect bounds)
        {
            _image.AddDirtyRect(bounds);
        }

        private void _DrawPrimitive(IPrimitive primitive, Int32Rect bounds, bool isClear = false)
        {
            var paths = GeometryHelper.CalcPrimitivePaths(primitive);

            if (isClear)
            {
                foreach (var point in Helper.FilterUniquePoints(paths, _bounds, primitive.Property.Pen.Dashes))
                    _DrawPoint(point, _backColorValue, primitive.Property.Pen.Thickness);

                if (primitive is ICanFilledPrimitive)
                {
                    var canfilled = primitive as ICanFilledPrimitive;
                    if (canfilled.FillColor != null)
                        foreach (var fillP in canfilled.GenFilledRegion(paths).Where(p => bounds.Contains(p)))
                            _DrawPoint(fillP, _backColorValue, 1);
                }
            }
            else
            {
                foreach (var point in Helper.FilterUniquePoints(paths, _bounds, primitive.Property.Pen.Dashes))
                    _DrawPoint(point, primitive.Property.Pen.Color, primitive.Property.Pen.Thickness);

                if (primitive is ICanFilledPrimitive)
                {
                    var canfilled = primitive as ICanFilledPrimitive;
                    if (canfilled.FillColor != null)
                        foreach (var fillP in canfilled.GenFilledRegion(paths).Where(p => bounds.Contains(p)))
                            _DrawPoint(fillP, canfilled.FillColor, 1);
                }
            }
        }

        private void _DrawPoint(Int32Point pos, byte[] color, int thickness = 1)
        {
            var points = GeometryHelper.CalcPositions(pos.X, pos.Y, Offset, Stride, thickness, _bounds);

            foreach (var point in points)
                _DrawPixel(point, color);
        }

        unsafe private void _DrawPixel(IntPtr ptr, byte[] color)
        {
            var _ptr = (byte*)ptr;
            if (color[3] != byte.MaxValue && color != _backColorValue)
            {
                var r = byte.MaxValue - color[3];
                var _color = new byte[4];
                _color[0] = (byte)((color[0] * color[3] + (*_ptr) * r) / byte.MaxValue);
                _color[1] = (byte)((color[1] * color[3] + (*(_ptr + 1)) * r) / byte.MaxValue);
                _color[2] = (byte)((color[2] * color[3] + (*(_ptr + 2)) * r) / byte.MaxValue);
                _color[3] = byte.MaxValue;
                Marshal.Copy(_color, 0, ptr, 4);
            }
            else Marshal.Copy(color, 0, ptr, 4);
        }

        internal byte[] GetColor(Int32 x, Int32 y)
        {
            var start = _offset;
            start += y * Stride;
            start += x * GeometryHelper.PixelByteLength;
            return _GetPixel(start);
        }

        private byte[] _GetPixel(IntPtr ptr)
        {
            var colors = new byte[4];
            Marshal.Copy(ptr, colors, 0, 4);
            return colors;
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
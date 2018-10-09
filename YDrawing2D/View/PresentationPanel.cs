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
        public PresentationPanel(double width, double height, double dpiX, double dpiY, Color backColor, Point origin, RenderMode renderMode)
        {
            _renderMode = renderMode;

            DPIRatio = dpiX / GeometryHelper.SysDPI;
            VisualHelper.HitTestThickness = (int)(VisualHelper.HitTestThickness * DPIRatio);
            VisualHelper.HitTestThickness = Math.Max(1, VisualHelper.HitTestThickness);
            _clipBounds = new RectangleGeometry();

            _image = new WriteableBitmap((int)(width * DPIRatio), (int)(height * DPIRatio), dpiX, dpiY, PixelFormats.Bgra32, null);
            _bounds = new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight);
            _offset = _image.BackBuffer;
            _stride = _image.BackBufferStride;
            _imageHeight = _image.Height;
            _flags = new bool[_image.PixelWidth, _image.PixelHeight];
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            _visuals = new List<PresentationVisual>();
            _transform = new Matrix();

            #region Async
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1) };
            _timer.Tick += _UpdateSample;
            #endregion

            BackColor = backColor;
            _origin = origin;
        }

        public RenderMode RenderMode { get { return _renderMode; } }
        protected RenderMode _renderMode;

        public Point Origin { get { return _origin; } }
        protected Point _origin;

        internal readonly double DPIRatio;

        private RectangleGeometry _clipBounds;
        public Int32Rect Bounds { get { return _bounds; } }
        protected Int32Rect _bounds;

        internal IntPtr Offset { get { return _offset; } }
        private IntPtr _offset;

        internal int Stride { get { return _stride; } }
        private int _stride;

        internal double ImageHeight { get { return _imageHeight; } }
        private double _imageHeight;

        private bool[, ] _flags;

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
                    _backColorValue32 = Helper.ColorToInt32(_backColorValue);
                    UpdateAll();
                }
            }
        }
        protected Color _backColor;

        internal byte[] BackColorValue { get { return _backColorValue; } }
        private byte[] _backColorValue;
        private int _backColorValue32;

        /// <summary>
        /// Internally used bitmap
        /// </summary>
        internal WriteableBitmap Image { get { return _image; } }
        private WriteableBitmap _image;

        public IEnumerable<PresentationVisual> Visuals { get { return _visuals;} }
        protected List<PresentationVisual> _visuals;

        internal Matrix Transform { get { return _transform; } }
        private Matrix _transform;

        public double ScaleX { get { return _scaleX; } }
        protected double _scaleX = 1;

        public double ScaleY { get { return _scaleY; } }
        protected double _scaleY = 1;

        #region Async
        DispatcherTimer _timer;
        bool _isUpdatingAll = false;
        bool _needUpdate = false;
        bool _completeLoop = false;
        int _cnt = 0;
        int _flagSync = 0;
        object _loopLock = new object();
        ParallelOptions _option = new ParallelOptions();
        CancellationTokenSource _currentSource;

        /// <summary>
        /// Update the entire panel async
        /// </summary>
        internal void _UpdateAllAsync()
        {
            Monitor.Enter(_loopLock);
            if (_isUpdatingAll)
            {
                if (_currentSource != null && _currentSource.Token.CanBeCanceled)
                    _currentSource.Cancel();

                _needUpdate = true;
                Monitor.Exit(_loopLock);
                return;
            }
            _isUpdatingAll = true;
            foreach (var visual in _visuals)
                visual.Mode = Mode.WatingForUpdate;
            ClearBuffer();
            _cnt = 0;
            _timer.Start();
            ThreadPool.QueueUserWorkItem(e =>
            {
                _completeLoop = false;

                var visuals = default(List<PresentationVisual>);
                while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                    Thread.Sleep(1);
                visuals = _visuals.ToList();
                Interlocked.Exchange(ref _flagSync, 0);

                _currentSource = new CancellationTokenSource();
                _option.CancellationToken = _currentSource.Token;
                try
                {
                    var ret = Parallel.ForEach(visuals, _option, _UpdateAsync);
                }
                catch (Exception)
                {
                }
                //foreach (var visual in visuals)
                //{
                //    if (_needUpdate) break;
                //    _UpdateAsync(visual);
                //}
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
                    {
                        Thread.Sleep(1);
                        continue;
                    }
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
                if (_currentSource.IsCancellationRequested) break;
                if (!_bounds.IsIntersectWith(primitive)) continue;
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
            _ClearBuffer();
            foreach (var visual in _visuals)
                _UpdateSync(visual);
            ExitRender();
        }

        internal void _UpdateSync(PresentationVisual visual)
        {
            visual.Update();
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!_bounds.IsIntersectWith(primitive)) continue;
                var bounds = GeometryHelper.RestrictBounds(_bounds, primitive.Property.Bounds);
                _DrawPrimitive(primitive, bounds);
                _UpdateBounds(bounds);
            }
        }

        internal void _ClearVisual(PresentationVisual visual, bool optimizate = false)
        {
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!_bounds.IsIntersectWith(primitive)) continue;
                var canFilled = primitive is ICanFilledPrimitive;
                if (optimizate && 
                    (primitive.Property.Pen.Color == null || primitive.Property.Pen.Color[3] == byte.MaxValue) && 
                    (!canFilled || (primitive as ICanFilledPrimitive).FillColor == null || (primitive as ICanFilledPrimitive).FillColor[3] == byte.MaxValue)) continue;
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

                _Add(visual);

                if (isUpdate)
                {
                    Monitor.Exit(_loopLock);
                    Update(visual, true);
                    Monitor.Enter(_loopLock);
                }
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

                _Remove(visual);

                if (isUpdate)
                {
                    Monitor.Exit(_loopLock);
                    ClearVisual(visual);
                    Monitor.Enter(_loopLock);
                }
            }
            Monitor.Exit(_loopLock);
        }

        private void _Add(PresentationVisual visual)
        {
            while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                Thread.Sleep(1);
            _visuals.Add(visual);
            Interlocked.Exchange(ref _flagSync, 0);
        }

        private void _Remove(PresentationVisual visual)
        {
            while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                Thread.Sleep(1);
            _visuals.Remove(visual);
            Interlocked.Exchange(ref _flagSync, 0);
        }

        private void _Clear()
        {
            while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                Thread.Sleep(1);
            _visuals.Clear();
            Interlocked.Exchange(ref _flagSync, 0);
        }

        public void ClearAll()
        {
            _Clear();
            UpdateAll();
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
            Monitor.Enter(_loopLock);
            EnterRender();

            if (isSingle)
            {
                if (visual.Mode == Mode.Normal)
                {
                    _ClearVisual(visual);
                    _UpdateSync(visual);
                }
            }
            else
            {
                var visuals = _visuals.Where(other => other.Mode == Mode.Normal && other.IsIntersectWith(visual));
                foreach (var _visual in visuals)
                    _ClearVisual(_visual, _visual != visual);
                foreach (var _visual in visuals)
                    _UpdateSync(_visual);
            }

            ExitRender();
            Monitor.Exit(_loopLock);
        }

        internal void ClearVisual(PresentationVisual visual)
        {
            if (visual == null) return;
            Monitor.Enter(_loopLock);
            EnterRender();
            _ClearVisual(visual);

            var visuals = _visuals.Where(other => other.Mode == Mode.Normal && other != visual && other.IsIntersectWith(visual));
            foreach (var _visual in visuals)
                _ClearVisual(_visual, true);
            foreach (var _visual in visuals)
                _UpdateSync(_visual);

            ExitRender();
            Monitor.Exit(_loopLock);
        }

        /// <summary>
        /// Clear panel buffer with the specified color
        /// </summary>
        /// <param name="color">The color to clear</param>
        public void ClearBuffer()
        {
            EnterRender();
            _ClearBuffer();
            ExitRender();
        }

        /// <summary>
        /// Clear panel buffer with the specified color(Internal call)
        /// </summary>
        /// <param name="color">The color to clear</param>
        unsafe internal void _ClearBuffer()
        {
            var start = (int*)_offset;
            for (int i = 0; i < _bounds.Height; i++)
            {
                for (int j = 0; j < _bounds.Width; j++)
                {
                    *start = _backColorValue32;
                    start++;
                }
            }
            _UpdateBounds(_bounds);
        }

        /// <summary>
        /// must call this method before do any render method.
        /// </summary>
        internal void EnterRender()
        {
            //Dispatcher.Invoke(() => _image.Lock());
            _image.Lock();
        }

        /// <summary>
        /// must call this method after any render method done.
        /// </summary>
        internal void ExitRender()
        {
            //Dispatcher.Invoke(() => _image.Unlock());
            _image.Unlock();
        }

        internal void UpdateBounds(Int32Rect bounds)
        {
            EnterRender();
            _UpdateBounds(bounds);
            ExitRender();
        }

        private void _UpdateBounds(Int32Rect bounds)
        {
            //Dispatcher.Invoke(() => _image.AddDirtyRect(bounds));
            _image.AddDirtyRect(bounds);
        }

        private void _DrawPrimitive(IPrimitive primitive, Int32Rect bounds, bool isClear = false)
        {
            var paths = default(IEnumerable<PrimitivePath>);
            if (primitive.Property.Pen.Thickness > 0 || ((primitive is ICanFilledPrimitive) && (primitive as ICanFilledPrimitive).FillColor != null))
                paths = GeometryHelper.CalcPrimitivePaths(primitive);

            Array.Clear(_flags, 0, _flags.Length);

            if (isClear)
            {
                if (primitive.Property.Pen.Thickness > 0)
                    foreach (var point in Helper.FilterUniquePoints(paths, bounds, primitive.Property.Pen.Dashes))
                        _DrawPoint(point, _backColorValue, _backColorValue32, primitive.Property.Pen.Thickness);

                if (primitive is ICanFilledPrimitive)
                {
                    var canfilled = primitive as ICanFilledPrimitive;
                    if (canfilled.FillColor != null)
                        foreach (var fillP in canfilled.GenFilledRegion(paths, bounds))
                            _DrawPoint(fillP, _backColorValue, _backColorValue32);
                }
            }
            else
            {
                if (primitive.Property.Pen.Thickness > 0)
                {
                    var colorValue = Helper.ColorToInt32(primitive.Property.Pen.Color);
                    foreach (var point in Helper.FilterUniquePoints(paths, bounds, primitive.Property.Pen.Dashes))
                        _DrawPoint(point, primitive.Property.Pen.Color, colorValue, primitive.Property.Pen.Thickness);
                }

                if (primitive is ICanFilledPrimitive)
                {
                    var canfilled = primitive as ICanFilledPrimitive;
                    if (canfilled.FillColor != null)
                    {
                        var colorValue = Helper.ColorToInt32(canfilled.FillColor);
                        foreach (var fillP in canfilled.GenFilledRegion(paths, bounds))
                            _DrawPoint(fillP, canfilled.FillColor, colorValue);
                    }
                }
            }
        }

        private void _DrawPoint(Int32Point pos, byte[] color, int colorValue, int thickness = 1)
        {
            foreach (var point in GeometryHelper.CalcPositions(pos.X, pos.Y, Offset, Stride, thickness, _bounds, _flags))
                _DrawPixel(point, color, colorValue);
        }

        unsafe private void _DrawPixel(IntPtr ptr, byte[] color, int colorValue)
        {
            var _ptr = (byte*)ptr;
            if (color[3] != byte.MaxValue && color != _backColorValue)
            {
                var r = byte.MaxValue - color[3];
                var _color = 0;
                _color += (color[0] * color[3] + (*_ptr) * r) / byte.MaxValue;
                _color += ((color[1] * color[3] + (*(_ptr + 1)) * r) / byte.MaxValue) << 8;
                _color += ((color[2] * color[3] + (*(_ptr + 2)) * r) / byte.MaxValue) << 16;
                _color += byte.MaxValue << 24;
                *(int*)ptr = _color;
            }
            else *(int*)ptr = colorValue;
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

        /// <summary>
        /// Add some custom logic(Do not participate in transform), such as coordinate system, etc.
        /// </summary>
        public virtual void OnRenderCustom(DrawingContext drawingContext)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            _clipBounds.Rect = new Rect(RenderSize);
            OnRenderCustom(drawingContext);
            if (_image == null) return;
            drawingContext.DrawImage(_image, new Rect(new Point(), new Size(_image.Width, _image.Height)));
        }

        public void Dispose()
        {
            if (_currentSource != null && _currentSource.Token.CanBeCanceled)
                _currentSource.Cancel();
            _currentSource?.Dispose();

            _clipBounds = null;
            _timer.Stop();
            _timer = null;

            //foreach (var visual in _visuals)
            //    visual.Dispose();
            _Clear();
            _visuals = null;

            _image = null;
        }
    }
}
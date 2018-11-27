using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    /// <summary>
    /// Simple polygon
    /// </summary>
    public class _SimpleGeometry : IPrimitive
    {
        public _SimpleGeometry(PenF pen, Color? fillColor, PointF begin, bool isClosed)
        {
            _bounds = RectF.Empty;
            _pen = pen;
            _fillColor = fillColor;
            Begin = begin;
            _isClosed = isClosed;
            UnClosedLine = null;
            _stream = new List<IPrimitive>();
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public PrimitiveType Type { get { return PrimitiveType.SimpleGeometry; } }

        public bool Filled { get { return _fillColor.HasValue; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        public MeshModel Model { get { return null; } set { } }

        public MeshModel FillModel { get { return null; } set { } }

        internal bool IsClosed { get { return _isClosed; } set { _isClosed = value; } }
        private bool _isClosed;

        internal PointF Begin;

        internal _Line UnClosedLine;

        internal IEnumerable<IPrimitive> Stream { get { return _stream; } }
        private List<IPrimitive> _stream;

        internal void StreamTo(IPrimitive primitive)
        {
            if (primitive == null) return;
            _stream.Add(primitive);
        }

        internal void Close()
        {
            if (_stream == null) return;
            foreach (var primitive in _stream)
                _bounds.Union(primitive.Bounds);
        }

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                if (!isOutline)
                    yield return Begin;
                foreach (var primitive in _stream)
                    foreach (var p in primitive[isOutline])
                        yield return p;
                if (!isOutline && UnClosedLine != null)
                    yield return UnClosedLine.End;
            }
        }

        public bool HitTest(PointF p, float sensitive)
        {
            if (!_HitTestOutline(p, sensitive))
                return _HitTestFill(p, sensitive);
            return true;
        }

        public bool HitTest(RectF rect)
        {
            if (!_HitTestOutline(rect))
                return _HitTestFill(rect);
            return true;
        }

        internal bool _HitTestOutline(PointF p, float sensitive)
        {
            if (_pen.IsNULL) return false;
            foreach (var primitive in _stream)
                if (primitive.Bounds.Contains(p, sensitive) && primitive.HitTest(p, sensitive))
                    return true;
            return false;
        }

        internal bool _HitTestFill(PointF p, float sensitive)
        {
            if (!Filled) return false;
            return GeometryHelper.Contains(this, p);
        }

        internal bool _HitTestOutline(RectF rect)
        {
            if (_pen.IsNULL) return false;
            foreach (var primitive in _stream)
                if (primitive.Bounds.IntersectsWith(rect) && primitive.HitTest(rect))
                    return true;
            return false;
        }

        internal bool _HitTestFill(RectF rect)
        {
            if (!Filled) return false;
            return GeometryHelper.Contains(this, rect.TopLeft) || GeometryHelper.Contains(this, rect.BottomLeft)
                   || GeometryHelper.Contains(this, rect.TopRight) || GeometryHelper.Contains(this, rect.BottomRight);
        }

        public void Dispose()
        {
            UnClosedLine = null;
            _stream.Dispose();
            _stream.Clear();
            _stream = null;
        }
    }

    public class _ComplexGeometry : IPrimitive
    {
        public IEnumerable<PointF> this[bool isOutline] { get { yield break; } }

        public RectF Bounds { get { return _bounds; } }
        internal RectF _bounds;

        public PenF Pen { get { return PenF.NULL; } }

        public Color? FillColor { get { return null; } }

        public MeshModel Model { get { return null; } set { } }

        public MeshModel FillModel { get { return _fillModel; } set { _fillModel = value; } }
        private MeshModel _fillModel;

        public bool Filled { get { return _children != null && _children.Any(child => child.Filled); } }

        internal bool _wholeFill;

        public PrimitiveType Type { get { return PrimitiveType.ComplexGeometry; } }

        public IEnumerable<_SimpleGeometry> Children { get { return _children; } }
        private List<_SimpleGeometry> _children;

        public void AddChild(_SimpleGeometry child)
        {
            if (_children == null)
                _children = new List<_SimpleGeometry>();
            _children.Add(child);
        }

        internal void Close()
        {
            if (_children == null) return;
            foreach (var child in _children)
            {
                child.Close();
                _bounds.Union(child.Bounds);
            }
        }

        public bool HitTest(PointF p, float sensitive)
        {
            if (_children == null) return false;

            if (_wholeFill) return true;

            if (!_children.Any(child => child._HitTestOutline(p, sensitive)))
            {
                var cnt = 0;
                _children.ForEach(child => 
                {
                    var ret = child._HitTestFill(p, sensitive);
                    if (ret)
                        cnt++;
                });
                return cnt % 2 == 1;
            }
            return true;
        }

        public bool HitTest(RectF rect)
        {
            if (_children == null) return false;

            if (_wholeFill) return true;

            if (!_children.Any(child => child._HitTestOutline(rect)))
            {
                var flag = false;
                var cnt = 0;
                _children.ForEach(child =>
                {
                    var ret = child._HitTestFill(rect.TopLeft, 0);
                    if (ret)
                        cnt++;
                });
                flag |= cnt % 2 == 1;

                if (!flag)
                {
                    cnt = 0;
                    _children.ForEach(child =>
                    {
                        var ret = child._HitTestFill(rect.BottomLeft, 0);
                        if (ret)
                            cnt++;
                    });
                    flag |= cnt % 2 == 1;
                }

                if (!flag)
                {
                    cnt = 0;
                    _children.ForEach(child =>
                    {
                        var ret = child._HitTestFill(rect.TopRight, 0);
                        if (ret)
                            cnt++;
                    });
                    flag |= cnt % 2 == 1;
                }

                if (!flag)
                {
                    cnt = 0;
                    _children.ForEach(child =>
                    {
                        var ret = child._HitTestFill(rect.BottomRight, 0);
                        if (ret)
                            cnt++;
                    });
                    flag |= cnt % 2 == 1;
                }

                return flag;
            }
            return true;
        }

        public void Dispose()
        {
            _children?.Dispose();
            _children?.Clear();
            _children = null;

            _fillModel = null;
        }
    }
}
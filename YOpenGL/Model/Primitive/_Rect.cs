using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class _Rect : IPrimitive
    {
        public _Rect(RectF rect, PenF pen, Color? color)
        {
            _bounds = rect;
            _pen = pen;
            _fillColor = color;
        }

        public RectF Bounds { get { return _bounds; } }
        private RectF _bounds;

        public PenF Pen { get { return _pen; } }
        private PenF _pen;

        public bool Filled { get { return FillColor.HasValue; } }

        public Color? FillColor { get { return _fillColor; } }
        private Color? _fillColor;

        public MeshModel Model { get { return _model; } set { _model = value; } }
        private MeshModel _model;

        public MeshModel FillModel { get { return _fillModel; } set { _fillModel = value; } }
        private MeshModel _fillModel;

        public PrimitiveType Type { get { return PrimitiveType.Rect; } }

        public IEnumerable<PointF> this[bool isOutline]
        {
            get
            {
                if (!isOutline)
                {
                    yield return _bounds.TopLeft;
                    yield return _bounds.TopRight;
                    yield return _bounds.BottomRight;
                    yield return _bounds.BottomLeft;
                }
                else
                {
                    yield return _bounds.TopLeft;
                    yield return _bounds.TopRight;
                    yield return _bounds.TopRight;
                    yield return _bounds.BottomRight;
                    yield return _bounds.BottomRight;
                    yield return _bounds.BottomLeft;
                    yield return _bounds.BottomLeft;
                    yield return _bounds.TopLeft;
                }
            }
        }

        public bool HitTest(PointF p, float sensitive)
        {
            if (Filled)
                return true;
            return Math.Abs(p._x - _bounds.Left) < sensitive || Math.Abs(p._x - _bounds.Right) < sensitive
                || Math.Abs(p._y - _bounds.Top) < sensitive || Math.Abs(p._y - _bounds.Bottom) < sensitive;
        }

        public void Dispose()
        {
            _model = null;
            _fillModel = null;
        }
    }
}
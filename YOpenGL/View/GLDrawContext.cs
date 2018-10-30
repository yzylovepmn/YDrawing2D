using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL
{
    public class GLDrawContext : IDisposable
    {
        internal GLDrawContext(GLVisual visual)
        {
            _visual = visual;
            _primitives = new List<IPrimitive>();
            _transform = new StackTransform();
        }

        internal GLVisual Visual { get { return _visual; } }
        private GLVisual _visual;

        internal IEnumerable<IPrimitive> Primitives { get { return _primitives; } }
        private List<IPrimitive> _primitives;

        #region Transform
        internal StackTransform Transform { get { return _transform; } }
        private StackTransform _transform;

        public void PushOpacity(float opacity)
        {
            _transform.PushOpacity(opacity);
        }

        public void PushTranslate(float offsetX, float offsetY)
        {
            _transform.PushTranslate(offsetX, offsetY);
        }

        public void PushScale(float scaleX, float scaleY)
        {
            _transform.PushScale(scaleX, scaleY);
        }

        public void PushScaleAt(float scaleX, float scaleY, float centerX, float centerY)
        {
            _transform.PushScaleAt(scaleX, scaleY, centerX, centerY);
        }

        public void PushRotate(float angle)
        {
            _transform.PushRotate(angle);
        }

        public void PushRotateAt(float angle, float centerX, float centerY)
        {
            _transform.PushRotateAt(angle, centerX, centerY);
        }

        public void Pop()
        {
            _transform.Pop();
        }
        #endregion

        #region Draw
        public void DrawLine(PenF pen, PointF start, PointF end)
        {
            _primitives.Add(_DrawLine(pen, start, end));
        }

        private _Line? _DrawLine(PenF pen, PointF start, PointF end)
        {
            if (start == end) return null;
            start = _transform.Transform(start);
            end = _transform.Transform(end);

            return new _Line(start, end, pen);
        }

        public void DrawCicle(PenF pen, Color? fillColor, PointF center, float radius)
        {
            center = _transform.Transform(center);
            radius *= _transform.ScaleX;
            _primitives.Add(new _Arc(center, radius, float.PositiveInfinity, float.PositiveInfinity, pen, fillColor));
        }

        public void DrawArc(PenF pen, PointF center, float radius, float startAngle, float endAngle, bool isClockwise)
        {
            if (startAngle == endAngle)
                return;

            if (!isClockwise)
                MathUtil.Switch(ref startAngle, ref endAngle);

            GeometryHelper.FormatAngle(ref startAngle);
            GeometryHelper.FormatAngle(ref endAngle);

            center = _transform.Transform(center);
            radius *= _transform.ScaleX;

            var startRadian = GeometryHelper.GetRadian(startAngle);
            var endRadian = GeometryHelper.GetRadian(endAngle);

            _primitives.Add(new _Arc(center, radius, startRadian, endRadian, pen, null));
        }

        public void DrawRectangle(PenF pen, Color? fillColor, RectF rectangle)
        {
            rectangle.Transform(_transform.Matrix);
            _primitives.Add(new _Rect(rectangle, pen, fillColor));
        }

        /// <summary>
        /// The points.Count() == degree + 1;
        /// </summary>
        public void DrawBezier(PenF pen, int degree, IEnumerable<PointF> points)
        {
            _primitives.Add(_DrawBezier(pen, degree, points));
        }

        private _Bezier _DrawBezier(PenF pen, int degree, IEnumerable<PointF> points)
        {
            return new _Bezier(points.Select(p => _transform.Transform(p)).ToArray(), degree, pen);
        }

        #region Stream
        private PointF? _begin;
        private PointF? _current;
        private _Geometry _stream;

        public void BeginFigure(PenF pen, Color? fillColor, PointF begin, bool isClosed)
        {
            if (_begin.HasValue)
                throw new InvalidOperationException("Must call EndFigure before call this method!");
            _begin = begin;
            _current = begin;
            _stream = new _Geometry(pen, fillColor, begin, isClosed);
        }

        public void EndFigure()
        {
            if (_begin == null)
                throw new InvalidOperationException("Must call BeginFigure before call this method!");
            if (_stream.IsClosed && _begin != _current)
                LineTo(_begin.Value);
            else _stream.UnClosedLine = _DrawLine(null, _current.Value, _begin.Value);
            _primitives.Add(_stream);
            _begin = null;
            _current = null;
        }

        public void LineTo(PointF point)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _stream.StreamTo(_DrawLine(null, _current.Value, point));
            _current = point;
        }

        public void PolyLineTo(IEnumerable<PointF> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            foreach (var point in points)
                LineTo(point);
        }

        public void BezierTo(int degree, IEnumerable<PointF> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            var _points = new List<PointF>();
            _points.Add(_current.Value);
            _points.AddRange(points);
            _stream.StreamTo(_DrawBezier(null, degree, _points));
            _current = points.Last();
        }

        /// <summary>
        /// The points.Count() must be an integer multiple of degree
        /// </summary>
        public void PolyBezierTo(int degree, IEnumerable<PointF> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            var _points = new List<PointF>();
            foreach (var point in points)
            {
                _points.Add(point);
                if (_points.Count == degree)
                {
                    BezierTo(degree, _points);
                    _points.Clear();
                }
            }
        }

        public void ArcTo(PointF point, float radius, bool isLargeAngle, bool isClockwise)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _ArcTo(point, radius, isLargeAngle, isClockwise);
            _current = point;
        }

        private void _ArcTo(PointF point, float radius, bool isLargeAngle, bool isClockwise)
        {
            if (point == _current.Value)
                return;

            var startP = _transform.Transform(_current.Value);
            var endP = _transform.Transform(point);
            radius *= _transform.ScaleX;

            var vec = endP - startP;
            if (vec.Length <= radius * 2)
            {
                var normal = new VectorF(vec.Y, -vec.X);
                normal.Normalize();
                var center = new PointF((startP.X + endP.X) / 2, (startP.Y + endP.Y) / 2);
                var len = normal * (float)Math.Sqrt(radius * radius - vec.LengthSquared / 4);
                if (isLargeAngle ^ isClockwise)
                    center += len;
                else center -= len;
                if (!isClockwise)
                    MathUtil.Switch(ref startP, ref endP);

                var startRadian = GeometryHelper.GetRadian(center, startP);
                var endRadian = GeometryHelper.GetRadian(center, endP);

                _stream.StreamTo(new _Arc(center, radius, startRadian, endRadian, null));
            }
        }
        #endregion
        #endregion

        private void _Clear()
        {
            _primitives.Dispose();
            _primitives.Clear();
        }

        internal void Reset()
        {
            _Clear();

            _transform.Reset();
        }

        public void Dispose()
        {
            _Clear();
            _transform.Dispose();
            _primitives = null;
            _visual = null;
        }
    }
}
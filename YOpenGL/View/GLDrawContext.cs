using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            pen.Color = _transform.Transform(pen.Color);

            return new _Line(start, end, pen);
        }

        public void DrawCicle(PenF pen, Color? fillColor, PointF center, float radius)
        {
            center = _transform.Transform(center);
            radius *= _transform.ScaleX;

            pen.Color = _transform.Transform(pen.Color);
            if (fillColor.HasValue)
                fillColor = _transform.Transform(fillColor.Value);

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

            pen.Color = _transform.Transform(pen.Color);

            _primitives.Add(new _Arc(center, radius, startRadian, endRadian, pen, null));
        }

        public void DrawRectangle(PenF pen, Color? fillColor, RectF rectangle)
        {
            rectangle.Transform(_transform.Matrix);

            pen.Color = _transform.Transform(pen.Color);
            if (fillColor.HasValue)
                fillColor = _transform.Transform(fillColor.Value);

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
            pen.Color = _transform.Transform(pen.Color);

            return new _Bezier(points.Select(p => _transform.Transform(p)).ToArray(), degree, pen);
        }

        /// <summary>
        /// Non-uniform rational B-spline (NURBS)
        /// </summary>
        public void DrawSpline(PenF pen, int degree, IEnumerable<float> knots, IEnumerable<PointF> controlPoints, IEnumerable<float> weights, IEnumerable<PointF> fitPoints)
        {
            pen.Color = _transform.Transform(pen.Color);

            _primitives.Add(new _Spline(degree, knots.ToArray(), controlPoints.Select(c => _transform.Transform(c)).ToArray(), weights.ToArray(), fitPoints.Select(f => _transform.Transform(f)).ToArray(), pen));
        }

        public void DrawText(PenF pen, Color? fillColor, FormattedText textToDraw, PointF origin)
        {
            pen.Color = _transform.Transform(pen.Color);
            if (fillColor.HasValue)
                fillColor = _transform.Transform(fillColor.Value);

            _DrawGeometry(pen, fillColor, textToDraw.BuildGeometry(new Point(origin.X, -origin.Y - textToDraw.Height)));
        }

        public void DrawGlyphRun(PenF pen, Color? fillColor, GlyphRun glyphRun)
        {
            pen.Color = _transform.Transform(pen.Color);
            if (fillColor.HasValue)
                fillColor = _transform.Transform(fillColor.Value);

            _DrawGeometry(pen, fillColor, glyphRun.BuildGeometry());
        }

        private void _DrawGeometry(PenF pen, Color? fillColor, Geometry geometry)
        {
            if (geometry.IsEmpty()) return;
            foreach (var figure in geometry.GetOutlinedPathGeometry().Figures)
            {
                BeginFigure(pen, fillColor, _Transform(figure.StartPoint), figure.IsClosed);
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment)
                    {
                        var line = (LineSegment)segment;
                        LineTo(_Transform(line.Point));
                    }
                    if (segment is PolyLineSegment)
                    {
                        var line = (PolyLineSegment)segment;
                        PolyLineTo(_Transform(line.Points.ToArray()));
                    }
                    if (segment is BezierSegment)
                    {
                        var bezier = (BezierSegment)segment;
                        BezierTo(3, new List<PointF>() { _Transform(bezier.Point1), _Transform(bezier.Point2), _Transform(bezier.Point3) });
                    }
                    if (segment is QuadraticBezierSegment)
                    {
                        var bezier = (QuadraticBezierSegment)segment;
                        BezierTo(2, new List<PointF>() { _Transform(bezier.Point1), _Transform(bezier.Point2) });
                    }
                    if (segment is PolyBezierSegment)
                    {
                        var bezier = (PolyBezierSegment)segment;
                        PolyBezierTo(3, _Transform(bezier.Points.ToArray()));
                    }
                    if (segment is PolyQuadraticBezierSegment)
                    {
                        var bezier = (PolyQuadraticBezierSegment)segment;
                        PolyBezierTo(2, _Transform(bezier.Points.ToArray()));
                    }
                }
            }
            _EndFigures(true);
        }

        private static IEnumerable<PointF> _Transform(params Point[] points)
        {
            foreach (var p in points)
                yield return _Transform(p);
        }

        private static PointF _Transform(Point p)
        {
            return (PointF)new Point(p.X, -p.Y);
        }

        #region Stream
        private PointF? _begin;
        private PointF? _current;
        private _ComplexGeometry _geo;
        private _SimpleGeometry _subGeo;

        /// <summary>
        /// Start drawing a Figure flow (continuous),must call <see cref="EndFigures"/> to end this Figure or combine Figures(generate by <see cref="BeginFigure(PenF, Color?, PointF, bool)"/>)
        /// </summary>
        public void BeginFigure(PenF pen, Color? fillColor, PointF begin, bool isClosed)
        {
            pen.Color = _transform.Transform(pen.Color);
            if (fillColor.HasValue)
                fillColor = _transform.Transform(fillColor.Value);

            if (_begin.HasValue)
            {
                if (_subGeo.IsClosed && _begin != _current)
                    LineTo(_begin.Value);
                else _subGeo.UnClosedLine = _DrawLine(PenF.NULL, _current.Value, _begin.Value);
            }

            _begin = begin;
            _current = begin;
            _subGeo = new _SimpleGeometry(pen, fillColor, begin, isClosed);
            _geo.AddChild(_subGeo);
        }

        /// <summary>
        /// Combine the previous Figures into one complex geometry stream
        /// </summary>
        public void EndFigures()
        {
            _EndFigures();
        }

        private void _EndFigures(bool flag = false)
        {
            if (_begin == null)
                throw new InvalidOperationException("Must call BeginFigure before call this method!");

            if (_subGeo.IsClosed && _begin != _current)
                LineTo(_begin.Value);
            else _subGeo.UnClosedLine = _DrawLine(PenF.NULL, _current.Value, _begin.Value);

            _geo.Close();
            _primitives.Add(_geo);

            _geo = new _ComplexGeometry() { _wholeFill = flag };
            _begin = null;
            _current = null;
        }

        public void LineTo(PointF point)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _subGeo.StreamTo(_DrawLine(PenF.NULL, _current.Value, point));
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
            _subGeo.StreamTo(_DrawBezier(PenF.NULL, degree, _points));
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

                _subGeo.StreamTo(new _Arc(center, radius, startRadian, endRadian, PenF.NULL));
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
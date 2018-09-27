using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Model;
using YDrawing2D.Util;

namespace YDrawing2D.View
{
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Specify the starting point of the geometrystream
        /// </summary>
        void BeginFigure(Color? fillColor, DrawingPen pen, Point begin, bool isClosed);

        /// <summary>
        /// End the current geometry stream
        /// </summary>
        void EndFigure();

        void DrawLine(DrawingPen pen, Point start, Point end);
        void DrawCicle(Color? fillColor, DrawingPen pen, Point center, double radius);
        void DrawArc(DrawingPen pen, Point center, double radius, double startAngle, double endAngle, bool isClockwise);
        void DrawEllipse(Color? fillColor, DrawingPen pen, Point center, double radiusX, double radiusY);
        void DrawRectangle(Color? fillColor, DrawingPen pen, Rect rectangle);

        /// <summary>
        /// Non-uniform rational B-spline (NURBS)
        /// </summary>
        void DrawSpline(DrawingPen pen, int degree, IEnumerable<double> knots, IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<Point> fitPoints);

        /// <summary>
        /// The points.Count() == degree + 1;
        /// </summary>
        void DrawBezier(DrawingPen pen, int degree, IEnumerable<Point> points);


        void LineTo(Point point);
        void PolyLineTo(IEnumerable<Point> points);
        void ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise);

        /// <summary>
        /// The points.Count() == degree;
        /// The last point of points is end point, the other is the control point
        /// </summary>
        void BezierTo(int degree, IEnumerable<Point> points);
        /// <summary>
        /// The points.Count() must be an integer multiple of degree
        /// </summary>
        void PolyBezierTo(int degree, IEnumerable<Point> points);


        void PushOpacity(double opacity);
        void PushTranslate(double offsetX, double offsetY);
        void PushScale(double scaleX, double scaleY);
        void PushScaleAt(double scaleX, double scaleY, double centerX, double centerY);
        void PushRotate(double angle);
        void PushRotateAt(double angle, double centerX, double centerY);
        void Pop();
    }

    public class PresentationContext : IContext
    {
        internal PresentationContext(PresentationVisual visual)
        {
            _primitives = new List<IPrimitive>();
            _visual = visual;
            _transform = new StackTransform();
        }

        private PresentationVisual _visual;

        #region Stream
        private Point? _begin;
        private Point? _current;
        private CustomGeometry _stream = CustomGeometry.Empty;
        #endregion

        internal IEnumerable<IPrimitive> Primitives { get { return _primitives.ToList(); } }
        private List<IPrimitive> _primitives;

        public void DrawLine(DrawingPen pen, Point start, Point end)
        {
            _primitives.Add(_DrawLine(new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), start, end));
        }

        private Line _DrawLine(_DrawingPen pen, Point start, Point end)
        {
            start = GeometryHelper.ConvertWithTransform(start, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            end = GeometryHelper.ConvertWithTransform(end, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);

            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);

            return new Line(_start, _end, pen);
        }

        public void DrawCicle(Color? fillColor, DrawingPen pen, Point center, double radius)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Cicle(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, _center, _radius, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawEllipse(Color? fillColor, DrawingPen pen, Point center, double radiusX, double radiusY)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radiusX *= _visual.Panel.ScaleX * _transform.ScaleX;
            radiusY *= _visual.Panel.ScaleY * _transform.ScaleY;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radiusX = Helper.ConvertTo(radiusX * _visual.Panel.DPIRatio);
            var _radiusY = Helper.ConvertTo(radiusY * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Ellipse(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, _center, _radiusX, _radiusY, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawArc(DrawingPen pen, Point center, double radius, double startAngle, double endAngle, bool isClockwise)
        {
            _DrawArc(pen, center, radius, GeometryHelper.GetRadian(startAngle), GeometryHelper.GetRadian(endAngle), isClockwise);
        }

        private void _DrawArc(DrawingPen pen, Point center, double radius, double startRadian, double endRadian, bool isClockwise)
        {
            if (!isClockwise)
                Helper.Switch(ref startRadian, ref endRadian);

            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var startP = new Point(radius * Math.Cos(startRadian) + center.X, center.Y - radius * Math.Sin(startRadian));
            var endP = new Point(radius * Math.Cos(endRadian) + center.X, center.Y - radius * Math.Sin(endRadian));

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _startP = GeometryHelper.ConvertToInt32Point(startP, _visual.Panel.DPIRatio);
            var _endP = GeometryHelper.ConvertToInt32Point(endP, _visual.Panel.DPIRatio);
            var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Arc(_startP, _endP, _center, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawRectangle(Color? fillColor, DrawingPen pen, Rect rectangle)
        {
            BeginFigure(fillColor, pen, rectangle.Location, true);
            LineTo(rectangle.TopRight);
            LineTo(rectangle.BottomRight);
            LineTo(rectangle.BottomLeft);
            EndFigure();
        }

        public void DrawSpline(DrawingPen pen, int degree, IEnumerable<double> knots, IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<Point> fitPoints)
        {
            _primitives.Add(new Spline(degree, knots.ToArray(), controlPoints?.Select(ctrlP => GeometryHelper.ConvertWithTransform(ctrlP, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform)).ToArray(), weights.ToArray(), fitPoints?.Select(fitP => GeometryHelper.ConvertWithTransform(fitP, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform)).ToArray(), new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), _visual.Panel.DPIRatio));
        }

        public void DrawBezier(DrawingPen pen, int degree, IEnumerable<Point> points)
        {
            _DrawBezier(new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), degree, points);
        }

        private void _DrawBezier(_DrawingPen pen, int degree, IEnumerable<Point> points, bool isStream = false)
        {
            if (points.Count() != degree + 1) throw new ArgumentException("points.Count() != degree + 1");
            var bezier = new Bezier(points.Select(p => GeometryHelper.ConvertWithTransform(p, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform)).ToArray(), degree, pen, _visual.Panel.DPIRatio);
            if (!isStream)
                _primitives.Add(bezier);
            else _stream.StreamTo(bezier);
        }

        public void LineTo(Point point)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _stream.StreamTo(_DrawLine(_stream.Property.Pen, _current.Value, point));
            _current = point;
        }

        public void PolyLineTo(IEnumerable<Point> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            foreach (var point in points)
                LineTo(point);
        }

        public void ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _ArcTo(point, radius, isLargeAngle, isClockwise);
            _current = point;
        }

        private void _ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise)
        {
            var startP = GeometryHelper.ConvertWithTransform(_current.Value, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            var endP = GeometryHelper.ConvertWithTransform(point, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var vec = endP - startP;
            if (vec.Length <= (radius * 2))
            {
                var normal = new Vector(vec.Y, -vec.X);
                normal.Normalize();
                var center = new Point((startP.X + endP.X) / 2, (startP.Y + endP.Y) / 2);
                var len = normal * Math.Sqrt(radius * radius - vec.LengthSquared / 4);
                if (isLargeAngle ^ isClockwise)
                    center -= len;
                else center += len;
                if (!isClockwise)
                    Helper.Switch(ref startP, ref endP);

                var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
                var _startP = GeometryHelper.ConvertToInt32Point(startP, _visual.Panel.DPIRatio);
                var _endP = GeometryHelper.ConvertToInt32Point(endP, _visual.Panel.DPIRatio);

                _stream.StreamTo(new Arc(_startP, _endP, _center, _stream.Property.Pen));
            }
        }

        public void BezierTo(int degree, IEnumerable<Point> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            var _points = new List<Point>();
            _points.Add(_current.Value);
            _points.AddRange(points);
            _DrawBezier(_stream.Property.Pen, degree, _points, true);
            _current = points.Last();
        }

        public void PolyBezierTo(int degree, IEnumerable<Point> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            var _points = new List<Point>();
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

        public void BeginFigure(Color? fillColor, DrawingPen pen, Point begin, bool isClosed)
        {
            _begin = begin;
            _current = begin;
            _stream = new CustomGeometry(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), isClosed);
        }

        public void EndFigure()
        {
            if (_begin == null)
                throw new InvalidOperationException("Must call BeginFigure before call this method!");
            if (_stream.IsClosed && _begin != _current)
                LineTo(_begin.Value);
            else _stream.UnClosedLine = _DrawLine(_stream.Property.Pen, _current.Value, _begin.Value);
            _primitives.Add(_stream);
            _begin = null;
            _current = null;
        }

        internal void Reset()
        {
            _transform.Reset();
            _begin = null;
            _current = null;
            _primitives.Clear();
        }

        #region Transform
        internal StackTransform Transform { get { return _transform; } }
        private StackTransform _transform;

        public void PushOpacity(double opacity)
        {
            _transform.PushOpacity(opacity);
        }

        public void PushTranslate(double offsetX, double offsetY)
        {
            _transform.PushTranslate(offsetX, offsetY);
        }

        public void PushScale(double scaleX, double scaleY)
        {
            _transform.PushScale(scaleX, scaleY);
        }

        public void PushScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            _transform.PushScaleAt(scaleX, scaleY, centerX, centerY);
        }

        public void PushRotate(double angle)
        {
            _transform.PushRotate(angle);
        }

        public void PushRotateAt(double angle, double centerX, double centerY)
        {
            _transform.PushRotateAt(angle, centerX, centerY);
        }

        public void Pop()
        {
            _transform.Pop();
        }
        #endregion

        public void Dispose()
        {
            _transform.Dispose();
            _primitives.Clear();
            _primitives = null;
            _visual = null;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YDrawing2D.Model;
using YDrawing2D.Util;

namespace YDrawing2D.View
{
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Specify the starting point of the geometry stream
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

        void DrawText(Color? foreground, DrawingPen pen, string textToDraw, Typeface typeface, double emSize, Point origin);

        void DrawGlyphRun(Color? foreground, DrawingPen pen, GlyphRun glyphRun);

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

        internal PresentationVisual Visual { get { return _visual; } }
        private PresentationVisual _visual;

        internal bool NeedFilpCoordinate { get { return _needFilpCoordinate; } }
        private bool _needFilpCoordinate = true;

        private int _flagSync = 0;

        #region Stream
        private Point? _begin;
        private Point? _current;
        private CustomGeometry _stream = CustomGeometry.Empty;
        #endregion

        internal IEnumerable<IPrimitive> Primitives
        {
            get
            {
                while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                    Thread.Sleep(1);
                var primitives = _primitives.ToList();
                Interlocked.Exchange(ref _flagSync, 0);
                return primitives;
            }
        }
        private List<IPrimitive> _primitives;

        #region Sync
        private void _Add(IPrimitive primitive)
        {
            while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                Thread.Sleep(1);
            _primitives.Add(primitive);
            Interlocked.Exchange(ref _flagSync, 0);
        }

        private void _Clear()
        {
            while (Interlocked.Exchange(ref _flagSync, 1) == 1)
                Thread.Sleep(1);
            _primitives.Clear();
            Interlocked.Exchange(ref _flagSync, 0);
        }
        #endregion

        public void DrawLine(DrawingPen pen, Point start, Point end)
        {
            _Add(_DrawLine(new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), start, end));
        }

        private Line _DrawLine(_DrawingPen pen, Point start, Point end)
        {
            start = GeometryHelper.ConvertWithTransform(start, this);
            end = GeometryHelper.ConvertWithTransform(end, this);

            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);

            return new Line(_start, _end, pen);
        }

        public void DrawCicle(Color? fillColor, DrawingPen pen, Point center, double radius)
        {
            center = GeometryHelper.ConvertWithTransform(center, this);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _Add(new Cicle(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, _center, _radius, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawEllipse(Color? fillColor, DrawingPen pen, Point center, double radiusX, double radiusY)
        {
            center = GeometryHelper.ConvertWithTransform(center, this);
            radiusX *= _visual.Panel.ScaleX * _transform.ScaleX;
            radiusY *= _visual.Panel.ScaleY * _transform.ScaleY;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radiusX = Helper.ConvertTo(radiusX * _visual.Panel.DPIRatio);
            var _radiusY = Helper.ConvertTo(radiusY * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _Add(new Ellipse(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, _center, _radiusX, _radiusY, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawArc(DrawingPen pen, Point center, double radius, double startAngle, double endAngle, bool isClockwise)
        {
            _DrawArc(pen, center, radius, GeometryHelper.GetRadian(startAngle), GeometryHelper.GetRadian(endAngle), isClockwise);
        }

        private void _DrawArc(DrawingPen pen, Point center, double radius, double startRadian, double endRadian, bool isClockwise)
        {
            if (!isClockwise)
                Helper.Switch(ref startRadian, ref endRadian);

            center = GeometryHelper.ConvertWithTransform(center, this);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var startP = new Point(radius * Math.Cos(startRadian) + center.X, center.Y - radius * Math.Sin(startRadian));
            var endP = new Point(radius * Math.Cos(endRadian) + center.X, center.Y - radius * Math.Sin(endRadian));

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _startP = GeometryHelper.ConvertToInt32Point(startP, _visual.Panel.DPIRatio);
            var _endP = GeometryHelper.ConvertToInt32Point(endP, _visual.Panel.DPIRatio);
            var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _Add(new Arc(_startP, _endP, _center, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawRectangle(Color? fillColor, DrawingPen pen, Rect rectangle)
        {
            BeginFigure(fillColor, pen, rectangle.Location, true);
            _stream.Shape = Shape.Rect;
            LineTo(rectangle.TopRight);
            LineTo(rectangle.BottomRight);
            LineTo(rectangle.BottomLeft);
            EndFigure();
        }

        public void DrawSpline(DrawingPen pen, int degree, IEnumerable<double> knots, IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<Point> fitPoints)
        {
            _Add(new Spline(degree, knots.ToArray(), controlPoints?.Select(ctrlP => GeometryHelper.ConvertWithTransform(ctrlP, this)).ToArray(), weights.ToArray(), fitPoints?.Select(fitP => GeometryHelper.ConvertWithTransform(fitP, this)).ToArray(), new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), _visual.Panel.DPIRatio));
        }

        public void DrawBezier(DrawingPen pen, int degree, IEnumerable<Point> points)
        {
            _DrawBezier(new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), degree, points);
        }

        private void _DrawBezier(_DrawingPen pen, int degree, IEnumerable<Point> points, bool isStream = false)
        {
            if (points.Count() != degree + 1) throw new ArgumentException("points.Count() != degree + 1");
            var bezier = new Bezier(points.Select(p => GeometryHelper.ConvertWithTransform(p, this)).ToArray(), degree, pen, _visual.Panel.DPIRatio);
            if (!isStream)
                _Add(bezier);
            else _stream.StreamTo(bezier);
        }

        public void DrawText(Color? fillColor, DrawingPen pen, string textToDraw, Typeface typeface, double emSize, Point origin)
        {
            _needFilpCoordinate = false;
            origin = new Point(origin.X, _visual.Panel.ImageHeight - origin.Y);
            var textFormat = new FormattedText(textToDraw, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize, null);
            for (int i = 0; i < textToDraw.Length; i++)
            {
                var charFormat = new FormattedText(textToDraw[i].ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize, Brushes.White);
                _DrawPathGeometry(fillColor, pen, charFormat.BuildGeometry(textFormat.BuildHighlightGeometry(origin, i, 1).Bounds.TopLeft));
            }
            _needFilpCoordinate = true;
        }

        public void DrawGlyphRun(Color? foreground, DrawingPen pen, GlyphRun glyphRun)
        {
            _needFilpCoordinate = false;
            _DrawPathGeometry(foreground, pen, glyphRun.BuildGeometry());
            _needFilpCoordinate = true;
        }

        private void _DrawPathGeometry(Color? foreground, DrawingPen pen, Geometry geometry)
        {
            if (geometry.IsEmpty()) return;
            foreach (var figure in geometry.GetFlattenedPathGeometry().Figures)
            {
                BeginFigure(foreground, pen, figure.StartPoint, figure.IsClosed);
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment)
                    {
                        var line = (LineSegment)segment;
                        LineTo(line.Point);
                    }
                    if (segment is PolyLineSegment)
                    {
                        var line = (PolyLineSegment)segment;
                        PolyLineTo(line.Points);
                    }
                    if (segment is BezierSegment)
                    {
                        var bezier = (BezierSegment)segment;
                        BezierTo(3, new List<Point>() { bezier.Point1, bezier.Point2, bezier.Point3 });
                    }
                    if (segment is QuadraticBezierSegment)
                    {
                        var bezier = (QuadraticBezierSegment)segment;
                        BezierTo(2, new List<Point>() { bezier.Point1, bezier.Point2 });
                    }
                    if (segment is PolyBezierSegment)
                    {
                        var bezier = (PolyBezierSegment)segment;
                        PolyBezierTo(3, bezier.Points);
                    }
                    if (segment is PolyQuadraticBezierSegment)
                    {
                        var bezier = (PolyQuadraticBezierSegment)segment;
                        PolyBezierTo(2, bezier.Points);
                    }
                }
            }
            EndFigure();
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
            var startP = GeometryHelper.ConvertWithTransform(_current.Value, this);
            var endP = GeometryHelper.ConvertWithTransform(point, this);
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
            if (_begin == null)
            {
                _begin = begin;
                _current = begin;
                _stream = new CustomGeometry(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null, pen.Thickness < 0 ? _DrawingPen.EMPTY : new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()), isClosed);
            }
            else
            {
                if (_stream.IsClosed && _begin != _current)
                    LineTo(_begin.Value);
                _stream.IsClosed = isClosed;
                _stream.SetPen(pen.Thickness < 0 ? _DrawingPen.EMPTY : new _DrawingPen(Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio), Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray()));
                _stream.SetFillColor(fillColor.HasValue ? Helper.CalcColor(fillColor.Value, _transform.Opacity) : null);
                _begin = begin;
                _current = begin;
            }
        }

        public void EndFigure()
        {
            if (_begin == null)
                throw new InvalidOperationException("Must call BeginFigure before call this method!");
            if (_stream.IsClosed && _begin != _current)
                LineTo(_begin.Value);
            else _stream.UnClosedLine = _DrawLine(_stream.Property.Pen, _current.Value, _begin.Value);
            _Add(_stream);
            _begin = null;
            _current = null;
        }

        internal void Reset()
        {
            _Clear();

            _transform.Reset();
            _begin = null;
            _current = null;
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
            _Clear();

            _transform.Dispose();
            _primitives = null;
            _visual = null;
        }
    }
}
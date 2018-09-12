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
    public interface IContext
    {
        /// <summary>
        /// Specify the starting point of the stream
        /// </summary>
        void BeginFigure(Point begin);
        void DrawLine(DrawingPen pen, Point start, Point end);
        void DrawCicle(DrawingPen pen, Point center, double radius);
        void DrawEllipse(DrawingPen pen, Point center, double radiusX, double radiusY);
        void DrawArc(DrawingPen pen, Point center, double radius, double startAngle, double endAngle, bool isClockwise);
        void LineTo(DrawingPen pen, Point point);
        void PolyLineTo(DrawingPen pen, IEnumerable<Point> points);
        void ArcTo(DrawingPen pen, Point point, double radius, bool isLargeAngle, bool isClockwise);
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

        private Point? _begin;
        private Point? _current;
        private PresentationVisual _visual;

        internal IEnumerable<IPrimitive> Primitives { get { return new List<IPrimitive>(_primitives); } }
        private List<IPrimitive> _primitives;

        public void DrawLine(DrawingPen pen, Point start, Point end)
        {
            start = GeometryHelper.ConvertWithTransform(start, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            end = GeometryHelper.ConvertWithTransform(end, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);

            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Line(_start, _end, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawCicle(DrawingPen pen, Point center, double radius)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radius *= _visual.Panel.ScaleX * _transform.ScaleX;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Cicle(_center, _radius, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawEllipse(DrawingPen pen, Point center, double radiusX, double radiusY)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform, _transform);
            radiusX *= _visual.Panel.ScaleX * _transform.ScaleX;
            radiusY *= _visual.Panel.ScaleY * _transform.ScaleY;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radiusX = Helper.ConvertTo(radiusX * _visual.Panel.DPIRatio);
            var _radiusY = Helper.ConvertTo(radiusY * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Ellipse(_center, _radiusX, _radiusY, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
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

        public void LineTo(DrawingPen pen, Point point)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            DrawLine(pen, _current.Value, point);
            _current = point;
        }

        public void PolyLineTo(DrawingPen pen, IEnumerable<Point> points)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            foreach (var point in points)
                LineTo(pen, point);
        }

        public void ArcTo(DrawingPen pen, Point point, double radius, bool isLargeAngle, bool isClockwise)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _ArcTo(pen, point, radius, isLargeAngle, isClockwise);
            _current = point;
        }

        private void _ArcTo(DrawingPen pen, Point point, double radius, bool isLargeAngle, bool isClockwise)
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
                var _radius = Helper.ConvertTo(radius * _visual.Panel.DPIRatio);
                var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

                _primitives.Add(new Arc(_startP, _endP, _center, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color, _transform.Opacity), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
            }
        }

        public void BeginFigure(Point begin)
        {
            _begin = begin;
            _current = begin;
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
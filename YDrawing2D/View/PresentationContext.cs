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
        void DrawLine(Point start, Point end, DrawingPen pen);
        void DrawCicle(Point center, double radius, DrawingPen pen);
        void DrawArc(Point center, double radius, double startAngle, double endAngle, bool isClockwise, DrawingPen pen);
        void LineTo(Point point, DrawingPen pen);
        void ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise, DrawingPen pen);
    }

    public class PresentationContext : IContext
    {
        internal PresentationContext(PresentationVisual visual)
        {
            _primitives = new List<IPrimitive>();
            _visual = visual;
        }

        private Point? _begin;
        private PresentationVisual _visual;

        internal IEnumerable<IPrimitive> Primitives { get { return _primitives; } }
        private List<IPrimitive> _primitives;

        public void DrawLine(Point start, Point end, DrawingPen pen)
        {
            start = GeometryHelper.ConvertWithTransform(start, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            end = GeometryHelper.ConvertWithTransform(end, _visual.Panel.ImageHeight, _visual.Panel.Transform);

            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Line(_start, _end, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawCicle(Point center, double radius, DrawingPen pen)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            radius *= _visual.Panel.ScaleX;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radius = (int)(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Cicle(_center, _radius, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void DrawArc(Point center, double radius, double startAngle, double endAngle, bool isClockwise, DrawingPen pen)
        {
            _DrawArc(center, radius, GeometryHelper.GetRadian(startAngle), GeometryHelper.GetRadian(endAngle), isClockwise, pen);
        }

        private void _DrawArc(Point center, double radius, double startRadian, double endRadian, bool isClockwise, DrawingPen pen)
        {
            if (!isClockwise)
                Helper.Switch(ref startRadian, ref endRadian);

            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            radius *= _visual.Panel.ScaleX;

            var startP = new Point(radius * Math.Cos(startRadian) + center.X, center.Y - radius * Math.Sin(startRadian));
            var endP = new Point(radius * Math.Cos(endRadian) + center.X, center.Y - radius * Math.Sin(endRadian));

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _startP = GeometryHelper.ConvertToInt32Point(startP, _visual.Panel.DPIRatio);
            var _endP = GeometryHelper.ConvertToInt32Point(endP, _visual.Panel.DPIRatio);
            var _radius = (int)(radius * _visual.Panel.DPIRatio);
            var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Arc(_startP, _endP, _center, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
        }

        public void LineTo(Point point, DrawingPen pen)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            DrawLine(_begin.Value, point, pen);
            _begin = point;
        }

        public void ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise, DrawingPen pen)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            _ArcTo(point, radius, isLargeAngle, isClockwise, pen);
            _begin = point;
        }

        private void _ArcTo(Point point, double radius, bool isLargeAngle, bool isClockwise, DrawingPen pen)
        {
            var startP = GeometryHelper.ConvertWithTransform(_begin.Value, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            var endP = GeometryHelper.ConvertWithTransform(point, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            radius *= _visual.Panel.ScaleX;
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
                var _radius = (int)(radius * _visual.Panel.DPIRatio);
                var _thickness = Helper.ConvertTo(pen.Thickness * _visual.Panel.DPIRatio);

                _primitives.Add(new Arc(_startP, _endP, _center, new _DrawingPen(_thickness, Helper.CalcColor(pen.Color), pen.Dashes == null ? null : Helper.ConvertTo(pen.Dashes).ToArray())));
            }
        }

        public void BeginFigure(Point begin)
        {
            _begin = begin;
        }

        internal void Reset()
        {
            _begin = null;
            _primitives.Clear();
        }

        public void Dispose()
        {
            _primitives.Clear();
            _primitives = null;
            _visual = null;
        }
    }
}
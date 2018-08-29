using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YDrawing2D.Model;

namespace YDrawing2D.View
{
    public interface IContext
    {
        void BeginFigure(Point begin);
        void DrawLine(Point start, Point end, double thickness, Color color);
        void LineTo(Point point, double thickness, Color color);
        void DrawCicle(Point center, double radius, double thickness, Color color);
        void DrawArc(Point center, double radius, double startAngle, double endAngle, bool isClockwise, double thickness, Color color);
    }

    public class PresentationContext : IContext
    {
        public PresentationContext(PresentationVisual visual)
        {
            _primitives = new List<IPrimitive>();
            _visual = visual;
        }

        private Point? _begin;
        private PresentationVisual _visual;

        internal IEnumerable<IPrimitive> Primitives { get { return _primitives; } }
        private List<IPrimitive> _primitives;

        public void DrawLine(Point start, Point end, double thickness, Color color)
        {
            start = GeometryHelper.ConvertWithTransform(start, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            end = GeometryHelper.ConvertWithTransform(end, _visual.Panel.ImageHeight, _visual.Panel.Transform);

            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);
            var _thickness = (int)(thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Line(Math.Max(_thickness, 1), Helper.CalcColor(color), _start, _end));
        }

        public void DrawCicle(Point center, double radius, double thickness, Color color)
        {
            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            radius *= _visual.Panel.ScaleX;

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _radius = (int)(radius * _visual.Panel.DPIRatio);
            var _thickness = (int)(thickness * _visual.Panel.DPIRatio);
            _primitives.Add(new Cicle(Math.Max(_thickness, 1), Helper.CalcColor(color), _center, _radius));
        }

        public void DrawArc(Point center, double radius, double startAngle, double endAngle, bool isClockwise, double thickness, Color color)
        {
            if (!isClockwise)
                Helper.Switch(ref startAngle, ref endAngle);

            center = GeometryHelper.ConvertWithTransform(center, _visual.Panel.ImageHeight, _visual.Panel.Transform);
            radius *= _visual.Panel.ScaleX;

            var radian = GeometryHelper.GetRadian(startAngle);
            var startP = new Point(radius * Math.Cos(radian) + center.X, center.Y - radius * Math.Sin(radian));
            radian = GeometryHelper.GetRadian(endAngle);
            var endP = new Point(radius * Math.Cos(radian) + center.X, center.Y - radius * Math.Sin(radian));

            var _center = GeometryHelper.ConvertToInt32Point(center, _visual.Panel.DPIRatio);
            var _startP = GeometryHelper.ConvertToInt32Point(startP, _visual.Panel.DPIRatio);
            var _endP = GeometryHelper.ConvertToInt32Point(endP, _visual.Panel.DPIRatio);
            var _radius = (int)(radius * _visual.Panel.DPIRatio);
            var _thickness = (int)(thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Arc(_thickness, Helper.CalcColor(color), _startP, _endP, _center));
        }

        public void LineTo(Point point, double thickness, Color color)
        {
            if (!_begin.HasValue) throw new InvalidOperationException("must be figure begin point!");
            DrawLine(_begin.Value, point, thickness, color);
            _begin = point;
        }

        public void BeginFigure(Point begin)
        {
            _begin = begin;
        }

        internal void Reset()
        {
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
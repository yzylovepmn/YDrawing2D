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
            start = GeometryHelper.ConvertToWPFSystem(start, _visual.Panel.Image.Height);
            end = GeometryHelper.ConvertToWPFSystem(end, _visual.Panel.Image.Height);
            var _start = GeometryHelper.ConvertToInt32Point(start, _visual.Panel.DPIRatio);
            var _end = GeometryHelper.ConvertToInt32Point(end, _visual.Panel.DPIRatio);
            var _thickness = (int)(thickness * _visual.Panel.DPIRatio);

            _primitives.Add(new Line(_thickness, Helper.CalcColor(color), _start, _end));
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
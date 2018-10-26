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

        private _Line _DrawLine(PenF pen, PointF start, PointF end)
        {
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YRenderingSystem._3D
{
    public class RectSelector : IDisposable
    {
        public RectSelector(GLPanel3D viewport)
        {
            _viewport = viewport;
            _fill = new RectFill();
            _wireframe = new RectWireframe();
            _selectorVisual = new GLVisual3D() { IsHitTestVisible = false };
            var group = new GLModel3DGroup();
            group.AddChild(_wireframe);
            group.AddChild(_fill);
            _selectorVisual.Model = group;
        }

        private GLPanel3D _viewport;
        private GLVisual3D _selectorVisual;
        private RectFill _fill;
        private RectWireframe _wireframe;

        public Color Color { get { return _fill.Material.Color; } set { _fill.Material.Color = value; } }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    if (_isVisible)
                        _viewport.AddVisual(_selectorVisual);
                    else _viewport.RemoveVisual(_selectorVisual);
                }
            }
        }
        private bool _isVisible;

        public PointF P1
        {
            get { return _p1; }
            set
            {
                if (_p1 != value)
                {
                    _p1 = value;
                    _UpdateData();
                }
            }
        }
        private PointF _p1;

        public PointF P2
        {
            get { return _p2; }
            set
            {
                if (_p2 != value)
                {
                    _p2 = value;
                    _UpdateData();
                }
            }
        }
        private PointF _p2;

        private void _UpdateData()
        {
            var xmin = Math.Min(_p1.X, _p2.X);
            var xmax = Math.Max(_p1.X, _p2.X);
            var ymin = Math.Min(_p1.Y, _p2.Y);
            var ymax = Math.Max(_p1.Y, _p2.Y);

            var zDepth = _viewport.Camera.Type == CameraType.Orthographic ? -0.999f : -0f;
            var bottomLeft = _viewport.PointInWpfToPoint3D(new PointF(xmin, ymin), zDepth);
            var bottomRight = _viewport.PointInWpfToPoint3D(new PointF(xmax, ymin), zDepth);
            var topLeft = _viewport.PointInWpfToPoint3D(new PointF(xmin, ymax), zDepth);
            var topRight = _viewport.PointInWpfToPoint3D(new PointF(xmax, ymax), zDepth);

            if (bottomLeft.HasValue)
            {
                var points = new List<Point3F>() { bottomRight.Value, bottomLeft.Value, topLeft.Value, topRight.Value };
                _fill.SetPoints(points);
                points.Add(points.First());
                _wireframe.SetPoints(points);
                if (_p1.X > _p2.X)
                    _wireframe.Dashes = new byte[] { 1, 1 };
                else _wireframe.Dashes = null;

                if (_isVisible)
                    _viewport.Refresh();
            }
        }

        public void Dispose()
        {
            _viewport = null;
        }

        public class RectFill : GLMeshModel3D
        {
            public RectFill()
            {
                Mode = GLPrimitiveMode.GL_TRIANGLE_FAN;
                _emissive = new EmissiveMaterial();
                AddMaterial(_emissive, MaterialOption.Front);
            }

            public Material Material { get { return _emissive; } }
            private EmissiveMaterial _emissive;
        }

        public class RectWireframe : GLMeshModel3D
        {
            public RectWireframe()
            {
                Mode = GLPrimitiveMode.GL_LINE_STRIP;
                LineWidth = 0.5f;
                AddMaterial(new EmissiveMaterial(), MaterialOption.Front);
            }
        }
    }
}
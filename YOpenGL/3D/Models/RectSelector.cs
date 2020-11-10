using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public class RectSelector : IDisposable
    {
        public RectSelector(GLPanel3D viewport)
        {
            _viewport = viewport;
            _fill = new RectFill();
            _wireframe = new RectWireframe();
        }

        private GLPanel3D _viewport;
        private RectFill _fill;
        private RectWireframe _wireframe;

        public Color Color { get { return _fill.Material.Color; } set { _fill.Material.Color = value; } }

        internal IEnumerable<GLModel3D> Models
        {
            get
            {
                yield return _fill;
                yield return _wireframe;
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    if (_isVisible)
                        _viewport.AddModels(Models);
                    else _viewport.RemoveModels(Models);
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

            var zDepth = _viewport.Camera.Type == CameraType.Orthographic ? -1 : 0f;
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

        public class RectFill : GLModel3D
        {
            public RectFill()
            {
                IsHitTestVisible = false;
                IsVolumeObject = false;
                Mode = GLPrimitiveMode.GL_TRIANGLE_FAN;
                _emissive = new EmissiveMaterial();
                AddMaterial(_emissive, MaterialOption.Front);
            }

            public Material Material { get { return _emissive; } }
            private EmissiveMaterial _emissive;
        }

        public class RectWireframe : GLModel3D
        {
            public RectWireframe()
            {
                IsHitTestVisible = false;
                IsVolumeObject = false;
                Mode = GLPrimitiveMode.GL_LINE_STRIP;
                AddMaterial(new EmissiveMaterial(), MaterialOption.Front);
            }
        }
    }
}
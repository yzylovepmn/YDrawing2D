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
        private const float _ZDpeth = 0f;

        public RectSelector(GLPanel3D viewport)
        {
            _viewport = viewport;
            _viewport.Camera.PropertyChanged += _OnPropertyChanged;
            _UpdateTransform();
            _fill = new RectFill();
            _wireframe = new RectWireframe();
        }

        private GLPanel3D _viewport;
        private RectFill _fill;
        private RectWireframe _wireframe;
        private Matrix3F _transform;
        private Point3F _topLeft;
        private Point3F _topRight;
        private Point3F _bottomLeft;
        private Point3F _bottomRight;

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

        private void _OnPropertyChanged(object sender, EventArgs e)
        {
            _UpdateTransform();
        }

        private void _UpdateData()
        {
            var xmin = Math.Min(_p1.X, _p2.X);
            var xmax = Math.Max(_p1.X, _p2.X);
            var ymin = Math.Min(_p1.Y, _p2.Y);
            var ymax = Math.Max(_p1.Y, _p2.Y);

            _bottomLeft = new Point3F(xmin, ymin, _ZDpeth) * _transform;
            _bottomRight = new Point3F(xmax, ymin, _ZDpeth) * _transform;
            _topLeft = new Point3F(xmin, ymax, _ZDpeth) * _transform;
            _topRight = new Point3F(xmax, ymax, _ZDpeth) * _transform;

            var points = new List<Point3F>() { _bottomRight, _bottomLeft, _topLeft, _topRight };
            _fill.SetPoints(points);
            _wireframe.SetPoints(points);

            if (_isVisible)
                _viewport.Refresh();
        }

        private void _UpdateTransform()
        {
            if (!_viewport.IsInit) return;
            _transform = _viewport.Camera.GetTotalTransform();
            _transform.Append(_viewport.GetNDCToWPF());
            _transform.Invert();
        }

        public void Dispose()
        {
            _viewport.Camera.PropertyChanged -= _OnPropertyChanged;
            _viewport = null;
        }

        public class RectFill : GLModel3D
        {
            public RectFill()
            {
                IsHitTestVisible = false;
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
                Mode = GLPrimitiveMode.GL_LINE_LOOP;
                AddMaterial(new EmissiveMaterial(), MaterialOption.Front);
            }
        }
    }
}
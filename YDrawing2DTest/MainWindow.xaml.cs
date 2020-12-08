using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YDrawing2D;
using YDrawing2D.Util;
using YDrawing2D.View;
using YOpenGL;
using YOpenGL._3D;

namespace YDrawing2DTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _glPanel = new GLPanel(new PointF(0, 0), Colors.Black, 60, YOpenGL.RenderMode.Async, ResourceMode.Normal);
            _glPanel3D = new GLPanel3D(Colors.Black);

            GD_2D.Children.Add(_glPanel);
            GD_3D.Children.Add(_glPanel3D);

            _InitModel();

            WindowState = WindowState.Normal;
        }

        private void _InitModel()
        {
            var model3D = new GLMeshModel3D() { Mode = GLPrimitiveMode.GL_TRIANGLES };
            model3D.SetPoints(new List<Point3F>()
            {
                new Point3F(0, 0, 0), new Point3F(100, 0, 0), new Point3F(0, 100, 0), new Point3F(100, 100, 0),
                new Point3F(0, 0, 0), new Point3F(0, 0, 100), new Point3F(0, 100, 0), new Point3F(0, 100, 100),
                new Point3F(0, 0, 0), new Point3F(100, 0, 0), new Point3F(0, 0, 100), new Point3F(100, 0, 100),
                new Point3F(100, 0, 0), new Point3F(100, 100, 100), new Point3F(100, 0, 100), new Point3F(100, 100, 0),
                new Point3F(0, 0, 100), new Point3F(100, 0, 100), new Point3F(0, 100, 100), new Point3F(100, 100, 100),
                new Point3F(0, 100, 0), new Point3F(0, 100, 100), new Point3F(100, 100, 0), new Point3F(100, 100, 100),
            });
            model3D.SetNormals(new List<Vector3F>()
            {
                new Vector3F(0, 0, -1), new Vector3F(0, 0, -1), new Vector3F(0, 0, -1), new Vector3F(0, 0, -1),
                new Vector3F(-1, 0, 0), new Vector3F(-1, 0, 0), new Vector3F(-1, 0, 0), new Vector3F(-1, 0, 0),
                new Vector3F(0, -1, 0), new Vector3F(0, -1, 0), new Vector3F(0, -1, 0), new Vector3F(0, -1, 0),
                new Vector3F(1, 0, 0), new Vector3F(1, 0, 0), new Vector3F(1, 0, 0), new Vector3F(1, 0, 0),
                new Vector3F(0, 0, 1), new Vector3F(0, 0, 1), new Vector3F(0, 0, 1), new Vector3F(0, 0, 1),
                new Vector3F(0, 1, 0), new Vector3F(0, 1, 0), new Vector3F(0, 1, 0), new Vector3F(0, 1, 0),
            });
            model3D.SetIndices(new List<uint>()
            {
                0, 2, 1, 1, 2, 3,
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 9, 11, 10,
                12, 13, 14, 12, 15, 13,
                16, 17, 18, 19, 18, 17,
                20, 21, 22, 22, 21, 23,
            });

            //_model3D.AddMaterial(new EmissiveMaterial() { Color = Colors.Gray }, MaterialOption.Both);
            model3D.AddMaterial(new DiffuseMaterial() { Color = Colors.White }, MaterialOption.Both);
            model3D.AddMaterial(new SpecularMaterial() { Color = Colors.White }, MaterialOption.Both);
            _visual3D = new GLVisual3D();
            _visual3D.Model = model3D;
            _glPanel3D.AddVisual(_visual3D);
            _glPanel3D.AddLight(new AmbientLight(Colors.White));
            //_glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(-1, -1, -1)));
            _pointLight = new PointLight(Colors.White, new Point3F(70, -40, 200));
            _glPanel3D.AddLight(_pointLight);
            //_glPanel3D.AddLight(new SpotLight(Colors.White, new Point3F(50, 50, 150), new Vector3F(0, 0, -1)) { InnerConeAngle = 20, OuterConeAngle = 80 });
        }

        private static GLPanel _glPanel;
        private static GLPanel3D _glPanel3D;
        private static GLVisual3D _visual3D;
        //private static GLModel3D_Old _model3D;
        private static PointLight _pointLight;

        public static DrawingPen WhitePen = new DrawingPen(1, Colors.White);
        public static DrawingPen ActivePen = new DrawingPen(1, Colors.Yellow);
        public static DrawingPen SelectedPen = new DrawingPen(1, Colors.Blue);

        public static PenF GLWhitePen = new PenF(1, Colors.White);
        public static PenF GLActivePen = new PenF(1, Colors.DarkRed);
        public static PenF GLSelectedPen = new PenF(1, Colors.Blue);
        private static PresentationPanel _panel;
        public static PresentationVisual ActiveVisual
        {
            get { return _visual; }
            set
            {
                if (_visual != value)
                {
                    var old = _visual;
                    _visual = value;
                    if (old != null)
                        _panel.Update(old);
                    if (_visual != null)
                        _panel.Update(_visual);
                    //_panel.UpdateAll();
                }
            }
        }
        private static PresentationVisual _visual;

        public static GLVisual GLActiveVisual
        {
            get { return _glvisual; }
            set
            {
                if (_glvisual != value)
                {
                    var old = _glvisual;
                    _glvisual = value;
                    if (old != null)
                        _glPanel.Update(old);
                    if (_glvisual != null)
                        _glPanel.Update(_glvisual);
                    _glPanel.Refresh();
                }
            }
        }
        private static GLVisual _glvisual;

        public static PresentationVisual SelectedVisual
        {
            get { return _selectedVisual; }
            set
            {
                if (_selectedVisual != value)
                {
                    var old = _selectedVisual;
                    _selectedVisual = value;
                    if (old != null)
                        _panel.Update(old);
                    if (_selectedVisual != null)
                        _panel.Update(_selectedVisual);
                    //_panel.UpdateAll();
                }
            }
        }
        private static PresentationVisual _selectedVisual;

        public static GLVisual GLSelectedVisual
        {
            get { return _glselectedVisual; }
            set
            {
                if (_glselectedVisual != value)
                {
                    var old = _glselectedVisual;
                    _glselectedVisual = value;
                    if (old != null)
                        _glPanel.Update(old);
                    if (_glselectedVisual != null)
                        _glPanel.Update(_glselectedVisual);
                    _glPanel.Refresh();
                }
            }
        }
        private static GLVisual _glselectedVisual;

        private Hint _hint;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _glPanel3D.MouseDown += _glPanel3D_MouseDown;
            _glPanel3D.MouseMove += _glPanel3D_MouseMove;
            _glPanel3D.Camera.PropertyChanged += Camera_PropertyChanged;

            _glPanel.AddVisual(new Text("Hello world!", new PointF(200, 0)));
            _hint = new Hint() { HitTestVisible = false };
            _glPanel.AddVisual(_hint);
            _glPanel.MouseMove += _panel_MouseMove;
            _glPanel.MouseWheel += _panel_MouseWheel;
            _glPanel.MouseLeftButtonDown += _panel_MouseLeftButtonDown;
            _glPanel.MouseLeftButtonUp += _glPanel_MouseLeftButtonUp;
            _glPanel.UpdateAll();
        }

        private void Camera_PropertyChanged(object sender, EventArgs e)
        {
            _pointLight.Position = _glPanel3D.Camera.Position;
        }

        private void _glPanel3D_MouseMove(object sender, MouseEventArgs e)
        {
            var moveP = _glPanel3D.GetPosition();
            if (_glPanel3D.Selector.IsVisible)
            {
                _glPanel3D.Selector.P2 = moveP;
                _glPanel3D.Selector.Color = _glPanel3D.Selector.P1.X > _glPanel3D.Selector.P2.X ? new Color() { ScA = 0.1f, ScG = 1f } : new Color() { ScA = 0.1f, ScB = 1f };
            }
        }

        private void _glPanel3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var downP = _glPanel3D.GetPosition();
            var retts = _glPanel3D.HitTest(downP);
            if (!_glPanel3D.Selector.IsVisible)
            {
                _glPanel3D.Selector.P1 = downP;
                _glPanel3D.Selector.P2 = downP;
                _glPanel3D.Selector.IsVisible = true;
            }
            else
            {
                _glPanel3D.Selector.IsVisible = false;
                var rets = _glPanel3D.HitTest(new RectF(_glPanel3D.Selector.P1, _glPanel3D.Selector.P2), _glPanel3D.Selector.P1.X < _glPanel3D.Selector.P2.X ? RectHitTestMode.FullContain : RectHitTestMode.Intersect);
            }
        }

        private void _panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _glPanel.CaptureMouse();
            _pInView = _glPanel.ToView((PointF)e.GetPosition(_glPanel));
            _hint.IsVisible = true;
            _hint.Rect = new RectF(_pInView, _pInView);
            if (Keyboard.Modifiers == ModifierKeys.None)
                GLSelectedVisual = _glPanel.HitTest(_pInView);
        }

        private void _glPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _glPanel.ReleaseMouseCapture();
            _hint.IsVisible = false;
        }

        private void _panel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pInView = _glPanel.ToView((PointF)e.GetPosition(_glPanel));
            if (e.Delta > 0)
                _glPanel.ScaleAt(1.1f, 1.1f, pInView.X, pInView.Y);
            else _glPanel.ScaleAt(1 / 1.1f, 1 / 1.1f, pInView.X, pInView.Y);
        }

        PointF _pInView;
        PointF _mouseMovePoint;
        bool isfirst = true;
        private void _panel_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseP = (PointF)e.GetPosition(_glPanel);
            var pInView = _glPanel.ToView(mouseP);
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                //var visual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
                //_panel.RemoveVisual(visual);
                if (e.LeftButton == MouseButtonState.Released)
                    GLActiveVisual = _glPanel.HitTest(pInView);
                else
                {
                    var rect = new RectF(pInView, _pInView);
                    var ret = _glPanel.HitTest(rect);
                    _hint.Rect = rect;
                    if (ret.Count() > 0)
                        GLActiveVisual = ret.First();
                    else GLActiveVisual = null;
                }
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.LeftButton == MouseButtonState.Pressed)
            {
                if (isfirst)
                    isfirst = false;
                else _glPanel.Translate((mouseP.X - _mouseMovePoint.X) / _glPanel.ScaleX, -(mouseP.Y - _mouseMovePoint.Y) / _glPanel.ScaleY);
            }
            _mouseMovePoint = mouseP;
        }
    }

    public class Hint : GLVisual
    {
        public RectF Rect { get { return _rect; } set { _rect = value; _panel.Update(this, true); } }
        private RectF _rect;

        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; _panel.Update(this, true); } }
        private bool _isVisible;

        protected override void Draw(GLDrawContext context)
        {
            if (_isVisible)
                context.DrawRectangle(MainWindow.GLSelectedPen, new Color() { A = 0x10, B = 0xf0 }, _rect);
        }
    }

    public class Text : GLVisual
    {
        public Text(string textToDraw, PointF origin)
        {
            _text = textToDraw;
            _origin = origin;
        }

        private PointF _origin;
        private string _text;

        protected override void Draw(GLDrawContext context)
        {
            context.PushTranslate(_origin.X, _origin.Y);
            context.PushRotateAt(30, 200, 200);
            context.PushOpacity(0.5f);

            var typeFace = new Typeface(new FontFamily("新宋体"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.DrawText(PenF.NULL, Colors.Red, new FormattedText(_text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
            else if (this == MainWindow.GLSelectedVisual)
                context.DrawText(PenF.NULL, Colors.Blue, new FormattedText(_text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
            else context.DrawText(PenF.NULL, Colors.BurlyWood, new FormattedText(_text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
        }
    }

    public class CustomShape : GLVisual
    {
        public CustomShape(PointF origin)
        {
            _origin = origin;
        }

        private PointF _origin;

        protected override void Draw(GLDrawContext context)
        {
            context.PushTranslate(_origin.X, _origin.Y);
            //context.PushRotateAt(30, 0, 0);

            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLActivePen, Colors.Blue, new PointF(100, 100), true);
            else if (this == MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLSelectedPen, Colors.Red, new PointF(100, 100), true);
            else context.BeginFigure(MainWindow.GLWhitePen, Colors.Green, new PointF(100, 100), true);
            context.LineTo(new PointF(600, 100), true);
            context.LineTo(new PointF(600, 600), true);
            context.LineTo(new PointF(100, 600), true);
            //context.ArcTo(new PointF(200, 500), 100, false, true);
            context.BezierTo(2, new List<PointF>() { new PointF(300, 400), new PointF(300, 200) }, true);

            // Start new figure
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLActivePen, Colors.Blue, new PointF(200, 200), true);
            else if (this == MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLSelectedPen, Colors.Red, new PointF(200, 200), true);
            else context.BeginFigure(MainWindow.GLWhitePen, Colors.Green, new PointF(200, 200), true);
            context.LineTo(new PointF(400, 300), true);
            context.LineTo(new PointF(600, 200), true);
            context.LineTo(new PointF(500, 400), true);
            context.LineTo(new PointF(700, 600), true);
            context.LineTo(new PointF(400, 600), true);
            context.LineTo(new PointF(200, 400), true);
            // End two figures
            context.EndFigures();
        }
    }

    public class Rectangle : GLVisual
    {
        public Rectangle(RectF rect)
        {
            _rect = rect;
        }

        public RectF Rect { get { return _rect; } }
        private RectF _rect;

        protected override void Draw(GLDrawContext context)
        {
            Color color = Colors.Green;
            PenF pen = MainWindow.GLWhitePen;
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
            {
                color = Colors.Blue;
                pen = MainWindow.GLActivePen;
            }
            else if (this == MainWindow.GLSelectedVisual)
            {
                color = new Color() { R = 255, A = 64 };
                pen = MainWindow.GLSelectedPen;
            }

            context.DrawRectangle(pen, color, _rect);
            context.DrawPoint(color, _rect.BottomLeft, 20);
            context.DrawPoint(color, _rect.BottomRight, 160);
            context.DrawPoint(color, _rect.TopLeft, 160);
            context.DrawPoint(color, _rect.TopRight, 160);
        }
    }

    public class Line : GLVisual
    {
        public Line(PointF start, PointF end)
        {
            _start = start;
            _end = end;
        }

        private PointF _start;
        private PointF _end;


        protected override void Draw(GLDrawContext context)
        {
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.DrawLine(MainWindow.GLActivePen, _start, _end);
            else if (this == MainWindow.GLSelectedVisual)
                context.DrawLine(MainWindow.GLSelectedPen, _start, _end);
            else context.DrawLine(MainWindow.GLWhitePen, _start, _end);
        }
    }

    public class Cicle : GLVisual
    {
        public Cicle(PointF center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        private PointF _center;
        private float _radius;

        protected override void Draw(GLDrawContext context)
        {
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.DrawCicle(MainWindow.GLActivePen, Colors.Blue, _center, _radius);
            else if (this == MainWindow.GLSelectedVisual)
                context.DrawCicle(MainWindow.GLSelectedPen, Colors.Red, _center, _radius);
            else context.DrawCicle(MainWindow.GLWhitePen, null, _center, _radius);
        }
    }

    public class Ellipse : PresentationVisual
    {
        public Ellipse(Point center, double radiusX, double radiusY)
        {
            _center = center;
            _radiusX = radiusX;
            _radiusY = radiusY;
        }

        private Point _center;
        private double _radiusX;
        private double _radiusY;

        protected override void Draw(IContext context)
        {
            //context.PushOpacity(0.5);
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.DrawEllipse(null, MainWindow.ActivePen, _center, _radiusX, _radiusY);
            else if (this == MainWindow.SelectedVisual)
                context.DrawEllipse(null, MainWindow.SelectedPen, _center, _radiusX, _radiusY);
            else context.DrawEllipse(null, MainWindow.WhitePen, _center, _radiusX, _radiusY);
        }
    }

    public class Arc : GLVisual
    {
        public Arc(PointF center, float startAngle, float endAngle, float radius, bool isClockwise = true)
        {
            _center = center;
            _radius = radius;
            _startAngle = startAngle;
            _endAngle = endAngle;
            _isClockwise = isClockwise;
        }

        public Arc(PointF start, PointF end, float radius, bool isClockwise, bool isLargeAngle)
        {
            _start = start;
            _end = end;
            _radius = radius;
            _isClockwise = isClockwise;
            _isLargeAngle = isLargeAngle;
        }

        private PointF? _start;
        private PointF? _end;
        private PointF _center;
        private float _radius;
        private float _startAngle;
        private float _endAngle;
        private bool _isClockwise;
        private bool _isLargeAngle;

        protected override void Draw(GLDrawContext context)
        {
            if (!_start.HasValue)
            {
                if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                    context.DrawArc(MainWindow.GLActivePen, _center, _radius, _startAngle, _endAngle, _isClockwise);
                else if (this == MainWindow.GLSelectedVisual)
                    context.DrawArc(MainWindow.GLSelectedPen, _center, _radius, _startAngle, _endAngle, _isClockwise);
                else context.DrawArc(MainWindow.GLWhitePen, _center, _radius, _startAngle, _endAngle, _isClockwise);
            }
        }
    }
}
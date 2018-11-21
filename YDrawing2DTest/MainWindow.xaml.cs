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
            _glPanel = new GLPanel(new PointF(0, 0), Colors.Black);
            Content = _glPanel;
        }

        private static GLPanel _glPanel;

        public static DrawingPen WhitePen = new DrawingPen(1, Colors.White);
        public static DrawingPen ActivePen = new DrawingPen(1, Colors.Yellow);
        public static DrawingPen SelectedPen = new DrawingPen(1, Colors.Blue);

        public static PenF GLWhitePen = new PenF(1, Colors.White);
        public static PenF GLActivePen = new PenF(1, Colors.Yellow);
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 100000; i++)
            {
                //_glPanel.AddVisual(new Cicle(new PointF(0, 0), i * 2));
                _glPanel.AddVisual(new Line(new PointF(0, i), new PointF(800, i + 100)));
            }
            //_glPanel.AddVisual(new CustomShape(new PointF(0, 0)));
            //_glPanel.AddVisual(new Text(new PointF(100, 200)));
            //_glPanel.AddVisual(new Cicle(new PointF(500, 500), 200));
            //_glPanel.AddVisual(new Cicle(new PointF(100, 500), 100));
            //_glPanel.AddVisual(new Arc(new PointF(550, 100), 10, 30, 100));
            //_glPanel.AddVisual(new Rectangle(new RectF((SizeF)_glPanel.RenderSize)));
            _glPanel.MouseMove += _panel_MouseMove;
            _glPanel.MouseWheel += _panel_MouseWheel;
            _glPanel.MouseLeftButtonDown += _panel_MouseLeftButtonDown;
            _glPanel.UpdateAll();
            //_panel = new PresentationPanel(ActualWidth, ActualHeight, 96, 96, Colors.Black, new Point(0, 0), YDrawing2D.RenderMode.Async);
            //Content = _panel;
            //var r = new Random(5);
            //for (int i = 0; i < 10000; i++)
            //{
            //    //_panel.AddVisual(new Line(new Point(0, i), new Point(800, i + 100)));
            //    //_panel.AddVisual(new Line(new Point(200, 800 - i), new Point(600, 800 - i)));
            //    //_panel.AddVisual(new Cicle(new Point(400, 400), i));
            //    //_panel.AddVisual(new Ellipse(new Point(400, 400), 20 + i, 40 + 2 * i));
            //    //_panel.AddVisual(new Arc(new Point(400, 400), i, i * 2, 50 + i));
            //}
            //_panel.AddVisual(new Line(new Point(0, 0), new Point(500, 800)), true);
            //_panel.AddVisual(new Arc(new Point(600, 500), 30, 300, 200), true);
            //_panel.AddVisual(new Rectangle(new Rect(new Point(100, 100), new Point(600, 500))), true);
            //_panel.AddVisual(new Ellipse(new Point(400, 100), 200, 400), true);
            //_panel.AddVisual(new Cicle(new Point(100, 300), 300), true);
            //_panel.AddVisual(new CustomShape(), true);
            //_panel.AddVisual(new Text(), true);
            ////_panel.UpdateAll();
            //_panel.MouseMove += _panel_MouseMove;
            //_panel.MouseWheel += _panel_MouseWheel;
            //_panel.MouseLeftButtonDown += _panel_MouseLeftButtonDown;
        }

        private void _panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p = _glPanel.GetMousePosition(e);
            if (Keyboard.Modifiers == ModifierKeys.None)
                GLSelectedVisual = _glPanel.HitTest(p);
        }

        private void _panel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var p = _glPanel.GetMousePosition(e);
            if (e.Delta > 0)
                _glPanel.ScaleAt(1.1f, 1.1f, p.X, p.Y);
            else _glPanel.ScaleAt(1 / 1.1f, 1 / 1.1f, p.X, p.Y);
        }

        PointF p;
        bool isfirst = true;
        private void _panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                //var visual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
                //_panel.RemoveVisual(visual);
                var point = _glPanel.GetMousePosition(e);
                GLActiveVisual = _glPanel.HitTest(point);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.LeftButton == MouseButtonState.Pressed)
            {
                if (isfirst)
                {
                    isfirst = false;
                    p = _glPanel.GetMousePosition(e);
                }
                else
                {
                    var _p = _glPanel.GetMousePosition(e);
                    _glPanel.Translate(_p.X - p.X, _p.Y - p.Y);
                    p = _p;
                }
            }
        }
    }

    public class Text : GLVisual
    {
        public Text(PointF origin)
        {
            _origin = origin;
        }

        private PointF _origin;

        protected override void Draw(GLDrawContext context)
        {
            context.PushTranslate(_origin.X, _origin.Y);
            context.PushRotateAt(30, 200, 200);
            context.PushOpacity(0.5f);

            var typeFace = new Typeface(new FontFamily("新宋体"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var text = "Hello world!";
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.DrawText(PenF.NULL, Colors.Red, new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
            else if (this == MainWindow.GLSelectedVisual)
                context.DrawText(PenF.NULL, Colors.Blue, new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
            else context.DrawText(PenF.NULL, Colors.BurlyWood, new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, 80, Brushes.Black), new PointF());
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

            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLActivePen, Colors.Blue, new PointF(100, 100), true);
            else if (this == MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLSelectedPen, Colors.Red, new PointF(100, 100), true);
            else context.BeginFigure(MainWindow.GLWhitePen, Colors.Green, new PointF(100, 100), true);
            context.LineTo(new PointF(600, 100));
            context.LineTo(new PointF(600, 600));
            context.LineTo(new PointF(100, 600));
            //context.ArcTo(new PointF(200, 200), 400, true, true);
            context.BezierTo(2, new List<PointF>() { new PointF(300, 400), new PointF(300, 200) });

            // Start new figure
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLActivePen, Colors.Blue, new PointF(200, 200), true);
            else if (this == MainWindow.GLSelectedVisual)
                context.BeginFigure(MainWindow.GLSelectedPen, Colors.Red, new PointF(200, 200), true);
            else context.BeginFigure(MainWindow.GLWhitePen, Colors.Green, new PointF(200, 200), true);
            context.LineTo(new PointF(400, 300));
            context.LineTo(new PointF(600, 200));
            context.LineTo(new PointF(500, 400));
            context.LineTo(new PointF(700, 600));
            context.LineTo(new PointF(400, 600));
            context.LineTo(new PointF(200, 400));
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
            if (this == MainWindow.GLActiveVisual && this != MainWindow.GLSelectedVisual)
                context.DrawRectangle(MainWindow.GLActivePen, Colors.Blue, _rect);
            else if (this == MainWindow.GLSelectedVisual)
                context.DrawRectangle(MainWindow.GLSelectedPen, new Color() { R = 255, A = 64 }, _rect);
            else context.DrawRectangle(MainWindow.GLWhitePen, Colors.Green, _rect);
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
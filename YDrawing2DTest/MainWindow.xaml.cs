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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YDrawing2D;
using YDrawing2D.Util;
using YDrawing2D.View;

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
        }
        public static DrawingPen WhitePen = new DrawingPen(1, Colors.White);
        public static DrawingPen ActivePen = new DrawingPen(1, Colors.Yellow);
        public static DrawingPen SelectedPen = new DrawingPen(1, Colors.Blue);
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _panel = new PresentationPanel(ActualWidth, ActualHeight, 96, 96, Colors.Black, new Point(0, 0), RenderMode.Async);
            Content = _panel;
            var r = new Random(5);
            for (int i = 0; i < 10000; i++)
            {
                //_panel.AddVisual(new Line(new Point(0, i), new Point(800, i + 100)));
                //_panel.AddVisual(new Line(new Point(200, 800 - i), new Point(600, 800 - i)));
                //_panel.AddVisual(new Cicle(new Point(400, 400), i));
                //_panel.AddVisual(new Ellipse(new Point(400, 400), 20 + i, 40 + 2 * i));
                //_panel.AddVisual(new Arc(new Point(400, 400), i, i * 2, 50 + i));
            }
            _panel.AddVisual(new Line(new Point(0, 0), new Point(500, 800)), true);
            _panel.AddVisual(new Arc(new Point(600, 500), 30, 300, 200), true);
            _panel.AddVisual(new Rectangle(new Rect(new Point(100, 100), new Point(600, 500))), true);
            _panel.AddVisual(new Ellipse(new Point(400, 100), 200, 400), true);
            _panel.AddVisual(new Cicle(new Point(100, 300), 300), true);
            _panel.AddVisual(new CustomShape(), true);
            _panel.AddVisual(new Text(), true);
            //_panel.UpdateAll();
            _panel.MouseMove += _panel_MouseMove;
            _panel.MouseWheel += _panel_MouseWheel;
            _panel.MouseLeftButtonDown += _panel_MouseLeftButtonDown;
        }

        private void _panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p = e.GetPosition(_panel);
            if (Keyboard.Modifiers == ModifierKeys.None)
                SelectedVisual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
        }

        private void _panel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var p = e.GetPosition(_panel);
            if (e.Delta > 0)
                _panel.ScaleAt(1.1, 1.1, p.X, p.Y);
            else _panel.ScaleAt(1 / 1.1, 1 / 1.1, p.X, p.Y);
        }

        Point p;
        bool isfirst = true;
        private void _panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                //var visual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
                //_panel.RemoveVisual(visual);
                ActiveVisual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.LeftButton == MouseButtonState.Pressed)
            {
                if (isfirst)
                {
                    isfirst = false;
                    p = e.GetPosition(_panel);
                }
                else
                {
                    var _p = e.GetPosition(_panel);
                    _panel.Translate(_p.X - p.X, _p.Y - p.Y);
                    p = _p;
                }
            }
        }
    }

    public class Text : PresentationVisual
    {
        protected override void Draw(IContext context)
        {
            var typeFace = new Typeface(new FontFamily("新宋体"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var text = "Hello World!";
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.DrawText(null, MainWindow.ActivePen, text, typeFace, 80, new Point(300, 300));
            else if (this == MainWindow.SelectedVisual)
                context.DrawText(null, MainWindow.SelectedPen, text, typeFace, 80, new Point(300, 300));
            else context.DrawText(null, MainWindow.WhitePen, text, typeFace, 80, new Point(300, 300));
        }
    }

    public class CustomShape : PresentationVisual
    {
        protected override void Draw(IContext context)
        {
            context.PushOpacity(0.5);
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.BeginFigure(null, MainWindow.ActivePen, new Point(600, 400), true);
            else if (this == MainWindow.SelectedVisual)
                context.BeginFigure(null, MainWindow.SelectedPen, new Point(600, 400), true);
            else context.BeginFigure(null, MainWindow.WhitePen, new Point(600, 400), true);

            //context.LineTo(new Point(700, 400));
            //context.LineTo(new Point(750, 480));
            //context.LineTo(new Point(800, 400));
            //context.LineTo(new Point(900, 400));
            //context.LineTo(new Point(820, 350));
            //
            //context.ArcTo(new Point(600, 480), 180, false, false);

            context.BezierTo(2, new List<Point>() { new Point(500, 300), new Point(300, 700) });

            context.EndFigure();
        }
    }

    public class Rectangle : PresentationVisual
    {
        public Rectangle(Rect rect)
        {
            _rect = rect;
        }

        public Rect Rect { get { return _rect; } }
        private Rect _rect;

        protected override void Draw(IContext context)
        {
            context.PushOpacity(0.5);
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.DrawRectangle(null, MainWindow.ActivePen, _rect);
            else if (this == MainWindow.SelectedVisual)
                context.DrawRectangle(null, MainWindow.SelectedPen, _rect);
            else context.DrawRectangle(null, MainWindow.WhitePen, _rect);
        }
    }

    public class Line : PresentationVisual
    {
        public Line(Point start, Point end)
        {
            _start = start;
            _end = end;
        }

        private Point _start;
        private Point _end;

        protected override void Draw(IContext context)
        {
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.DrawLine(MainWindow.ActivePen, _start, _end);
            else if(this == MainWindow.SelectedVisual)
                context.DrawLine(MainWindow.SelectedPen, _start, _end);
            else context.DrawLine(MainWindow.WhitePen, _start, _end);
        }
    }

    public class Cicle : PresentationVisual
    {
        public Cicle(Point center, double radius)
        {
            _center = center;
            _radius = radius;
        }

        private Point _center;
        private double _radius;

        protected override void Draw(IContext context)
        {
            context.PushOpacity(0.5);
            if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                context.DrawCicle(null, MainWindow.ActivePen, _center, _radius);
            else if (this == MainWindow.SelectedVisual)
                context.DrawCicle(null, MainWindow.SelectedPen, _center, _radius);
            else context.DrawCicle(null, MainWindow.WhitePen, _center, _radius);
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

    public class Arc : PresentationVisual
    {
        public Arc(Point center, double startAngle, double endAngle, double radius, bool isClockwise = true)
        {
            _center = center;
            _radius = radius;
            _startAngle = startAngle;
            _endAngle = endAngle;
            _isClockwise = isClockwise;
        }

        public Arc(Point start, Point end, double radius, bool isClockwise, bool isLargeAngle)
        {
            _start = start;
            _end = end;
            _radius = radius;
            _isClockwise = isClockwise;
            _isLargeAngle = isLargeAngle;
        }

        private Point? _start;
        private Point? _end;
        private Point _center;
        private double _radius;
        private double _startAngle;
        private double _endAngle;
        private bool _isClockwise;
        private bool _isLargeAngle;

        protected override void Draw(IContext context)
        {
            if (!_start.HasValue)
            {
                if (this == MainWindow.ActiveVisual && this != MainWindow.SelectedVisual)
                    context.DrawArc(MainWindow.ActivePen, _center, _radius, _startAngle, _endAngle, _isClockwise);
                else if (this == MainWindow.SelectedVisual)
                    context.DrawArc(MainWindow.SelectedPen, _center, _radius, _startAngle, _endAngle, _isClockwise);
                else context.DrawArc(MainWindow.WhitePen, _center, _radius, _startAngle, _endAngle, _isClockwise);
            }
        }
    }
}
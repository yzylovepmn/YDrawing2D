﻿using System;
using System.Collections.Generic;
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
        public static DrawingPen WhitePen = new DrawingPen(1, Colors.White, new double[] { 1, 3 });
        public static DrawingPen ActivePen = new DrawingPen(1, Colors.Blue, new double[] { 1, 3 });
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
                        _panel.MoveToTop(_visual, true);
                    //_panel.UpdateAll();
                }
            }
        }
        private static PresentationVisual _visual;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var len = Math.Max(ActualWidth, ActualHeight);
            _panel = new PresentationPanel(len, len, 96, 96, Colors.Black);
            Content = _panel;
            for (int i = 0; i < 400; i++)
            {
                //_panel.AddVisual(new Line(new Point(200, i), new Point(600, i)));
                //_panel.AddVisual(new Line(new Point(200, 800 - i), new Point(600, 800 - i)));
                //_panel.AddVisual(new Cicle(new Point(400, 400), 20 + i));
                //_panel.AddVisual(new Arc(new Point(400, 400), i, i * 2, 50 + i));
            }
            _panel.AddVisual(new Line(new Point(0, 0), new Point(800, 800)));
            _panel.AddVisual(new Cicle(new Point(200, 300), 200));
            _panel.AddVisual(new Arc(new Point(600, 500), 30, 300, 200));
            _panel.UpdateAll();
            _panel.MouseMove += _panel_MouseMove;
            _panel.MouseWheel += _panel_MouseWheel;
            _panel.MouseLeftButtonDown += _panel_MouseLeftButtonDown;
        }

        private void _panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p = e.GetPosition(_panel);
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
                ActiveVisual = VisualHelper.HitTest(_panel, e.GetPosition(_panel));
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
            if (this == MainWindow.ActiveVisual)
                context.DrawLine(_start, _end, MainWindow.ActivePen);
            else context.DrawLine(_start, _end, MainWindow.WhitePen);
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
            if (this == MainWindow.ActiveVisual)
                context.DrawCicle(_center, _radius, MainWindow.ActivePen);
            else context.DrawCicle(_center, _radius, MainWindow.WhitePen);
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
                if (this == MainWindow.ActiveVisual)
                    context.DrawArc(_center, _radius, _startAngle, _endAngle, _isClockwise, MainWindow.ActivePen);
                else context.DrawArc(_center, _radius, _startAngle, _endAngle, _isClockwise, MainWindow.WhitePen);
            }
            else
            {
                context.BeginFigure(_start.Value);
                if (this == MainWindow.ActiveVisual)
                    context.ArcTo(_end.Value, _radius, _isLargeAngle, _isClockwise, MainWindow.ActivePen);
                else context.ArcTo(_end.Value, _radius, _isLargeAngle, _isClockwise, MainWindow.WhitePen);
            }
        }
    }
}
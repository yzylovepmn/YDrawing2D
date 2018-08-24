using System;
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

        private PresentationPanel _panel;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var len = (int)Math.Max(ActualWidth, ActualHeight);
            _panel = new PresentationPanel(len, len, 96, 96, Colors.Black);
            Content = _panel;
            _panel.AddVisual(new Line(new Point(0, 0), new Point(800, 800)));
            _panel.AddVisual(new Line(new Point(0, 800), new Point(800, 0)));
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
            context.DrawLine(_start, _end, 1, Colors.White);
        }
    }
}
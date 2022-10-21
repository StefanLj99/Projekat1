using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Stefan_Ljubinkovic_PZ1
{
    /// <summary>
    /// Interaction logic for PolygonProperties.xaml
    /// </summary>
    public partial class PolygonProperties : Window
    {
        List<Point> Points { get; set; }
        public PolygonProperties(List<Point> points, string stroke, string fill, string strokeFill, string text, string textColor)
        {
            InitializeComponent();

            Points = points;

            comboBoxBorderColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            comboBoxTextColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            comboBoxColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);

            textBoxLine.Text = stroke;
            textBoxtText.Text = text;

            comboBoxBorderColor.SelectedValue = strokeFill;
            comboBoxTextColor.SelectedValue = textColor;
            comboBoxColor.SelectedValue = fill;
            

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            int stroke;

            if (int.TryParse(textBoxLine.Text, out stroke))
            {
                Polygon p = new Polygon();

                foreach (Point point in Points)
                {
                    p.Points.Add(new Point(point.X, point.Y));
                }

                p.StrokeThickness = stroke;

                p.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxColor.SelectedItem.ToString()));
                p.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxBorderColor.SelectedItem.ToString()));


                if (textBoxtText.Text != "")
                {
                    Canvas g = new Canvas();
                    g.Children.Add(p);

                    Point center = p.Points.Aggregate(new { xSum = 0.0, ySum = 0.0, n = 0 },
                        (acc, pPoint) => new
                        {
                            xSum = acc.xSum + pPoint.X,
                            ySum = acc.ySum + pPoint.Y,
                            n = acc.n + 1
                        },
                        acc => new Point(acc.xSum / acc.n, acc.ySum / acc.n));

                    TextBlock tb = new TextBlock();

                    Canvas.SetLeft(tb, center.X);
                    Canvas.SetTop(tb, center.Y);

                    tb.Text = textBoxtText.Text;
                    tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxTextColor.SelectedItem.ToString()));
                                        
                    tb.FontSize = 50;

                    g.Children.Add(tb);

                    g.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).PolygonChangeProperties;
                    ((MainWindow)Application.Current.MainWindow).mainCanvas.Children.Add(g);

                }
                else
                {
                    p.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).PolygonChangeProperties;
                    ((MainWindow)Application.Current.MainWindow).mainCanvas.Children.Add(p);
                }

                ((MainWindow)Application.Current.MainWindow).PointsForPolygon.Clear();

                ((MainWindow)Application.Current.MainWindow).ShapesCounter++;
                ((MainWindow)Application.Current.MainWindow).Item = EItem.NONE;

                this.Close();


            }
        }
    }
}

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
    /// Interaction logic for EllipseProperties.xaml
    /// </summary>
    public partial class EllipseProperties : Window
    {
        public int X { get; set; }
        public int Y { get; set; }
        public EllipseProperties(int x, int y, string height, string width, string border, string text, string color, string borderColor, string textColor)
        {
            InitializeComponent();

            X = x;

            Y = y;

            comboBoxColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            comboBoxBorderColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            comboBoxTextColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);


            textBoxVisina.Text = height;
            textBoxSirina.Text = width;
            textBoxLine.Text = border;
            textBoxText.Text = text;

            comboBoxColor.SelectedValue = color;
            comboBoxBorderColor.SelectedValue = borderColor;
            comboBoxTextColor.SelectedValue = textColor;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            int width, height, border;

            if (int.TryParse(textBoxVisina.Text, out height) && int.TryParse(textBoxSirina.Text, out width) && int.TryParse(textBoxLine.Text, out border))
            {
                Ellipse ellipse = new Ellipse();

                ellipse.Width = width;
                ellipse.Height = height;

                ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxColor.SelectedItem.ToString()));
                ellipse.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxBorderColor.SelectedItem.ToString()));
                ellipse.StrokeThickness = border;


                if (textBoxText.Text != "")
                {
                    Grid c = new Grid();
                    Canvas.SetLeft(c, X);
                    Canvas.SetTop(c, Y);

                    c.Children.Add(ellipse);

                    TextBlock txt = new TextBlock();
                    txt.HorizontalAlignment = HorizontalAlignment.Center;
                    txt.VerticalAlignment = VerticalAlignment.Center;
                    txt.TextAlignment = TextAlignment.Center;

                    txt.Text = textBoxText.Text;
                    txt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxTextColor.SelectedItem.ToString()));                  
                    
                    c.Children.Add(txt);

                    c.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).EllipseChangeProperties;
                    ((MainWindow)Application.Current.MainWindow).ShapesCounter++;
                    ((MainWindow)Application.Current.MainWindow).mainCanvas.Children.Add(c);
                    

                }
                else
                {
                    Canvas.SetLeft(ellipse, X);
                    Canvas.SetTop(ellipse, Y);

                    ellipse.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).EllipseChangeProperties;

                    ((MainWindow)Application.Current.MainWindow).mainCanvas.Children.Add(ellipse);


                    ((MainWindow)Application.Current.MainWindow).ShapesCounter++;
                }

                ((MainWindow)Application.Current.MainWindow).Item = EItem.NONE;
                this.Close();
            }
        }
    }
}

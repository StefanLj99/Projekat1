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
    /// Interaction logic for TextProperties.xaml
    /// </summary>
    public partial class TextProperties : Window
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TextProperties(int x, int y, string tekst, string size, string color)
        {
            InitializeComponent();

            X = x;
            Y = y;

            comboBoxColor.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);

            textBoxText.Text = tekst;
            textBoxSize.Text = size;
            comboBoxColor.SelectedValue = color;

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            int size;

            if (textBoxText.Text != "" && int.TryParse(textBoxSize.Text, out size))
            { 


                TextBlock tb = new TextBlock();
                tb.FontSize = size;
                tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comboBoxColor.SelectedItem.ToString()));
                tb.Text = textBoxText.Text;

                Canvas.SetTop(tb, Y);
                Canvas.SetLeft(tb, X);
                
                tb.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).TextChangeProperties;
                ((MainWindow)Application.Current.MainWindow).ShapesCounter++;
                ((MainWindow)Application.Current.MainWindow).mainCanvas.Children.Add(tb);
                

            }

            ((MainWindow)Application.Current.MainWindow).Item = EItem.NONE;

            this.Close();
        }
    }
}

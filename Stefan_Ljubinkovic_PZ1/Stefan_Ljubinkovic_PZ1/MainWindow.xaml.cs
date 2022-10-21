using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stefan_Ljubinkovic_PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public enum EItem { ELLIPSE, POLYGON, TEXT, UNDO, REDO, CLEAR, NONE}

    public partial class MainWindow : Window
    {
        public EItem Item { get; set; }

        public List<UIElement> undoRedoElements { get; set; } = new List<UIElement>();

        string path;
        List<string> xmls { get; set; } = new List<string>();

        public int ShapesCounter { get; set; }

        public List<Point> PointsForPolygon { get; set; }
        CanvasDrawing Cd { get; set; }

        UIElement lastElement { get; set; }

        List<UIElement> ClearedElements { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Cd = new CanvasDrawing();

            Item = EItem.NONE;

            PointsForPolygon = new List<Point>();

            ShapesCounter = 0;

            ClearedElements = new List<UIElement>();

            listXMLS.ItemsSource = xmls;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "XML Files|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                LoadCanvas();
            }
        }
        private void ClrPcker_Background_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Cd.ChangeColour(new SolidColorBrush(Color.FromRgb(ClrPcker_Background.SelectedColor.Value.R, ClrPcker_Background.SelectedColor.Value.G, ClrPcker_Background.SelectedColor.Value.B)),null);

        }
        private void LoadCanvas()
        {
            

            Cd.Init();

            Cd.Load(path);

            Cd.ScaleToCanvas(mainCanvas.Width, mainCanvas.Height);

            Cd.DrawSubNSw(mainCanvas);

            Cd.DrawLines(mainCanvas);
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            Item = EItem.ELLIPSE;
        }

        private void btnPolygon_Click(object sender, RoutedEventArgs e)
        {
            Item = EItem.POLYGON;
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            Item = EItem.TEXT;
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (Item == EItem.CLEAR)
            {
                foreach(UIElement uie in ClearedElements)
                {
                    mainCanvas.Children.Add(uie);
                    ShapesCounter++;
                }
                ClearedElements.Clear();
                Item = EItem.NONE;
            }
            else
            {
                
                    lastElement = mainCanvas.Children[mainCanvas.Children.Count - 1];

                    undoRedoElements.Add(lastElement);

                    mainCanvas.Children.Remove(lastElement);

                    ShapesCounter--;

                    Item = EItem.NONE;
                
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            //if (lastElement != null)
            //{
                try
                {
                    mainCanvas.Children.Add(undoRedoElements[undoRedoElements.Count - 1]) ;
                    undoRedoElements.RemoveAt(undoRedoElements.Count - 1);

                }catch (Exception) { }

                

                ShapesCounter++;

                Item = EItem.NONE;
            //}
            //lastElement = null;

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = mainCanvas.Children.Count - ShapesCounter; i < mainCanvas.Children.Count; i++)
            {
                ClearedElements.Add(mainCanvas.Children[i]);
            }
            mainCanvas.Children.RemoveRange(mainCanvas.Children.Count - ShapesCounter, ShapesCounter);
            Item = EItem.CLEAR;
            ShapesCounter = 0;
        }

        private void mainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(mainCanvas);

            switch (Item)
            {
                case EItem.ELLIPSE:
                    EllipseProperties winE = new EllipseProperties((int)point.X, (int)point.Y, "", "", "", "", "AntiqueWhite", "AntiqueWhite", "AntiqueWhite");
                    winE.Show();
                    break;
                case EItem.POLYGON:
                    PointsForPolygon.Add(point);
                    break;
                case EItem.TEXT:
                    TextProperties winT = new TextProperties((int)point.X, (int)point.Y, "", "", "AntiqueWhite");
                    winT.Show();
                    break;
                default:
                    break;
            }


        }

        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if(Item == EItem.POLYGON && PointsForPolygon.Count > 2)
            {
                List<Point> points = new List<Point>();

                foreach (Point p in PointsForPolygon){
                    points.Add(p);
                }

                PolygonProperties winP = new PolygonProperties(PointsForPolygon, "", "AntiqueWhite", "AntiqueWhite", "", "AntiqueWhite");
                winP.Show();
            }
        }

        public void EllipseChangeProperties(object sender, MouseButtonEventArgs e) {
            UIElement element = (UIElement)sender;

            if (element.GetType() == typeof(Grid))
            {
                Grid g = (Grid)sender;

                Ellipse el = (Ellipse)g.Children[0];

                TextBlock text = (TextBlock)g.Children[1];

                mainCanvas.Children.Remove(g);
                ShapesCounter--;

                EllipseProperties window = new EllipseProperties((int)Canvas.GetLeft(g), (int)Canvas.GetTop(g), el.Height.ToString(), el.Width.ToString(), el.StrokeThickness.ToString(), text.Text, BrushToString(el.Fill), BrushToString(el.Stroke), BrushToString(text.Foreground));
                window.Show();
            }
            else
            {
                Ellipse el = (Ellipse)sender;

                mainCanvas.Children.Remove(el);
                ShapesCounter--;

                EllipseProperties window = new EllipseProperties((int)Canvas.GetLeft(el), (int)Canvas.GetTop(el), el.Height.ToString(), el.Width.ToString(), el.StrokeThickness.ToString(), "", BrushToString(el.Fill), BrushToString(el.Stroke), "AntiqueWhite");
                window.Show();
                
                
            }
        }

        public void PolygonChangeProperties(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;

            if (element.GetType() == typeof(Canvas))
            {
                Canvas g = (Canvas)sender;

                Polygon p = (Polygon)g.Children[0];

                TextBlock text = (TextBlock)g.Children[1];

                mainCanvas.Children.Remove(g);
                ShapesCounter--;

                List<System.Windows.Point> points = new List<System.Windows.Point>();

                foreach (System.Windows.Point pointt in p.Points)
                {
                    points.Add(pointt);
                }

                PolygonProperties window = new PolygonProperties(points, p.StrokeThickness.ToString(), BrushToString(p.Fill), BrushToString(p.Stroke), text.Text, BrushToString((text.Foreground)));
                window.Show();
            }
            else
            {
                Polygon p = (Polygon)sender;

                mainCanvas.Children.Remove(p);
                ShapesCounter--;

                List<System.Windows.Point> points = new List<System.Windows.Point>();

                foreach (System.Windows.Point po in p.Points)
                {
                    points.Add(po);
                }

                PolygonProperties window = new PolygonProperties(points, p.StrokeThickness.ToString(), BrushToString(p.Fill), BrushToString(p.Stroke), "", "AntiqueWhite");

                window.Show();
            }

        }

        public void TextChangeProperties(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;

            mainCanvas.Children.Remove(tb);
            ShapesCounter--;

            TextProperties window = new TextProperties((int)Canvas.GetLeft(tb), (int)Canvas.GetTop(tb), tb.Text, tb.FontSize.ToString(), BrushToString(tb.Foreground));
            window.Show();



        }

        private string BrushToString(Brush a)
        {
            Color color = ((SolidColorBrush)a).Color;
            string selectedcolorname = "";
            foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
            {
                if ((Color)colorvalue.GetValue(null) == color)
                {
                    selectedcolorname = colorvalue.Name;
                }
            }

            return selectedcolorname;
        }



        private void btnScreen_Click_1(object sender, RoutedEventArgs e)
        {
            ScreenShot.takeScreenShoot();
        }
        
        private void btnLoadXMLS_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in LoadXMLs.getXmls())
            {
                xmls.Add(item);
            }
        }

        private void listXMLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            path = listXMLS.SelectedItem.ToString();
            Cd = new CanvasDrawing();
            Cd.Init();

            Cd.Load(path);

            Cd.ScaleToCanvas(mainCanvas.Width, mainCanvas.Height);

            Cd.DrawSubNSw(mainCanvas);

            Cd.DrawLines(mainCanvas);
        }

        private void btcSwitches_Click(object sender, RoutedEventArgs e)
        {
            Cd.ChangeSwitches();
        }
        bool flag = false;
        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            if (!flag)
            {
                Cd.notActiveNetwork();
                flag = true;
            }
            else
            {
                Cd.BackLines();
                flag = false;
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Multiselect = true;
            OpenFile.Title = "Select Picture(s)";
            OpenFile.Filter = "ALL supported Graphics| *.jpeg; *.jpg;*.png; *.jfif";
            if (OpenFile.ShowDialog() == true)
            {
                foreach (String file in OpenFile.FileNames)
                {
                    Add_Image(file);
                    break;
                }
            }
        }

        private void Add_Image(string file)
        {
            Console.WriteLine("Une image" + file);
            ImageBrush new_img = new ImageBrush();
            new_img.ImageSource = new BitmapImage(new Uri(file));

            Cd.ChangeColour(null, new_img);
        }
    }
}

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
using EGE;
using EGE.Environment;

namespace Map_editor
{
    /// <summary>
    /// Interaction logic for mapView.xaml
    /// </summary>
    public partial class mapView : UserControl
    {
        ScaleTransform zoom = new ScaleTransform(10, 10);
        double Thickness = 1;
        Double PixelScale = 20;

        public delegate void LocationUpdate(double X, double Y, object argument);
        public event LocationUpdate UpdateLocation;
        public event LocationUpdate MoveNode;
        public Dictionary<string, UIElement> MapObjects;
        Canvas lineCanvas = new Canvas();     
        public mapView()
        {
            InitializeComponent();
            MapObjects = new Dictionary<string, UIElement>();
            TransformGroup g = new TransformGroup();
            g.Children.Add(zoom);
            mapContainer.LayoutTransform = g;
            
            Line unitX = new Line();
            unitX.X2 = 10;
            unitX.StrokeThickness = Thickness;
            unitX.Stroke = Brushes.Red;
            map.Children.Add(unitX);
            Line unitZ = new Line();
            unitZ.Y2 = 10;
            unitZ.StrokeThickness = Thickness;
            unitZ.Stroke = Brushes.LimeGreen;
            map.Children.Add(unitZ);

            map.Children.Add(lineCanvas);
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            slider.Value -= e.Delta/2000f;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoom.ScaleX = 1/e.NewValue;
            zoom.ScaleY = 1/e.NewValue;
            map.Margin = new Thickness(map.Margin.Left, map.Margin.Top, 0, 0);
        }

        public void UpdateWorld()
        {
            for (int i = 0; i < Form1.currentWorld.CurrentMap.CurrentTerrain.Roads.Length; i++)
            {
                Node prevNode = new Node();
                EGE.Environment.Paths.Road r = Form1.currentWorld.CurrentMap.CurrentTerrain.Roads[i];
                for (int j = 0; j < r.RoadPath.PathNodes.Length; j++)
                {
                    string path = "CurrentMap/CurrentTerrain/Roads/" + i+"/RoadPath/PathNodes/" + j;
                    if (!MapObjects.Keys.Contains(path))
                    {
                        Button nodeBtn = new Button();
                        nodeBtn.Height = 20;
                        nodeBtn.Width = 20;
                        nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                        nodeBtn.VerticalAlignment = VerticalAlignment.Top;
                        nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
                        nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                        nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                        map.Children.Add(nodeBtn);
                        MapObjects.Add(path, nodeBtn);
                    }
                    Node n = (Node)Form1.getWorldValue(path);
                    double Top = (n.NodeLocation.Z * PixelScale) - 10;
                    double Left = (n.NodeLocation.X * PixelScale) - 10;
                    ((Button)MapObjects[path]).Tag = path;
                    ((Button)MapObjects[path]).Margin = new Thickness(Left, Top, 0, 0);

                    if (j > 0)
                    {
                        path = "CurrentMap/CurrentTerrain/Roads/" + i + "/" + j;
                        if (!MapObjects.Keys.Contains(path))
                        {
                            Line l = new Line();
                            l.StrokeThickness = Thickness;
                            l.Stroke = Brushes.Black;
                            
                            map.Children.Add(l);
                            MapObjects.Add(path, l);
                        }
                        
                        ((Line)MapObjects[path]).X1 = prevNode.NodeLocation.X*PixelScale;
                        ((Line)MapObjects[path]).Y1 = prevNode.NodeLocation.Z * PixelScale;
                        ((Line)MapObjects[path]).X2 = n.NodeLocation.X*PixelScale;
                        ((Line)MapObjects[path]).Y2 = n.NodeLocation.Z*PixelScale;
                        Canvas.SetZIndex(((Line)MapObjects[path]), -1);
                    }
                    prevNode = n;
                }
            }
        }

        private void NodeBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(mainGrid);
            p = new Point((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale);
            Form1.setWorldValue(((Button)sender).Tag+"/NodeLocation/X", Form1.currentWorld, (float)p.X);
            Form1.setWorldValue(((Button)sender).Tag + "/NodeLocation/Z", Form1.currentWorld, (float)p.Y);
            string[] pathParts = ((Button)sender).Tag.ToString().Split('/');
            string road = "CurrentMap/" + String.Join("/", pathParts, 1, pathParts.Length - 4);
            Form1.invokeWorldMethod(road + "/Build");
            UpdateWorld();

        }

        private void NodeBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void NodeBtn_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(mainGrid);
                p.X = ((p.X  ) / zoom.ScaleX) - map.Margin.Left;
                p.Y = ((p.Y  ) / zoom.ScaleY) - map.Margin.Top;
                ((Button)sender).Margin = new Thickness(p.X-10, p.Y-10, 0, 0);
            }
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            UpdateLocation.Invoke((Mouse.GetPosition(map).X)/PixelScale, (Mouse.GetPosition(map).Y)/PixelScale, this);
        }
        Point grabPoint;
        private void mainGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            grabPoint = Mouse.GetPosition(mainGrid);
        }

        private void mainGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Vector p = Point.Subtract(grabPoint,Mouse.GetPosition(mainGrid));
                p.X = p.X / zoom.ScaleX;
                p.Y = p.Y / zoom.ScaleY;
                map.Margin = new Thickness(map.Margin.Left - p.X, map.Margin.Top - p.Y, 0, 0);
                grabPoint = Mouse.GetPosition(mainGrid);
            }
        }
    }
}

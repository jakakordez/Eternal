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
        TranslateTransform offset = new TranslateTransform(10, 10);
        Double PixelScale = 20;
        bool MousePressed = false;

        public delegate void LocationUpdate(double X, double Y, object argument);
        public event LocationUpdate UpdateLocation;
        public event LocationUpdate MoveNode;
        public mapView()
        {
            InitializeComponent();
            TransformGroup g = new TransformGroup();
            g.Children.Add(zoom);
            g.Children.Add(offset);
            map.RenderTransform = g;
            
            Line unitX = new Line();
            unitX.X1 = 0;
            unitX.X2 = 10;
            unitX.Y1 = 0;
            unitX.Y2 = 0;
            unitX.StrokeThickness = Thickness;
            unitX.Stroke = Brushes.Red;
            map.Children.Add(unitX);
            Line unitZ = new Line();
            unitZ.X1 = 0;
            unitZ.X2 = 0;
            unitZ.Y1 = 0;
            unitZ.Y2 = 10;
            unitZ.StrokeThickness = Thickness;
            unitZ.Stroke = Brushes.LimeGreen;
            map.Children.Add(unitZ);
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            slider.Value += e.Delta/2000f;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoom.ScaleX = 1/e.NewValue;
            zoom.ScaleY = 1/e.NewValue;
            Thickness = 1 / e.NewValue;
            Thickness = 1 / e.NewValue;
        }

        public void DrawWorld()
        {
            map.Children.Clear();
            foreach (EGE.Environment.Paths.Road r in Form1.currentWorld.CurrentMap.CurrentTerrain.Roads)
            {
                Line l = new Line();
                l.StrokeThickness = Thickness;
                l.Stroke = Brushes.Black;
                bool first = true;
                foreach (EGE.Environment.Paths.PathNode p in r.RoadPath.PathNodes)
                {
                    if (first)
                    {
                        l.X1 = p.NodeLocation.X*PixelScale;
                        l.Y1 = p.NodeLocation.Z * PixelScale;
                        first = false;
                    }
                    else
                    {
                        l.X2 = p.NodeLocation.X * PixelScale;
                        l.Y2 = p.NodeLocation.Z * PixelScale;
                        map.Children.Add(l);
                        l = new Line();
                        l.StrokeThickness = Thickness;
                        l.Stroke = Brushes.Black;
                        l.X1 = p.NodeLocation.X * PixelScale;
                        l.Y1 = p.NodeLocation.Z * PixelScale;
                    }
                    addNode(p);
                }
            }
        }

        public Button addNode(Node obj)
        {
            Button nodeBtn = new Button();
            nodeBtn.Height = 10;
            nodeBtn.Width = 10;
            nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
            nodeBtn.VerticalAlignment = VerticalAlignment.Top;
            nodeBtn = UpdatePosition(nodeBtn, obj);
            nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
            nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
            nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
            map.Children.Add(nodeBtn);
            return nodeBtn;
        }

        private void NodeBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MousePressed = false;
        }

        private void NodeBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MousePressed = true;
        }

        private void NodeBtn_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (MousePressed)
            {
                double Top = (Mouse.GetPosition(map).Y)-5;
                double Left = (Mouse.GetPosition(map).X)-5;
                ((Button)sender).Margin = new Thickness(Left, Top, 0, 0);
                ((Node)((Button)sender).Tag).NodeLocation = new OpenTK.Vector3((float)(Top / PixelScale), ((Node)((Button)sender).Tag).NodeLocation.Y, (float)(Left / PixelScale));
            }
        }

        public Button UpdatePosition(Button btn, Node obj)
        {
            btn.Tag = obj;
            double Top = (obj.NodeLocation.Z * PixelScale) - 5;
            double Left = (obj.NodeLocation.X * PixelScale) - 5;
            btn.Margin = new Thickness(Left, Top, 0, 0);
            return btn;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            UpdateLocation.Invoke((Mouse.GetPosition(map).X)/PixelScale, (Mouse.GetPosition(map).Y)/PixelScale, this);
        }
    }
}

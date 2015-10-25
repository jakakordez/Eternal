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
        double PixelScale = 20;
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
            slider.Value += e.Delta/100;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoom.ScaleX = e.NewValue;
            zoom.ScaleY = e.NewValue;
            Thickness = 1 / e.NewValue;
            Thickness = 1 / e.NewValue;
        }

        public void DrawWorld(World currentWorld)
        {
            foreach (EGE.Environment.Paths.Road r in currentWorld.CurrentMap.CurrentTerrain.Roads)
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
                }
            }
        }

        public Button addNode(object obj)
        {
            Button nodeBtn = new Button();
            nodeBtn.Tag = obj;
            nodeBtn.Height = 10;
            nodeBtn.Width = 10;
            nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
            nodeBtn.VerticalAlignment = VerticalAlignment.Top;
            double Top = (((EGE.Environment.Node)obj).NodeLocation.Z * PixelScale) - 5;
            double Left = (((EGE.Environment.Node)obj).NodeLocation.X * PixelScale) - 5;
            nodeBtn.Margin = new Thickness(Left, Top, 0, 0);
            map.Children.Add(nodeBtn);
            return nodeBtn;
        }
    }
}

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
using EGE;
using EGE.Environment;
using System.IO;

namespace Map_editor
{
    /// <summary>
    /// Interaction logic for mapView.xaml
    /// </summary>
    public partial class mapView : UserControl
    {
        public enum PointerFunction
        {
            None = 1, Move, Height, Delete
        }

        public PointerFunction CurrentFunction = PointerFunction.None;
        ScaleTransform zoom = new ScaleTransform(10, 10);
        double Thickness = 10;
        Double PixelScale = 20;
        Point grabPoint;

        public delegate void LocationUpdate(double X, double Y, object argument, string additionalData);
        public event LocationUpdate UpdateLocation;
        public event LocationUpdate MoveNode;
        public Dictionary<string, UIElement> MapObjects;
        Canvas lineCanvas = new Canvas();
        Canvas heightfieldCanvas = new Canvas();
        Image heightfieldBitmap;
        public mapView()
        {
            InitializeComponent();
            MapObjects = new Dictionary<string, UIElement>();
            TransformGroup g = new TransformGroup();
            g.Children.Add(zoom);
            mapContainer.LayoutTransform = g;
            
            Line unitX = new Line();
            unitX.X2 = 10;
            unitX.StrokeThickness = 1;
            unitX.Stroke = Brushes.Red;
            map.Children.Add(unitX);
            Line unitZ = new Line();
            unitZ.Y2 = 10;
            unitZ.StrokeThickness = 1;
            unitZ.Stroke = Brushes.LimeGreen;
            map.Children.Add(unitZ);

            map.Children.Add(lineCanvas);
            map.Children.Add(heightfieldCanvas);
        }

        public void FocusNode(ulong key)
        {
            Button a = (Button)MapObjects["Nodes/" + key];
            double mx = -a.Margin.Left+(ActualWidth/zoom.ScaleX/2);
            double my = -a.Margin.Top+(ActualHeight / zoom.ScaleX / 2);
            map.Margin = new System.Windows.Thickness(mx, my, 0, 0);

        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            slider.Value -= e.Delta/500f;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double xhalf = ActualWidth/zoom.ScaleX / 2;
            double yhalf = ActualHeight / zoom.ScaleY / 2;
            map.Margin = new Thickness(map.Margin.Left-xhalf, map.Margin.Top-yhalf, 0, 0);
            zoom.ScaleX = 1/e.NewValue;
            zoom.ScaleY = 1/e.NewValue;
            xhalf = ActualWidth / zoom.ScaleX / 2;
            yhalf = ActualHeight / zoom.ScaleY / 2;
            map.Margin = new Thickness(map.Margin.Left+xhalf, map.Margin.Top+yhalf, 0, 0);
        }

        public void UpdateWorld()
        {
            foreach (var item in Nodes.NodeList)
            {
                if(item.Value.RelativeTo == 0)
                {
                    string path = "Nodes/" + item.Key;
                    if (!MapObjects.Keys.Contains(path))
                    {
                        Button nodeBtn = new Button();
                        nodeBtn.Height = 20;
                        nodeBtn.Width = 20;
                        nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                        nodeBtn.VerticalAlignment = VerticalAlignment.Top;
                        nodeBtn.BorderBrush = Brushes.Green;

                        nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                        nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                        nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
                        map.Children.Add(nodeBtn);
                        MapObjects.Add(path, nodeBtn);
                    }
                    OpenTK.Vector3 n = (OpenTK.Vector3)item.Value.Location;
                    double Top = (n.Z * PixelScale) - 10;
                    double Left = (n.X * PixelScale) - 10;
                    ((Button)MapObjects[path]).Tag = path;
                    ((Button)MapObjects[path]).Margin = new Thickness(Left, Top, 0, 0);
                }    
            }
            for (int i = 0; i < Form1.currentWorld.CurrentMap.CurrentTerrain.Roads.Length; i++)
            {
                OpenTK.Vector3 prevNode = new OpenTK.Vector3();
                EGE.Environment.Paths.Road r = Form1.currentWorld.CurrentMap.CurrentTerrain.Roads[i];
                for (int j = 0; j < r.RoadPath.Length; j++)
                {
                    string path = "CurrentMap/CurrentTerrain/Roads/" + i+"/RoadPath/" + j+ "/Ref/Location";
                    /*if (!MapObjects.Keys.Contains(path))
                    {
                        Button nodeBtn = new Button();
                        nodeBtn.Height = 20;
                        nodeBtn.Width = 20;
                        nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                        nodeBtn.VerticalAlignment = VerticalAlignment.Top;

                        nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                        nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                        map.Children.Add(nodeBtn);
                        MapObjects.Add(path, nodeBtn);
                    }*/
                    OpenTK.Vector3 n = (OpenTK.Vector3)Form1.getWorldValue(path);
                    /*double Top = (n.Z * PixelScale) - 10;
                    double Left = (n.X * PixelScale) - 10;
                    ((Button)MapObjects[path]).Tag = path;
                    ((Button)MapObjects[path]).Margin = new Thickness(Left, Top, 0, 0);*/

                    if (j > 0)
                    {
                        path = "CurrentMap/CurrentTerrain/Roads/" + i + "/" + j;
                        if (!MapObjects.Keys.Contains(path))
                        {
                            Line l = new Line();
                            l.StrokeThickness = Thickness;
                            l.Stroke = Brushes.DarkGray;
                            
                            map.Children.Add(l);
                            MapObjects.Add(path, l);
                        }
                        
                        ((Line)MapObjects[path]).X1 = prevNode.X*PixelScale;
                        ((Line)MapObjects[path]).Y1 = prevNode.Z * PixelScale;
                        ((Line)MapObjects[path]).X2 = n.X*PixelScale;
                        ((Line)MapObjects[path]).Y2 = n.Z*PixelScale;
                        Canvas.SetZIndex(((Line)MapObjects[path]), -1);
                    }
                    prevNode = n;
                }
            }

            if(heightfieldBitmap == null)
            {
                heightfieldBitmap = new Image();
                heightfieldBitmap.Height = Form1.currentWorld.CurrentMap.CurrentTerrain.TerrainHeightfield.Size * PixelScale;
                heightfieldBitmap.Width = heightfieldBitmap.Height;
                heightfieldBitmap.HorizontalAlignment = HorizontalAlignment.Left;
                heightfieldBitmap.VerticalAlignment = VerticalAlignment.Top;
                var bmp = EGE.Resources.textureToBitmap(Form1.currentWorld.CurrentMap.CurrentTerrain.TerrainHeightfield.TextureName);
                bmp.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                BitmapImage bmpi = new BitmapImage();
                bmpi.BeginInit();
                bmpi.StreamSource = new MemoryStream((byte[])converter.ConvertTo(bmp, typeof(byte[])));
                bmpi.EndInit();
                heightfieldBitmap.Source = bmpi;
                map.Children.Add(heightfieldBitmap);
            }
            
            Canvas.SetZIndex(heightfieldBitmap, -5);
        }

        private void NodeBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            grabPoint = Mouse.GetPosition(mainGrid);
        }

        private void NodeBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentFunction == PointerFunction.Move)
            {
                Point p = Mouse.GetPosition(mainGrid);
                p = new Point((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale);
                OpenTK.Vector3 Location = new OpenTK.Vector3((float)p.X, Nodes.NodeList[Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString()))].Location.Y, (float)p.Y);
                Nodes.NodeList[Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString()))].Location = Location;
                //Form1.setWorldValue(((Button)sender).Tag+"/X", Form1.currentWorld, (float)p.X);
                //Form1.setWorldValue(((Button)sender).Tag + "/Z", Form1.currentWorld, (float)p.Y);
                string[] pathParts = ((Button)sender).Tag.ToString().Split('/');
                foreach (var road in Form1.currentWorld.CurrentMap.CurrentTerrain.Roads)
                {
                    foreach (var point in road.RoadPath)
                    {
                        if (point.ID == Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString())))
                        {
                            road.Build();
                            break;
                        }
                    }
                }
            }
            else if(CurrentFunction == PointerFunction.Height)
            {
                Vector p = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid));
                float ydif = (float)(p.Y/10);
                OpenTK.Vector3 Location = Nodes.NodeList[Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString()))].Location+new OpenTK.Vector3(0, ydif, 0);
                Nodes.NodeList[Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString()))].Location = Location;
                string[] pathParts = ((Button)sender).Tag.ToString().Split('/');
                foreach (var road in Form1.currentWorld.CurrentMap.CurrentTerrain.Roads)
                {
                    foreach (var point in road.RoadPath)
                    {
                        if (point.ID == Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString())))
                        {
                            road.Build();
                            break;
                        }
                    }
                }
            }
            
           /* if (MapObjects.ContainsKey(Misc.pathUp(Misc.pathUp(((Button)sender).Tag.ToString())) + "/1/NodeLocation")) // Update only if road contains at least two nodes
            {
                string road = "CurrentMap/" + String.Join("/", pathParts, 1, pathParts.Length - 5);
                Form1.invokeWorldMethod(road + "/Build");
            }*/
            UpdateWorld();
        }

        private void NodeBtn_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (CurrentFunction == PointerFunction.Move)
                {
                    Point p = Mouse.GetPosition(mainGrid);
                    p.X = ((p.X) / zoom.ScaleX) - map.Margin.Left;
                    p.Y = ((p.Y) / zoom.ScaleY) - map.Margin.Top;
                    ((Button)sender).Margin = new Thickness(p.X - 10, p.Y - 10, 0, 0);
                }
                else if (CurrentFunction == PointerFunction.Height)
                {
                    Vector p = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid));
                    float ydif = (float)(p.Y / 10);
                    OpenTK.Vector3 Location = Nodes.NodeList[Convert.ToUInt64(Misc.pathName(((Button)sender).Tag.ToString()))].Location + new OpenTK.Vector3(0, ydif, 0);

                    UpdateLocation.Invoke((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale, this, " H: " + Location.Y);
                }
            }
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || CurrentFunction != PointerFunction.Height)
            {
                UpdateLocation.Invoke((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale, this, "");
            }
        }
        
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

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
            None = 1, Move, Height, RotateX, RotateY, RotateZ, Delete
        }
        Cursor NodeCursor = Cursors.Arrow;

        PointerFunction currentFunction = PointerFunction.None;
        public PointerFunction CurrentFunction{
            get{
                return currentFunction;
            }
            set{
                switch (value)
                {
                    case PointerFunction.None:
                        NodeCursor = Cursors.Arrow;
                        break;
                    case PointerFunction.Move:
                        NodeCursor = Cursors.SizeAll;
                        break;
                    case PointerFunction.Height:
                        NodeCursor = Cursors.SizeNS;
                        break;
                    case PointerFunction.RotateX:
                        NodeCursor = Cursors.ScrollE;
                        break;
                    case PointerFunction.RotateY:
                        NodeCursor = Cursors.ScrollN;
                        break;
                    case PointerFunction.RotateZ:
                        NodeCursor = Cursors.ScrollSW;
                        break;
                    case PointerFunction.Delete:
                        NodeCursor = Cursors.No; 
                        break;
                    default:
                        break;
                }
                MapObjects.Where(m => m.Value.GetType() == typeof(NodeObj)).ToList().ForEach(b => ((NodeObj)b.Value).Cursor = NodeCursor);
                currentFunction = value;
            }
        }
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

        public void FocusNode(string Id)
        {
            if (MapObjects.ContainsKey(Id))
            {
                NodeObj a = (NodeObj)MapObjects[Id];
                double mx = -a.Margin.Left + (ActualWidth / zoom.ScaleX / 2);
                double my = -a.Margin.Top + (ActualHeight / zoom.ScaleX / 2);
                map.Margin = new Thickness(mx, my, 0, 0);
            }
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
            foreach (var item in Form1.currentWorld.CurrentMap.ObjectCollection.ObjectReferences.GetNodes())
            {
                ObjectReference r = (ObjectReference)item.Value;
                string path = "CurrentMap/ObjectCollection/ObjectReferences/" + item.Key + "/Position";
                OpenTK.Vector3 n = r.Position.Location;
                if (!MapObjects.Keys.Contains(path))
                {
                    NodeObj nodeBtn = new NodeObj(path);
                    nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                    nodeBtn.VerticalAlignment = VerticalAlignment.Top;
                    nodeBtn.Cursor = NodeCursor;

                    nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                    nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                    nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
                    map.Children.Add(nodeBtn);
                    MapObjects.Add(path, nodeBtn);
                }
                ((NodeObj)MapObjects[path]).Locate(PixelScale);
            }
            for (int i = 0; i < Form1.currentWorld.CurrentMap.Roads.Length; i++)
            {
                OpenTK.Vector3 prevNode = new OpenTK.Vector3();
                EGE.Environment.Paths.Road r = Form1.currentWorld.CurrentMap.Roads[i];
                for (int j = 0; j < r.Points.Length; j++)
                {
                    string path = "CurrentMap/Roads/" + i + "/Points/" + j;
                    OpenTK.Vector3 n = Form1.currentWorld.CurrentMap.Roads[i].Points[j].Location;
                    if (!MapObjects.Keys.Contains(path))
                    {
                        NodeObj nodeBtn = new NodeObj(path);
                        nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                        nodeBtn.VerticalAlignment = VerticalAlignment.Top;
                        nodeBtn.Cursor = NodeCursor;

                        nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                        nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                        nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
                        map.Children.Add(nodeBtn);
                        MapObjects.Add(path, nodeBtn);
                    }
                    ((NodeObj)MapObjects[path]).Locate(PixelScale);

                    if (j > 0)
                    {
                        path = "CurrentMap/Roads/" + i + "/" + (j-1) +"-"+j;
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
                heightfieldBitmap.Height = Form1.currentWorld.CurrentMap.TerrainHeightfield.Size * PixelScale;
                heightfieldBitmap.Width = heightfieldBitmap.Height;
                heightfieldBitmap.HorizontalAlignment = HorizontalAlignment.Left;
                heightfieldBitmap.VerticalAlignment = VerticalAlignment.Top;
                var bmp = EGE.Resources.textureToBitmap(Form1.currentWorld.CurrentMap.TerrainHeightfield.TextureName);
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
            string[] pathParts = ((NodeObj)sender).Tag.ToString().Split('/');
            Node n = (Node)ObjectBrowser.getValue(((NodeObj)sender).Tag.ToString(), Form1.currentWorld);
            Point abs = new Point((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale);
            Vector rel = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid)) / 10;
            if (CurrentFunction == PointerFunction.Move)
            {
                OpenTK.Vector3 Location = new OpenTK.Vector3((float)abs.X, n.Location.Y, (float)abs.Y);
                n.Location = Location;
            }
            else if(CurrentFunction == PointerFunction.Height)
            {
                OpenTK.Vector3 Location = n.Location+new OpenTK.Vector3(0, (float)rel.Y, 0);
                n.Location = Location;
            }
            else
            {
                rel = rel / (2*Math.PI);
                OpenTK.Vector3 RotationDif = new OpenTK.Vector3();
                if (CurrentFunction == PointerFunction.RotateX) RotationDif = new OpenTK.Vector3((float)rel.Y, 0, 0);
                if (CurrentFunction == PointerFunction.RotateY) RotationDif = new OpenTK.Vector3(0, (float)rel.Y, 0);
                if (CurrentFunction == PointerFunction.RotateZ) RotationDif = new OpenTK.Vector3(0, 0, (float)rel.Y);
                n.Rotation = n.Rotation+RotationDif;
            }
            if(pathParts[1] == "Roads") Form1.currentWorld.CurrentMap.Roads[Convert.ToInt32(pathParts[2])].Build(Form1.currentWorld.CurrentMap.ObjectCollection);
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
                    ((NodeObj)sender).Margin = new Thickness(p.X - (((NodeObj)sender).ActualWidth/2), p.Y - (((NodeObj)sender).ActualHeight / 2), 0, 0);
                }
                else if (CurrentFunction == PointerFunction.Height)
                {
                    Node n = (Node)ObjectBrowser.getValue(((NodeObj)sender).Tag.ToString(), Form1.currentWorld);
                    Vector p = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid));
                    float ydif = (float)(p.Y / 10);
                    OpenTK.Vector3 Location = n.Location + new OpenTK.Vector3(0, ydif, 0);
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

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
using OpenTK;

namespace Map_editor
{
    /// <summary>
    /// Interaction logic for mapView.xaml
    /// </summary>
    public partial class mapView : UserControl
    {
        public enum PointerFunction
        {
            None = 1, Move, Height, RotateX, RotateY, RotateZ, Rotate, Delete
        }
        Cursor NodeCursor = Cursors.Arrow;

        PointerFunction currentFunction = PointerFunction.None;
        public PointerFunction CurrentFunction
        {
            get
            {
                return currentFunction;
            }
            set
            {
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

        public float Step = 1;
        ScaleTransform zoom = new ScaleTransform(10, 10);
        double Thickness = 10;
        Double PixelScale = 20;
        Point grabPoint;
        string selectedNode;

        public delegate void LocationUpdate(double X, double Y, double? Z, object argument, string additionalData);
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

        public void FocusNode(string Id, bool align = false)
        {
            if (MapObjects.ContainsKey(Id))
            {
                selectedNode = Id;
                NodeObj a = (NodeObj)MapObjects[Id];
                if (align)
                {
                    double mx = -a.Margin.Left + (ActualWidth / zoom.ScaleX / 2);
                    double my = -a.Margin.Top + (ActualHeight / zoom.ScaleX / 2);
                    map.Margin = new Thickness(mx, my, 0, 0);
                }
                Node n = (Node)ObjectBrowser.getValue(Id, Form1.currentWorld);
                ((EGE.Characters.DebugView)Form1.currentWorld.MainCharacter).Navigate(n, null);
            }
            else if (Id.StartsWith("CurrentMap/VehicleCollection/Vehicles"))
            {
                selectedNode = Id;
                Node n = (Node)ObjectBrowser.getValue(Id, Form1.currentWorld);
                var mesh = ObjectBrowser.getValue(string.Join("/", Id.Split('/').Take(4)) + "/vehicleMesh", Form1.currentWorld);
                ((EGE.Characters.DebugView)Form1.currentWorld.MainCharacter).Navigate(n, (string)mesh);
            }
        }

        public void ProcessInput(Key key)
        {
            switch (key)
            {
                case Key.NumPad0:
                    break;
                case Key.NumPad1:
                    moveNode(PointerFunction.Move, -Vector3.UnitX * Step);
                    break;
                case Key.NumPad2:
                    moveNode(PointerFunction.Move, Vector3.UnitZ * Step);
                    break;
                case Key.NumPad3:
                    moveNode(PointerFunction.Move, Vector3.UnitX * Step);
                    break;
                case Key.NumPad4:
                    moveNode(PointerFunction.Rotate, Vector3.UnitY * 0.1f * Step);
                    break;
                case Key.NumPad5:
                    moveNode(PointerFunction.Move, -Vector3.UnitZ * Step);
                    break;
                case Key.NumPad6:
                    moveNode(PointerFunction.Rotate, Vector3.UnitY * -0.1f * Step);
                    break;
                case Key.NumPad7:
                    break;
                case Key.NumPad8:
                    break;
                case Key.NumPad9:
                    break;
                case Key.Multiply:
                    break;
                case Key.Add:
                    moveNode(PointerFunction.Move, Vector3.UnitY * Step);
                    break;
                case Key.Separator:
                    break;
                case Key.Subtract:
                    moveNode(PointerFunction.Move, -Vector3.UnitY * Step);
                    break;
                case Key.Decimal:
                    break;
                case Key.Divide:
                    break;

            }
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            slider.Value -= e.Delta / 500f;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double xhalf = ActualWidth / zoom.ScaleX / 2;
            double yhalf = ActualHeight / zoom.ScaleY / 2;
            map.Margin = new Thickness(map.Margin.Left - xhalf, map.Margin.Top - yhalf, 0, 0);
            zoom.ScaleX = 1 / e.NewValue;
            zoom.ScaleY = 1 / e.NewValue;
            xhalf = ActualWidth / zoom.ScaleX / 2;
            yhalf = ActualHeight / zoom.ScaleY / 2;
            map.Margin = new Thickness(map.Margin.Left + xhalf, map.Margin.Top + yhalf, 0, 0);
        }

        private void UpdateNode(Vector3 n, string path)
        {
            if (!MapObjects.Keys.Contains(path))
            {
                NodeObj nodeBtn = new NodeObj(path);
                nodeBtn.HorizontalAlignment = HorizontalAlignment.Left;
                nodeBtn.VerticalAlignment = VerticalAlignment.Top;
                nodeBtn.Cursor = NodeCursor;

                nodeBtn.PreviewMouseMove += NodeBtn_PreviewMouseMove;
                nodeBtn.PreviewMouseUp += NodeBtn_PreviewMouseUp;
                nodeBtn.PreviewMouseDown += NodeBtn_PreviewMouseDown;
                nodeBtn.PreviewKeyDown += NodeBtn_PreviewKeyDown;
                map.Children.Add(nodeBtn);
                MapObjects.Add(path, nodeBtn);
            }
            ((NodeObj)MapObjects[path]).Locate(PixelScale);
        }

        private void NodeBtn_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ProcessInput(e.Key);
        }

        private void MapNodes(Node[] points, string basePath, string arrayPath)
        {
            string path;
            Vector3 prevNode = new Vector3();
            for (int j = 0; j < points.Length; j++)
            {
                path = basePath + arrayPath + j;
                Vector3 n = points[j].Location;
                UpdateNode(n, path);

                if (j > 0)
                {
                    path = basePath + (j - 1) + "-" + j;
                    if (!MapObjects.Keys.Contains(path))
                    {
                        Line l = new Line();
                        l.StrokeThickness = Thickness;
                        l.Stroke = Brushes.DarkGray;

                        map.Children.Add(l);
                        MapObjects.Add(path, l);
                    }

                    ((Line)MapObjects[path]).X1 = prevNode.X * PixelScale;
                    ((Line)MapObjects[path]).Y1 = prevNode.Z * PixelScale;
                    ((Line)MapObjects[path]).X2 = n.X * PixelScale;
                    ((Line)MapObjects[path]).Y2 = n.Z * PixelScale;
                    Canvas.SetZIndex(((Line)MapObjects[path]), -1);
                }
                prevNode = n;
            }
        }

        public void UpdateWorld()
        {
            foreach (var item in Form1.currentWorld.CurrentMap.ObjectCollection.ObjectReferences.GetNodes())
            {
                ObjectReference r = (ObjectReference)item.Value;
                string path = "CurrentMap/ObjectCollection/ObjectReferences/" + item.Key + "/Position";
                UpdateNode(r.Position.Location, path);
            }
            for (int i = 0; i < Form1.currentWorld.CurrentMap.Roads.Length; i++)
            {
                var r = Form1.currentWorld.CurrentMap.Roads[i];
                MapNodes(r.Points, "CurrentMap/Roads/" + i + "/", "Points/");
            }
            for (int i = 0; i < Form1.currentWorld.CurrentMap.Forests.Length; i++)
            {
                var r = Form1.currentWorld.CurrentMap.Forests[i];
                MapNodes(r.Polygon, "CurrentMap/Forests/" + i + "/", "Polygon/");
            }

            if (heightfieldBitmap == null)
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
            FocusNode(((NodeObj)sender).Tag.ToString(), false);
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
                n.Location = new Vector3((float)abs.X, n.Location.Y, (float)abs.Y);
            }
            else if (CurrentFunction == PointerFunction.Height)
            {
                n.Location = n.Location + new Vector3(0, (float)rel.Y, 0);
            }
            else
            {
                rel = rel / (2 * Math.PI);
                Vector3 RotationDif = new Vector3();
                if (CurrentFunction == PointerFunction.RotateX) RotationDif = new Vector3((float)rel.Y, 0, 0);
                if (CurrentFunction == PointerFunction.RotateY) RotationDif = new Vector3(0, (float)rel.Y, 0);
                if (CurrentFunction == PointerFunction.RotateZ) RotationDif = new Vector3(0, 0, (float)rel.Y);
                n.Rotation = n.Rotation + RotationDif;
            }

            if (pathParts[1] == "Roads" || pathParts[1] == "Forests") Form1.currentWorld.CurrentMap.Roads[Convert.ToInt32(pathParts[2])].Build(Form1.currentWorld.CurrentMap.ObjectCollection);
            UpdateWorld();
            
        }

        void ReportLocation(Vector3 l)
        {
            UpdateLocation.Invoke(l.X, l.Y, l.Z, null, "");
        }

        public Vector3 PickLocation()
        {
            double xLocation = ((ActualWidth / zoom.ScaleX / 2) - map.Margin.Left) / PixelScale;
            double yLocation = ((ActualHeight / zoom.ScaleY / 2) - map.Margin.Top) / PixelScale;
            return new Vector3((float)xLocation, 0, (float)yLocation);
        }

        private void moveNode(PointerFunction action, Vector3 value)
        {
            Node n = (Node)ObjectBrowser.getValue(selectedNode, Form1.currentWorld);
            string[] pathParts = selectedNode.Split('/');
            if (action == PointerFunction.Move || action == PointerFunction.Height)
            {
                n.Location += value;
                ReportLocation(n.Location);
            }
            else if (action == PointerFunction.Rotate)
            {
                n.Rotation += value / (float)(2 * Math.PI);
                ReportLocation(n.Rotation);
            }
            if (pathParts[1] == "Roads") Form1.currentWorld.CurrentMap.Roads[Convert.ToInt32(pathParts[2])].Build(Form1.currentWorld.CurrentMap.ObjectCollection);
            UpdateWorld();
            Graphics.PointerLocation = n;
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
                    ((NodeObj)sender).Margin = new Thickness(p.X - (((NodeObj)sender).ActualWidth / 2), p.Y - (((NodeObj)sender).ActualHeight / 2), 0, 0);
                }
                else if (CurrentFunction == PointerFunction.Height)
                {
                    Node n = (Node)ObjectBrowser.getValue(((NodeObj)sender).Tag.ToString(), Form1.currentWorld);
                    Vector p = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid));
                    float ydif = (float)(p.Y / 10);
                    Vector3 Location = n.Location + new Vector3(0, ydif, 0);
                    UpdateLocation.Invoke((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale, Location.Y, this, "");
                }
            }
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || CurrentFunction != PointerFunction.Height)
            {
                UpdateLocation.Invoke((Mouse.GetPosition(map).X) / PixelScale, (Mouse.GetPosition(map).Y) / PixelScale, null, this, "");
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
                Vector p = Point.Subtract(grabPoint, Mouse.GetPosition(mainGrid));
                p.X = p.X / zoom.ScaleX;
                p.Y = p.Y / zoom.ScaleY;
                map.Margin = new Thickness(map.Margin.Left - p.X, map.Margin.Top - p.Y, 0, 0);
                grabPoint = Mouse.GetPosition(mainGrid);
            }
        }
    }
}

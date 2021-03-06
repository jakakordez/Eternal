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
using EGE.Environment;

namespace Map_editor
{
    /// <summary>
    /// Interaction logic for NodeObj.xaml
    /// </summary>
    public partial class NodeObj : UserControl
    {
        public string Id;
        int buttonSize;
        RotateTransform rotTrans;
        
        public NodeObj(string Id)
        {
            InitializeComponent();
            this.Id = Id;

            Node n = getNode();
            buttonSize = n.RelativeTo == 0 ? 20 : 10;
            btn.Height = buttonSize;
            btn.Width = buttonSize;
            btn.BorderBrush = (n.RelativeTo==0)?Brushes.Red:Brushes.Green;
            rotTrans = new RotateTransform();
            TransformGroup g = new TransformGroup();
            g.Children.Add(rotTrans);
            g.Children.Add(new TranslateTransform(6, 0));
            rotationIndicator.RenderTransform = g;
        }

        public Node getNode()
        {
            //string[] pathParts = Id.Split('/');
            return (Node)ObjectBrowser.getValue(Id, Form1.currentWorld);
            //return Form1.currentWorld.CurrentMap.Roads[Convert.ToInt32(pathParts[2])].Points[Convert.ToInt32(pathParts[4])];
        }

        public void Locate(double PixelScale)
        {
            Node node = getNode();
            OpenTK.Vector3 n = node.Location;
            double Top = (n.Z * PixelScale) - (buttonSize / 2);
            double Left = (n.X * PixelScale) - (buttonSize / 2);
            Tag = Id;
            Margin = new Thickness(Left, Top, 0, 0);
            rotTrans.Angle = -(node.Rotation.Y*360/(2*Math.PI));
        }
    }
}

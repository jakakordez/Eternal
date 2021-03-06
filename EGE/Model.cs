﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Environment;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using BulletSharp;

namespace EGE
{
    public class Model 
    {
        public string MeshName { get; set; }

        public Node Center { get; set; }

        public Model()
        {
            MeshName = "";
        }

        public static Model Create(){
            Model n = new Model();
            n.Center = new Node();
            return n;    
        }

        public void Load()
        {
            //Tools.MeshManager.LoadMesh(MeshName);
        }

        public void Draw()
        {
            Matrix4 trans = Center.CreateTransform() * World.ViewMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(MeshName);
        }

        public void Build()
        {
        }
    }
}

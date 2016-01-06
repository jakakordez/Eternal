using System;
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
    public class Model : Buildable
    {
        public string MeshName { get; set; }

        public Environment.NodeReference Center { get; set; }

        public Model()
        {
            MeshName = "";
        }

        public static Model Create(){
            Model n = new Model();
            n.Center = new NodeReference(new Node());
            return n;    
        }

        public void Load()
        {
            //Tools.MeshManager.LoadMesh(MeshName);
        }

        public void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            trans = Center.Ref.CreateTransform() * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(MeshName);
        }

        public void Build()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Environment;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE
{
    public class Model:Buildable
    {
        public string MeshName { get; set; }
        public Environment.NodeReference Center { get;set; }

        public Model()
        {
            MeshName = "";
            Center = new NodeReference(new Node());
        }

        public void Load()
        {
            Tools.MeshManager.LoadMesh(MeshName);
        }

        public void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            trans = Center.Ref.CreateTransform() * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Tools.MeshManager.DrawMesh(MeshName);
        }

        public void Build()
        {
        }
    }
}

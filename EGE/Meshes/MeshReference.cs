using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE
{
    public class MeshReference
    {
        public string MeshKey{get;set;}

        public MeshReference()
        {
            MeshKey = "";
        }

        public void Draw()
        {
            Draw(Color4.Transparent);
        }

        public void Draw(Color4 Color)
        {
            Resources.DrawMesh(MeshKey, Color);
        }

        public void Draw(Vector3 Location)
        {
            Matrix4 trans = Matrix4.CreateTranslation(Location) * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(MeshKey);
        }
    }
}

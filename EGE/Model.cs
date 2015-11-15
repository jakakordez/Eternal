using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE
{
    public class Model
    {
        public string MeshName { get; set; }
        public Vector3 Location { get; set; }

        public Model()
        {
            MeshName = "";
        }

        public void Load()
        {
            Tools.MeshManager.LoadMesh(MeshName);
        }

        public void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            trans = Matrix4.CreateTranslation(Location) * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Tools.MeshManager.DrawMesh(MeshName);
        }
    }
}

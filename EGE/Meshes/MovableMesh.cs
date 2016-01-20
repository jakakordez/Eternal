using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Meshes
{
    class MovableMesh
    {
        public Vector3 MeshCenterOffset { get; set; }
        public Vector3 MeshRotation { get; set; }
        public string Mesh { get; set; }

        public MovableMesh()
        {
            Mesh = "";
        }

        public void Draw(Matrix4 transform)
        {
            Matrix4 trans = Matrix4.CreateRotationY(MeshRotation.Y) * Matrix4.CreateRotationZ(MeshRotation.Z) * Matrix4.CreateRotationX(MeshRotation.X);
            trans *= Matrix4.CreateTranslation(MeshCenterOffset) * transform * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(Mesh);
        }
    }
}

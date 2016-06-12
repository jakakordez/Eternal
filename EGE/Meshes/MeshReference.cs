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
        public string PrimaryMesh { get; set; }
        public string LowPolyMesh { get; set; }
        public Color4 MeshColor { get; set; }

        public MeshReference()
        {
            PrimaryMesh = "";
            LowPolyMesh = "";
            MeshColor = Color4.Transparent;
        }

        public void Draw(Matrix4 Transform, Vector3 eye)
        {
            Matrix4 trans = Transform * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            float l = (eye - Transform.ExtractTranslation()).LengthSquared;
            if (l < 600) Resources.DrawMesh(PrimaryMesh, MeshColor);
            else if (l < 90000) Resources.DrawMesh(LowPolyMesh, MeshColor);
            else Resources.DrawMesh(LowPolyMesh, MeshColor, true);
        }
    }
}

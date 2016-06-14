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
    }
}

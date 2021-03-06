﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Meshes
{
    class RotatableMesh:MovableMesh
    {
        public float XOffset { get; set; }
        public float XScale { get; set; }
        public float YOffset { get; set; }
        public float YScale { get; set; }
        public float ZOffset { get; set; }
        public float ZScale { get; set; }

        public RotatableMesh()
        {
            XScale = 1;
            YScale = 1;
            ZScale = 1;
        }

        public void SetX(float value)
        {
            Offset.Rotation = new Vector3(XOffset + (value * XScale), Offset.Rotation.Y, Offset.Rotation.Z);
        }

        public void SetY(float value)
        {
            Offset.Rotation = new Vector3(MeshRotation.X, YOffset + (value * YScale), Offset.Rotation.Z);
        }

        public void SetZ(float value)
        {
            Offset.Rotation = new Vector3(Offset.Rotation.X, Offset.Rotation.Y, ZOffset + (value * ZScale));
        }
    }
}

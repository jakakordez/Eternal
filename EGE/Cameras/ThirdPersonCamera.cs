using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace EGE
{
    class ThirdPersonCamera:Camera
    {
        public ThirdPersonCamera(CameraDefinition def) : base(def) { }

        public override void GenerateLookAt(Vector3 center)
        {
            Matrix4 e = Matrix4.Identity * Matrix4.CreateTranslation(-CameraSettings.Distance, 0, 0) * Matrix4.CreateRotationZ(Orientation.X) * Matrix4.CreateRotationY(Orientation.Y) * Matrix4.CreateTranslation(center);
            World.WorldMatrix = Matrix4.LookAt(e.ExtractTranslation(), center, Vector3.UnitY);
        }
    }
}

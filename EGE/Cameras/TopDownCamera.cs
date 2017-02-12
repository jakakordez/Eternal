using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE
{
    class TopDownCamera:Camera
    {
        public TopDownCamera(CameraDefinition def) : base(def) { }

        public override void GenerateLookAt(Vector3 center)
        {
            World.ViewMatrix = Matrix4.LookAt(center, center - Vector3.UnitY, -Vector3.UnitZ);
        }
    }
}

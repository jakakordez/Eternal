using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Newtonsoft.Json;

namespace EGE.Environment
{
    public class Node
    {
        public Vector3 Location { get; set; }
        public Vector3 Rotation { get;set; }

        public ulong RelativeTo;

        public Node() {
            Location = Vector3.Zero;
            Rotation = Vector3.Zero;
        }
        public Node(Vector3 Location) {
            this.Location = Location;
            Rotation = Vector3.Zero;
        }

        public Matrix4 CreateTransform()
        {
            return Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Location);
        }
    }
}

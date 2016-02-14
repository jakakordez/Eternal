using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;

namespace EGE.Environment.Paths
{
    public class JunctionReference
    {
        RigidBody CollisionObject;
        public NodeReference Location { get; set; }
        public ulong ID { get; set; }

        public JunctionReference()
        {
            Location = new NodeReference();
        }

        public void Load(CollisionShape sh)
        {
            CollisionObject = World.CreateRigidBody(0, Matrix4.CreateTranslation(Location.AbsPosition()), sh);
        }
    }
}

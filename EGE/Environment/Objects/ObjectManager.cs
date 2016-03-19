using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Environment
{
    public class ObjectManager
    {
        public ObjectCollection Objects { get; set; }
        public ObjectReference[] ObjectReferences { get; set; }

        public ObjectManager()
        {
            Objects = new ObjectCollection();
            ObjectReferences = new ObjectReference[0];
        }

        public void Load()
        {
            for (int i = 0; i < ObjectReferences.Length; i++)
            {
                Object o = Objects.Get(ObjectReferences[i].Object);
                CollisionShape shape;
                if (o.CollisionMesh == "") shape = Resources.GetMeshCollisionShape(o.ObjectMesh);
                else shape = Resources.GetMeshCollisionShape(o.CollisionMesh);

                ObjectReferences[i].CollisionObject = World.CreateRigidBody(0, ObjectReferences[i].Location.CreateTransform(), shape);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < ObjectReferences.Length; i++)
            {
                Matrix4 trans = ObjectReferences[i].Location.CreateTransform() * World.WorldMatrix;
                GL.LoadMatrix(ref trans);
                Object o = Objects.Get(ObjectReferences[i].Object);
                Resources.DrawMesh(o.ObjectMesh);
            }
        }
    }
}

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
        public ObjectReferenceCollection ObjectReferences { get; set; }

        public ObjectManager()
        {
            Objects = new ObjectCollection();
            ObjectReferences = new ObjectReferenceCollection();
        }

        public void Load()
        {
            foreach (var obref in ObjectReferences.GetNodes().ToArray())
            {
                ObjectReference value = (ObjectReference)obref.Value;
                Object o = (Object)Objects.Get(value.Object);
                CollisionShape shape;
                shape = o.ObjectMesh.GetCollisionMesh(false);
                value.CollisionObject = World.CreateRigidBody(0, value.Position.CreateTransform(), shape);
            }
        }

        public void Draw()
        {
            foreach (var obref in ObjectReferences.GetNodes())
            {
                ObjectReference value = (ObjectReference)obref.Value;
                Object o = (Object)Objects.Get(value.Object);
                Resources.DrawMesh(o.ObjectMesh, value.Position.CreateTransform());
            }
        }
    }
}

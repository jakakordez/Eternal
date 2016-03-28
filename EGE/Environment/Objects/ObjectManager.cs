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
                Object o = (Object)Objects.Get(((ObjectReference)obref.Value).Object);
                CollisionShape shape;
                if (o.CollisionMesh == "") shape = Resources.GetMeshCollisionShape(o.ObjectMesh);
                else shape = Resources.GetMeshCollisionShape(o.CollisionMesh);

                ((ObjectReference)obref.Value).CollisionObject = World.CreateRigidBody(0, ((ObjectReference)obref.Value).Position.CreateTransform(), shape);
            }
        }

        public void Draw()
        {
            foreach (var obref in ObjectReferences.GetNodes())
            {
                Matrix4 trans = ((ObjectReference)obref.Value).Position.CreateTransform() * World.WorldMatrix;
                GL.LoadMatrix(ref trans);
                Object o = (Object)Objects.Get(((ObjectReference)obref.Value).Object);
                Resources.DrawMesh(o.ObjectMesh);
            }
        }
    }
}

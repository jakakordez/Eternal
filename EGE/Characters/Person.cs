using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Characters
{
    class Person : Character
    {
        RigidBody CharacterBody;

        static CameraDefinition defaultCameraDefinition = new CameraDefinition()
        {
            Distance = 10,
            FPV = true,
            Offset = Vector3.Zero,
            ViewAngle = Vector2.One,
            Style = DrawingStyle.Normal
        };

        public Person(Vector3 StartPosition)
        {
            CameraList = new Camera[] { new Cameras.FirstPersonCamera() };
            CurrentCamera = 0;
            SphereShape body = new SphereShape(0.5f);
            CharacterBody = World.CreateRigidBody(80, Matrix4.CreateTranslation(StartPosition), body);
            
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);

            World.WorldMatrix = Camera.GenerateLookAt(CharacterBody.CenterOfMassPosition+(Vector3.UnitY), CameraList[0].Orientation, defaultCameraDefinition);
            GL.LoadMatrix(ref World.WorldMatrix);
        }

        public override void Update(float elaspedTime)
        {
            CameraList[0].Update();
           
            Vector2 direction = Misc.getCartesian(-CameraList[0].Orientation.Y);
            if (Controller.In(Func.Acceleration) == 1) CharacterBody.ApplyCentralForce(new Vector3(direction.X, 0, direction.Y)*800);
            if (Controller.In(Func.Brake) == 1) CharacterBody.ApplyCentralForce(-new Vector3(direction.X, 0, direction.Y) * 800);
            if (Controller.In(Func.Right) == 1) CharacterBody.ApplyCentralForce(Vector3.UnitZ*800);
            if (Controller.In(Func.Left) == 1) CharacterBody.ApplyCentralForce(-Vector3.UnitZ*800);
        }
    }
}

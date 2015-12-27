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
        float WalkingSpeed = 4, RunningSpeed = 10;

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
            Vector2 sideDirection = Misc.getCartesian(-CameraList[0].Orientation.Y+MathHelper.PiOver2);
            CharacterBody.LinearVelocity = new Vector3(0, CharacterBody.LinearVelocity.Y, 0);
            float y = CharacterBody.LinearVelocity.Y;
            if (Controller.In(Func.Acceleration))
            {
                if(Controller.In(Func.FastMode)) 
                    CharacterBody.LinearVelocity += new Vector3(direction.X*RunningSpeed, CharacterBody.LinearVelocity.Y, direction.Y*RunningSpeed);
                else CharacterBody.LinearVelocity += new Vector3(direction.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, direction.Y * WalkingSpeed);
            }
            else if(Controller.In(Func.Brake))
                CharacterBody.LinearVelocity += new Vector3(-direction.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, -direction.Y * WalkingSpeed);
            if(Controller.In(Func.Right))
                CharacterBody.LinearVelocity += new Vector3(sideDirection.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, sideDirection.Y * WalkingSpeed);
            else if (Controller.In(Func.Left))
                CharacterBody.LinearVelocity += new Vector3(-sideDirection.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, -sideDirection.Y * WalkingSpeed);
            CharacterBody.LinearVelocity = new Vector3(CharacterBody.LinearVelocity.X, y, CharacterBody.LinearVelocity.Z);
        }
    }
}

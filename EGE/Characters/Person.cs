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
        float WalkingSpeed = 4, RunningSpeed = 100;
        public Vehicles.Vehicle ControlledVehicle;

        public Person(Vector3 StartPosition)
        {
            CameraDefinition defaultCameraDefinition = new CameraDefinition()
            {
                Distance = 10,
                FPV = true,
                Offset = Vector3.UnitY,
                ViewAngle = Vector2.One,
                Style = DrawingStyle.Normal
            };

            CameraList = new Camera[] { new FirstPersonCamera(defaultCameraDefinition), new ThirdPersonCamera(defaultCameraDefinition) };
            CurrentCamera = 0;
            SphereShape body = new SphereShape(0.5f);
            CharacterBody = World.CreateRigidBody(80, Matrix4.CreateTranslation(StartPosition), body);
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);

            if (ControlledVehicle == null)
                CameraList[CurrentCamera].GenerateLookAt(CharacterBody.CenterOfMassPosition);
            else ControlledVehicle.DrawCamera();
            GL.LoadMatrix(ref World.WorldMatrix);
        }

        public override void Update(float elaspedTime)
        {
            Vector2 direction = Misc.getCartesian(-CameraList[0].Orientation.Y);
            Vector2 sideDirection = Misc.getCartesian(-CameraList[0].Orientation.Y+MathHelper.PiOver2);

            if (ControlledVehicle == null)
            {
                float y = CharacterBody.LinearVelocity.Y;
                if (Controller.In(Func.Forward))
                {
                    if (Controller.In(Func.FastMode))
                        CharacterBody.LinearVelocity = new Vector3(direction.X * RunningSpeed, CharacterBody.LinearVelocity.Y, direction.Y * RunningSpeed);
                    else CharacterBody.LinearVelocity = new Vector3(direction.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, direction.Y * WalkingSpeed);
                }
                else if (Controller.In(Func.Backward))
                    CharacterBody.LinearVelocity = new Vector3(-direction.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, -direction.Y * WalkingSpeed);
                else CharacterBody.LinearVelocity = new Vector3(0, CharacterBody.LinearVelocity.Y, 0);
                if (Controller.In(Func.Right))
                    CharacterBody.LinearVelocity += new Vector3(sideDirection.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, sideDirection.Y * WalkingSpeed);
                else if (Controller.In(Func.Left))
                    CharacterBody.LinearVelocity += new Vector3(-sideDirection.X * WalkingSpeed, CharacterBody.LinearVelocity.Y, -sideDirection.Y * WalkingSpeed);
                CharacterBody.LinearVelocity = new Vector3(CharacterBody.LinearVelocity.X, y, CharacterBody.LinearVelocity.Z);
                if (Controller.In(Func.Jump)) CharacterBody.ApplyCentralImpulse(new Vector3(0, 40, 0));
            }
            else ControlledVehicle.HandleInput();

            if (ControlledVehicle == null)
            {
                CameraList[CurrentCamera].Update();
                if (Controller.Pressed(Func.SwitchView))
                {
                    CurrentCamera = (CurrentCamera + 1) % CameraList.Length;
                }
            }
            else
            {
                ControlledVehicle.UpdateCamera();
                if (Controller.Pressed(Func.SwitchView)) ControlledVehicle.NextCamera();
            }
        }
    }
}

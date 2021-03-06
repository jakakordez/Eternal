﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using EGE.Environment;
using EGE.Meshes;

namespace EGE.Vehicles
{
    class Car:RoadVehicle
    {
        RaycastVehicle raycastVehicle;
        CollisionShape collisionShape;
        public Vector3 Dimensions { get; set; }
        public Vector2 SteeringWheelAngle { get; set; }
        public Node FrontWheel { get; set; }
        public Node RearWheel { get; set; }
        public float WheelRadius { get; set; }
        public float WheelWidth { get; set; }
        public float WheelFriction { get; set; }
        public float SuspensionStiffness { get; set; }
        public float SuspensionDamping { get; set; }
        public float SuspensionCompression { get; set; }
        public float RollInfluence { get; set; }
        public float SuspensionRestLength { get; set; }

        public Vector3 wheelDirectionCS0 = new Vector3(0, -1, 0);
        public Vector3 wheelAxleCS = new Vector3(-1, 0, 0);

        public const int rightIndex = 0;
        public const int upIndex = 1;
        public const int forwardIndex = 2;

        public RotatableMesh SteeringWheel { get; set; }
        public RotatableMesh VelocityNeedle { get; set; }
        public RotatableMesh RPMNeedle { get; set; }

        public float MaxEnginePower { get; set; }
        public float MaxEngineRPM { get; set; }
        public float[] GearRatio { get; set; }
        public Color4[] PrimaryColors { get; set; }
        float Throttle, RPM, Clutch;
        public int CurrentGear;

        public Car()
        {
            SteeringWheel = new RotatableMesh();
            SteeringWheel.ZScale = -5;
            VelocityNeedle = new RotatableMesh();
            RPMNeedle = new RotatableMesh();
            GearRatio = new float[] { -1, 1 };
            PrimaryColors = new Color4[] { Color4.White};
            CurrentGear = 1;
            FrontWheel = new Node();
            RearWheel = new Node();
        }

        public override void Load(Vector3 Location)
        {
            VehicleMesh.MeshColor = PrimaryColors[Global.RNG.Next(PrimaryColors.Length)];

            CollisionShape chassisShape = new BoxShape(Dimensions.Y / 2, Dimensions.Z / 2, Dimensions.X / 2);
            collisionShape = new CompoundShape();
            //localTrans effectively shifts the center of mass with respect to the chassis
            Matrix4 localTrans = Matrix4.CreateTranslation(0 * Vector3.UnitY);
            ((CompoundShape)collisionShape).AddChildShape(localTrans, chassisShape);
            collisionShape = VehicleMesh.GetCollisionMesh(true);
            vehicleBody = World.CreateRigidBody(Mass, Matrix4.CreateTranslation(Location), collisionShape);
            
            // create vehicle
            RaycastVehicle.VehicleTuning tuning = new RaycastVehicle.VehicleTuning();
            raycastVehicle = new RaycastVehicle(tuning, vehicleBody, new DefaultVehicleRaycaster(World.DynamicsWorld));

            vehicleBody.ActivationState = ActivationState.DisableDeactivation;

            // choose coordinate system
            raycastVehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);

            Vector3 connectionPointCS0 = FrontWheel.Location +(Vector3.UnitY*SuspensionRestLength); // Front left
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, true);

            connectionPointCS0 = FrontWheel.Location * new Vector3(-1, 1, 1) + (Vector3.UnitY * SuspensionRestLength); // Front right
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, -wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, true);

            connectionPointCS0 = RearWheel.Location * new Vector3(-1, 1, 1) + (Vector3.UnitY * SuspensionRestLength); // Rear right
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, -wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, false);

            connectionPointCS0 = RearWheel.Location + (Vector3.UnitY * SuspensionRestLength); // Rear left
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, false);

            for (int i = 0; i < raycastVehicle.NumWheels; i++)
            {
                WheelInfo wheel = raycastVehicle.GetWheelInfo(i);
                wheel.SuspensionStiffness = SuspensionStiffness;
                wheel.WheelsDampingRelaxation = SuspensionDamping;
                wheel.WheelsDampingCompression = SuspensionCompression;
                wheel.FrictionSlip = WheelFriction;
                wheel.RollInfluence = RollInfluence;
            }

            World.DynamicsWorld.AddAction(raycastVehicle);
        }

        public override void Draw(Vector3 eye)
        {
            Resources.DrawMesh(VehicleMesh, vehicleBody.CenterOfMassTransform);
            if ((vehicleBody.CenterOfMassPosition - eye).LengthSquared < 900)
            {
                SteeringWheel.Draw(vehicleBody.CenterOfMassTransform);
                if (CurrentCamera == 0)
                {
                    VelocityNeedle.Draw(vehicleBody.CenterOfMassTransform);
                    RPMNeedle.Draw(vehicleBody.CenterOfMassTransform);
                }
                for (int i = 0; i < 4; i++) DrawWheel(false, i);
            }
            else
            {
                for (int i = 0; i < 4; i++) DrawWheel(true, i);
            }
        }

        private void DrawWheel(bool lowPoly, int number)
        {
            Matrix4 wheel = Matrix4.Identity;
            if(lowPoly) wheel *= Matrix4.CreateScale(new Vector3(WheelWidth, WheelRadius, WheelRadius));
            wheel *= raycastVehicle.GetWheelTransformWS(number) * World.ViewMatrix;
            GL.LoadMatrix(ref wheel);
            Resources.DrawMesh(lowPoly?"meshes/wheel":WheelMesh);
        }

        public override void Update()
        {
            base.Update();
            raycastVehicle.ApplyEngineForce(-GearRatio[CurrentGear] * Thrust * 5000, 2);
            raycastVehicle.ApplyEngineForce(GearRatio[CurrentGear] * Thrust * 5000, 3);
            raycastVehicle.SetSteeringValue(Steering, 0);
            raycastVehicle.SetSteeringValue(Steering, 1);
            //raycastVehicle.SetBrake(Brake*100, 0);
            //raycastVehicle.SetBrake(Brake*100, 1);
            raycastVehicle.SetBrake(Brake*400, 2);
            raycastVehicle.SetBrake(Brake*400, 3);

            SteeringWheel.SetZ(Steering);
            //SteeringWheel.MeshRotation = new Vector3(SteeringWheel.MeshRotation.X, SteeringWheel.MeshRotation.Y, -5*Steering);
            VelocityNeedle.SetZ(raycastVehicle.CurrentSpeedKmHour/50);
            RPM = (float)((Math.Sin(Clutch * MathHelper.PiOver2)*1000) + ((1-Math.Sin(Clutch * MathHelper.PiOver2))*0));
            RPMNeedle.SetZ(RPM);
        }

        public override void HandleInput()
        {
            if (Controller.In(Func.Acceleration) && Thrust < 1)
            {
                Thrust += 0.1f;
            }
            else Thrust *= 0.5f;
            if (Controller.In(Func.Brake) && Brake < 1)
            {
                Brake += 0.1f;
            }
            else Brake *= 0.5f;
            if (Controller.In(Func.Left) && Steering < SteeringClamp)
            {
                Steering += 0.01f;
                if (Steering > SteeringClamp) Steering = SteeringClamp;
            }
            else if (Controller.In(Func.Right) && Steering > -SteeringClamp)
            {
                Steering -= 0.01f;
                if (Steering < -SteeringClamp) Steering = -SteeringClamp;
            }
            if(!Controller.In(Func.Right) && !Controller.In(Func.Left)) Steering *= 0.8f;

            if (Controller.Pressed(Func.GearUp)) CurrentGear = (CurrentGear + GearRatio.Length + 1) %GearRatio.Length;
            if (Controller.Pressed(Func.GearDown)) CurrentGear = (CurrentGear + GearRatio.Length - 1) % GearRatio.Length;
        }
    }
}

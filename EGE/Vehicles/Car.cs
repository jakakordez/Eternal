using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE.Vehicles
{
    class Car:RoadVehicle
    {
        RaycastVehicle raycastVehicle;
        CollisionShape collisionShape;
        public Vector3 Dimensions { get; set; }
        public Vector2 SteeringWheelAngle { get; set; }
        public Vector2 FrontWheelLocation { get; set; }
        public Vector2 RearWheelLocation { get; set; }
        public float WheelRadius { get; set; }
        public float WheelWidth { get; set; }
        public float WheelFriction { get; set; }
        public float SuspensionStiffness { get; set; }
        public float SuspensionDamping { get; set; }
        public float SuspensionCompression { get; set; }
        public float RollInfluence { get; set; }
        public float SuspensionHeight { get; set; }
        public float SuspensionRestLength { get; set; }

        public Vector3 wheelDirectionCS0 = new Vector3(0, -1, 0);
        public Vector3 wheelAxleCS = new Vector3(-1, 0, 0);

        public const int rightIndex = 0;
        public const int upIndex = 1;
        public const int forwardIndex = 2;

        public Meshes.MovableMesh SteeringWheel { get; set; }
        public Meshes.RotatableMesh VelocityNeedle { get; set; }
        public Meshes.RotatableMesh RPMNeedle { get; set; }

        public float MaxEnginePower { get; set; }
        public float MaxEngineRPM { get; set; }
        public float[] GearRatio { get; set; }
        float Throttle, RPM, CurrentGear, Clutch;

        public Car()
        {
            SteeringWheel = new Meshes.MovableMesh();
            VelocityNeedle = new Meshes.RotatableMesh();
            RPMNeedle = new Meshes.RotatableMesh();
            GearRatio = new float[0];
        }

        public void Load(Vector3 Location)
        {
            CollisionShape chassisShape = new BoxShape(Dimensions.Y / 2, Dimensions.Z / 2, Dimensions.X / 2);
            collisionShape = new CompoundShape();
            //localTrans effectively shifts the center of mass with respect to the chassis
            Matrix4 localTrans = Matrix4.CreateTranslation(0 * Vector3.UnitY);
            ((CompoundShape)collisionShape).AddChildShape(localTrans, chassisShape);

            vehicleBody = World.CreateRigidBody(Mass, Matrix4.CreateTranslation(Location), collisionShape);
            
            // create vehicle
            RaycastVehicle.VehicleTuning tuning = new RaycastVehicle.VehicleTuning();
            raycastVehicle = new RaycastVehicle(tuning, vehicleBody, new DefaultVehicleRaycaster(World.DynamicsWorld));

            vehicleBody.ActivationState = ActivationState.DisableDeactivation;


            bool isFrontWheel = true;
            float CUBE_HALF_EXTENTS = Dimensions.Y / 2;
            // choose coordinate system
            raycastVehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);

            Vector3 connectionPointCS0 = new Vector3(FrontWheelLocation.Y, SuspensionHeight, FrontWheelLocation.X);
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, isFrontWheel);

            connectionPointCS0 = new Vector3(-FrontWheelLocation.Y, SuspensionHeight, FrontWheelLocation.X);
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, isFrontWheel);

            isFrontWheel = false;
            connectionPointCS0 = new Vector3(-RearWheelLocation.Y, SuspensionHeight, -RearWheelLocation.X);
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, isFrontWheel);

            connectionPointCS0 = new Vector3(RearWheelLocation.Y, SuspensionHeight, -RearWheelLocation.X);
            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, SuspensionRestLength, WheelRadius, tuning, isFrontWheel);

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

        public override void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            
            trans = vehicleBody.CenterOfMassTransform * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Resources.DrawMesh(vehicleMesh);
            SteeringWheel.Draw(vehicleBody.CenterOfMassTransform);
            if (CurrentCamera == 0)
            {
                VelocityNeedle.Draw(vehicleBody.CenterOfMassTransform);
                RPMNeedle.Draw(vehicleBody.CenterOfMassTransform);
            }
            

            Matrix4 wheel;
            wheel = raycastVehicle.GetWheelTransformWS(0)*World.WorldMatrix;
            GL.LoadMatrix(ref wheel);
            Resources.DrawMesh(WheelMesh);
            wheel = Matrix4.CreateRotationY((float)MathHelper.Pi);
            wheel *= raycastVehicle.GetWheelTransformWS(1) * World.WorldMatrix;
            GL.LoadMatrix(ref wheel);
            Resources.DrawMesh(WheelMesh);
            wheel = Matrix4.CreateRotationY((float)MathHelper.Pi);
            wheel *= raycastVehicle.GetWheelTransformWS(2) * World.WorldMatrix;
            GL.LoadMatrix(ref wheel);
            Resources.DrawMesh(WheelMesh);
            wheel = raycastVehicle.GetWheelTransformWS(3) * World.WorldMatrix;
            GL.LoadMatrix(ref wheel);
            Resources.DrawMesh(WheelMesh);
        }

        public override void Update()
        {
            base.Update();
            raycastVehicle.ApplyEngineForce(Thrust * 5000, 2);
            raycastVehicle.ApplyEngineForce(Thrust * 5000, 3);
            raycastVehicle.SetSteeringValue(Steering, 0);
            raycastVehicle.SetSteeringValue(Steering, 1);
            //raycastVehicle.SetBrake(Brake*100, 0);
            //raycastVehicle.SetBrake(Brake*100, 1);
            raycastVehicle.SetBrake(Brake*400, 2);
            raycastVehicle.SetBrake(Brake*400, 3);

            SteeringWheel.MeshRotation = new Vector3(SteeringWheel.MeshRotation.X, SteeringWheel.MeshRotation.Y, -5*Steering);
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
        }
    }
}

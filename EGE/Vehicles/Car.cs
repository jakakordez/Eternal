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
        public Vector2 SteeringWheelAngle, FrontWheelLocation, RearWheelLocation;
        public float WheelRadius, WheelWidth, WheelFriction, SuspensionStiffness, SuspensionDamping, SuspensionCompression, RollInfluence, SuspensionHeight, SuspensionRestLength;
        public Vector3 wheelDirectionCS0 = new Vector3(0, -1, 0);
        public Vector3 wheelAxleCS = new Vector3(-1, 0, 0);

        public const int rightIndex = 0;
        public const int upIndex = 1;
        public const int forwardIndex = 2;

        public void Load()
        {

            WheelRadius = 0.41f;
            WheelWidth = 0.31f;
            WheelFriction = 2000;
            SuspensionStiffness = 800;
            SuspensionDamping = 20.3f;
            SuspensionCompression = 4000.4f;
            RollInfluence = 0.1f;
            SuspensionHeight = -0.2f;
            SuspensionRestLength = 0.4f;
            SteeringWheelAngle = new Vector2(-8, 0.4f);
            FrontWheelLocation = new Vector2(1.48f, 0.7f);
            RearWheelLocation = new Vector2(1.35f, 0.7f);


            Vector3 Dimensions = new Vector3(4.64f, 2.01f, 1.38f);

            CollisionShape chassisShape = new BoxShape(Dimensions.Y / 2, Dimensions.Z / 2, Dimensions.X / 2);
            collisionShape = new CompoundShape();
            //localTrans effectively shifts the center of mass with respect to the chassis
            Matrix4 localTrans = Matrix4.CreateTranslation(0 * Vector3.UnitY);
            ((CompoundShape)collisionShape).AddChildShape(localTrans, chassisShape);

            vehicleBody = World.CreateRigidBody(1505, Matrix4.CreateTranslation(new Vector3(693, 10, 284)), collisionShape);
            
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
        }

        public void Draw()
        {
            Matrix4 trans = World.WorldMatrix;
            
            trans = Matrix4.CreateTranslation(vehicleBody.CenterOfMassPosition) * World.WorldMatrix;
            GL.LoadMatrix(ref trans);
            Tools.MeshManager.DrawMesh("exterior");
        }
    }
}

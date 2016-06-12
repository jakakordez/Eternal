using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Meshes;
using BulletSharp;
using OpenTK;

namespace EGE.Vehicles
{
    public class Vehicle:ICloneable
    {
        public enum VehicleController
        {
            Player,
            AI,
            Network
        }

        public VehicleController ControllerType;
        public MeshReference VehicleMesh { get; set; }
        public string vehicleMesh { get; set; }
        public string lowPolyVehicleMesh { get; set; }

        public RigidBody vehicleBody;

        public Camera FirstPersonCamera { get; set; }
        public Camera ThirdPersonCamera { get; set; }
        public Camera[] CameraList { get; set; }

        public float Mass { get; set; }

        protected int CurrentCamera;

        public Vehicle()
        {
            CameraDefinition defaultCameraDefinition = new CameraDefinition()
            {
                Distance = 10,
                FPV = true,
                Offset = Vector3.Zero,
                ViewAngle = Vector2.One,
                Style = DrawingStyle.Normal
            };
            FirstPersonCamera = new FirstPersonCamera(defaultCameraDefinition);
            ThirdPersonCamera = new ThirdPersonCamera(defaultCameraDefinition);
            CameraList = new Camera[0];
            vehicleMesh = "";
            lowPolyVehicleMesh = "";
            CurrentCamera = 1;
            VehicleMesh = new MeshReference();
        }

        public virtual void Draw(Vector3 eye)
        {
        }

        public virtual void Update() { }

        public virtual void Load(Vector3 center) { }

        public virtual void HandleInput() { }

        protected virtual void HandleAI() { }

        public void NextCamera()
        {
            CurrentCamera = (CurrentCamera + 1) % (CameraList.Length + 2);
            
        }

        public void UpdateCamera()
        {
            if (CurrentCamera == 0) FirstPersonCamera.Update();
            else if (CurrentCamera == 1) ThirdPersonCamera.Update();
            else CameraList[CurrentCamera - 2].Update();
        }

        public void DrawCamera()
        {
            if (CurrentCamera == 0) FirstPersonCamera.GenerateLookAt(vehicleBody.CenterOfMassTransform);
            else if (CurrentCamera == 1) ThirdPersonCamera.GenerateLookAt(vehicleBody.CenterOfMassPosition);
            else CameraList[CurrentCamera - 2].GenerateLookAt(vehicleBody.CenterOfMassPosition);
        }

        public static Vehicle VehicleFromString(string type)
        {
            switch (type){
                case "Car": return new Car();
            }
            return new Vehicle();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

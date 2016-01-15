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
    public class Vehicle
    {
        public enum VehicleController
        {
            Player,
            AI,
            Network
        }

        public VehicleController ControllerType;

        protected string vehicleMesh = "meshes/cars/bmw/420d/exterior";

        public RigidBody vehicleBody;

        public Camera[] CameraList;

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
            CameraList = new Camera[] { new FirstPersonCamera(defaultCameraDefinition), new ThirdPersonCamera(defaultCameraDefinition) };
        }

        public virtual void Draw()
        {

        }

        public virtual void Update()
        {
           
        }

        public virtual void HandleInput()
        {

        }

        protected virtual void HandleAI()
        {

        }

        public static Vehicle VehicleFromString(string type)
        {
            switch (type){
                case "Car": return new Car();
            }
            return new Vehicle();
        }
    }
}

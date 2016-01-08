using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Meshes;
using BulletSharp;

namespace EGE.Vehicles
{
    class Vehicle
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
    }
}

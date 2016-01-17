using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Vehicles
{
    class RoadVehicle:Vehicle
    {
        protected float Thrust, Brake, Steering;
        public float SteeringClamp { get; set; }

        public string InteriorMesh { get; set; }

        public string WheelMesh { get; set; }

        public RoadVehicle()
        {
            WheelMesh = "";
            InteriorMesh = "";
        }
    }
}

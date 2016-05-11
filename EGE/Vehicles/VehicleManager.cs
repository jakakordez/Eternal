using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Vehicles
{
    public class VehicleManager
    {
        public VehicleCollection Vehicles { get; set; }
        public VehicleInstanceCollection VehicleInstances;

        public VehicleManager()
        {
            Vehicles = new VehicleCollection();
            VehicleInstances = new VehicleInstanceCollection();
        }

        public void Draw(Vector3 eye)
        {
            foreach (var obref in VehicleInstances.GetNodes())
            {
                ((Vehicle)obref.Value).Draw(eye);
            }
        }

        public string spawnVehicle(string name, Vector3 Location)
        {
            Vehicle a = (Vehicle)((Vehicle)Vehicles.Get(name)).Clone();
            a.Load(Location);
            return VehicleInstances.Add(a);
        }
    }
}

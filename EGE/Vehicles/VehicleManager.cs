using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Vehicles
{
    public class VehicleManager
    {
        public VehicleCollection Vehicles { get; set; }
        public VehicleInstanceCollection VehicleInstances { get; set; }

        public VehicleManager()
        {
            Vehicles = new VehicleCollection();
            VehicleInstances = new VehicleInstanceCollection();
        }

        public void Draw()
        {
            foreach (var obref in VehicleInstances.GetNodes())
            {
                ((Vehicle)obref.Value).Draw();
            }
        }
    }
}

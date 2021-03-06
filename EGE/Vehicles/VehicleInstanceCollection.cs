﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EGE.Vehicles
{
    public class VehicleInstanceCollection : Tools.NodeCollection
    {
        public VehicleInstanceCollection():base(true, typeof(Vehicle))
        {

        }

        public static Dictionary<string, object> Deserialize(string data)
        {
            Dictionary<string, Vehicle> tmp = new Dictionary<string, Vehicle>();
            Newtonsoft.Json.JsonConvert.PopulateObject(data, tmp, Global.SerializerSettings);
            return tmp.ToDictionary(item => item.Key, item => (object)item.Value);
        }

        public string nearestVehicle(Vector3 location)
        {
            return storage.OrderBy(s => (((Vehicle)s.Value).vehicleBody.CenterOfMassPosition-location).LengthSquared).First().Key;
        }
    }
}

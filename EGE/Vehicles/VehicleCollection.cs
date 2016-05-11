using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Vehicles
{
    public class VehicleCollection : Tools.NodeCollection
    {
        public VehicleCollection():base(false, typeof(Vehicle))
        {
            AvaliableTypes = new List<Type>(new Type[] { typeof(Car), typeof(Ship)});
        }

        public static Dictionary<string, object> Deserialize(string data)
        {
            Dictionary<string, Vehicle> tmp = new Dictionary<string, Vehicle>();
            Newtonsoft.Json.JsonConvert.PopulateObject(data, tmp, Global.SerializerSettings);
            return tmp.ToDictionary(item => item.Key, item => (object)item.Value);
        }
    }
}

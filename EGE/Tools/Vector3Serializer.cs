using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK;

namespace EGE.Tools
{
    public class Vector3Serializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 obj = (Vector3)value;
            writer.WriteValue(obj.X.ToString(serializer.Culture) + ";" + obj.Y.ToString(serializer.Culture) + ";" + obj.Z.ToString(serializer.Culture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
           
            string[] components = reader.Value.ToString().Split(';');
            Vector3 Value = new Vector3(Misc.toFloat(components[0]), Misc.toFloat(components[1]), Misc.toFloat(components[2]));
            return Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Vector3).IsAssignableFrom(objectType);
        }
    }
}

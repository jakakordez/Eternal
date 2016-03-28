using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EGE.Tools
{
    class CollectionSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(JsonConvert.SerializeObject(((NodeCollection)value).storage, Global.SerializerSettings));
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = objectType.GetMethod("Deserialize").Invoke(existingValue, new object[] { reader.Value.ToString() });
            ((NodeCollection)existingValue).Load((Dictionary<string, object>)result);
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(NodeCollection).IsAssignableFrom(objectType);
        }
    }
}

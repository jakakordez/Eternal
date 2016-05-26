using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EGE
{
    static class Global
    {
        public static Encoding Encoding = Encoding.UTF8;
        public static string Version = "0.0.0";
        public static Random RNG = new Random();

        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter> { new Tools.VectorSerializer(), new Tools.CollectionSerializer() },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            TypeNameHandling = TypeNameHandling.Auto
        };

        
    }
}

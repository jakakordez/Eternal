﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace EGE.Tools
{
    public class VectorSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value.GetType() == typeof(Color4))
            {
                Color4 obj = (Color4)value;
                writer.WriteValue((obj.R*255f).ToString(serializer.Culture) + ";" + (obj.G*255f).ToString(serializer.Culture) + ";" + (obj.B*255f).ToString(serializer.Culture));
            }
            else if(value.GetType() == typeof(Vector3))
            {
                Vector3 obj = (Vector3)value;
                writer.WriteValue(obj.X.ToString(serializer.Culture) + ";" + obj.Y.ToString(serializer.Culture) + ";" + obj.Z.ToString(serializer.Culture));
            }
            else
            {
                Vector2 obj = (Vector2)value;
                writer.WriteValue(obj.X.ToString(serializer.Culture) + ";" + obj.Y.ToString(serializer.Culture));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string[] components = reader.Value.ToString().Split(';');
            if (objectType == typeof(Color4))
            {
                return new Color4(Misc.toFloat(components[0])/ 255f, Misc.toFloat(components[1]) / 255f, Misc.toFloat(components[2]) / 255f, 255);
            }
            else if (objectType == typeof(Vector3))
            {
                return new Vector3(Misc.toFloat(components[0]), Misc.toFloat(components[1]), Misc.toFloat(components[2]));
            }
            else
            {
                return new Vector2(Misc.toFloat(components[0]), Misc.toFloat(components[1]));
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Vector3).IsAssignableFrom(objectType) 
                || typeof(Vector2).IsAssignableFrom(objectType)
                || typeof(Color4).IsAssignableFrom(objectType);
        }
    }
}

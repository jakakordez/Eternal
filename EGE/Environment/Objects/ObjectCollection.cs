﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment
{
    public class ObjectCollection : Tools.NodeCollection
    {
        public ObjectCollection():base(false, typeof(Object))
        {
        }

        public static Dictionary<string, object> Deserialize(string data)
        {
            Dictionary<string, Object> tmp = new Dictionary<string, Object>();
            Newtonsoft.Json.JsonConvert.PopulateObject(data, tmp, Global.SerializerSettings);
            return tmp.ToDictionary(item => item.Key, item => (object)item.Value);
        }
    }
}

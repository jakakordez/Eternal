using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment
{
    public class ObjectCollection
    {
        Dictionary<string, Object> objectCollection;
        public ObjectCollection()
        {
            objectCollection = new Dictionary<string, Object>();
        }
        
        public Object Get(string id)
        {
            return objectCollection[id];
        }

        public void Add(string id, Object o)
        {
            objectCollection.Add(id, o);
        }

        public void Remove(string id)
        {
            objectCollection.Remove(id);
        }

        public KeyValuePair<string, object>[] GetNodes()
        {
            return objectCollection.Select(t => new KeyValuePair<string, object>(t.Key, (object)t.Value)).ToArray();
        }

        public void Set(string id, Object val)
        {
            objectCollection[id] = val;
        }

        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(objectCollection, Global.SerializerSettings);
        }

        public void Deserialize(string data)
        {
            Newtonsoft.Json.JsonConvert.PopulateObject(data, objectCollection, Global.SerializerSettings);
        }
    }
}

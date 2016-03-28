using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment
{
    public class ObjectReferenceCollection : Tools.NodeCollection
    {
        public ObjectReferenceCollection():base(true, typeof(ObjectReference))
        {

        }

        public static Dictionary<string, object> Deserialize(string data)
        {
            Dictionary<string, ObjectReference> tmp = new Dictionary<string, ObjectReference>();
            Newtonsoft.Json.JsonConvert.PopulateObject(data, tmp, Global.SerializerSettings);
            return tmp.ToDictionary(item => item.Key, item => (object)item.Value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Tools
{
    public class NodeCollection
    {
        public Dictionary<string, object> storage;
        ulong key;
        public bool AutoIncrement;
        public Type contentType;

        public NodeCollection(bool AI, Type contentType)
        {
            AutoIncrement = AI;
            this.contentType = contentType;
            storage = new Dictionary<string, object>();
        }

        public object Get(string id)
        {
            return storage[id];
        }

        public void Add(string id, object o)
        {
            storage.Add(id, o);
        }
        public string Add(object o)
        {
            string keystring = getHash(key);
            while (storage.ContainsKey(keystring)) 
            {
                keystring = getHash(++key);
            }
            storage.Add(keystring, o);
            return key.ToString();
        }

        public void Remove(string id)
        {
            storage.Remove(id);
        }

        public KeyValuePair<string, object>[] GetNodes()
        {
            return storage.Select(t => new KeyValuePair<string, object>(t.Key, (object)t.Value)).ToArray();
        }

        public void Set(string id, object val)
        {
            storage[id] = val;
        }

        public void Load(Dictionary<string, object> data)
        {
            storage = data;
        }

        public static string getHash(ulong data)
        {
            string result = "";
            char[] characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            do
            {
                result = characters[data % (ulong)characters.Length]+result;
                data /= (ulong)characters.Length;
            }
            while (data > 0);
            return result;
        }
    }
}

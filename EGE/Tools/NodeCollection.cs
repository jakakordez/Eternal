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
            if(storage.ContainsKey(id)) return storage[id];
            return null;
        }

        public bool Contains(string id)
        {
            return storage.ContainsKey(id);
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
            return storage.Select(t => new KeyValuePair<string, object>(t.Key, t.Value))
                          .ToArray();
        }

        public KeyValuePair<string, object>[] GetNodes(string path)
        {
            int semicolons = path.Count(t => t == ';');
            return storage.Where(t => t.Key.Count(s => s == ';') == semicolons)
                          .Where(t => t.Key.StartsWith(path))
                          .Select(t => new KeyValuePair<string, object>(t.Key.Split(';')[semicolons], t.Value))
                          .OrderBy(t => t.Key)
                          .ToArray();
        }

        public string[] GetDirectories(string path)
        {
            int semicolons = path.Count(t => t == ';');
            return storage.Where(t => t.Key.Count(s => s == ';') > semicolons)
                          .Where(t => t.Key.StartsWith(path))
                          .Select(t => t.Key.Split(';')[semicolons])
                          .Distinct()
                          .OrderBy(t => t)
                          .ToArray();
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

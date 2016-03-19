using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment.Paths
{
    public class PathEndpoint
    {
        ulong ObjectID { get; set; }
        int NodeID { get; set; }

        public PathEndpoint() { }

        public PathEndpoint(ulong objectID, int node)
        {
            ObjectID = objectID;
            NodeID = node;
        }
    }
}

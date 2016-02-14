using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment.Paths
{
    public class JunctionReference
    {
        public NodeReference Location { get; set; }
        public ulong ID { get; set; }

        public JunctionReference()
        {
            Location = new NodeReference();
        }
    }
}

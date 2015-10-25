using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Environment.Paths
{
    public class Road
    {
        public Path RoadPath { get; set; }
        public string test { get; set; }

        public Road()
        {
            RoadPath = new Path();
            test = "";
        }
    }
}

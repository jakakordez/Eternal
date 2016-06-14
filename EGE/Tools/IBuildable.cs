using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGE.Environment;

namespace EGE
{
    public interface IBuildable
    {
        void Build(Map currentMap);
    }
}

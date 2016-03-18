﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Tools
{
    class Contruction
    {
        public static string MapPath;

        public static void Load(string Path, Map CurrentMap)
        {
            MapPath = Path;
            new Environment.Nodes().LoadNodes(Path + "\\Nodes.ege");
            CurrentMap.Load(Path+"\\Map.ege");
        }

        public static void Save(string Path, Map CurrentMap)
        {
            MapPath = Path;
            CurrentMap.Save(Path+"\\Map.ege");
            new Environment.Nodes().SaveNodes(Path + "\\Nodes.ege");
        }
    }
}

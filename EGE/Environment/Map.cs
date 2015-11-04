using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE
{
    public class Map
    {
        public Terrain CurrentTerrain { get; set; }

        public Map()
        {
            CurrentTerrain = new Terrain();
        }

        public void Load()
        {
            CurrentTerrain.Load(Tools.Contruction.MapPath + "\\Terrain.ege");
        }

        public void Save()
        {
            CurrentTerrain.Save(Tools.Contruction.MapPath + "\\Terrain.ege");
        }

        public override string ToString()
        {
            return "Map";
        }
    }
}

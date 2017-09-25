using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.FileSystem;
using Engine.Maps;

namespace Engine
{
    public sealed class ModData : IDisposable
    {
        public readonly Manifest Manifest;

        public readonly ObjectCreator ObjectCreator;


        public ModData(Manifest mod, bool useLoadScreen = false)
        {
            this.Manifest = mod;
            this.ObjectCreator = new ObjectCreator();
            // Take a local copy of the manifest
            
        }

        public Map PrepareMap(string uid)
        {
            string mapDir = Platform.ModsDir + Path.DirectorySeparatorChar + "ra" + Path.DirectorySeparatorChar +
                          Path.Combine("maps", uid);
            Map map = YamlHelper.Deserialize<Map>(mapDir + Path.DirectorySeparatorChar + "map.yaml");
            
            return map;
        }

        public void Dispose()
        {
        }
    }
}

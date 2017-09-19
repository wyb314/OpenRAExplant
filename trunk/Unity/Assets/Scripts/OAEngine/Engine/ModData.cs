using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return new Map();
        }

        public void Dispose()
        {
        }
    }
}

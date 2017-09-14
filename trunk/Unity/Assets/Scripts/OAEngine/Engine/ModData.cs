using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OAEngine.Engine;

namespace Engine
{
    public sealed class ModData : IDisposable
    {
        public readonly Manifest Manifest;

        public readonly ObjectCreator ObjectCreator;

        public Map PrepareMap(string uid)
        {
            return new Map();
        }

        public void Dispose()
        {
        }
    }
}

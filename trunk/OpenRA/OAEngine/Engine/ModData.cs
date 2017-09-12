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

        public void Dispose()
        {
        }
    }
}

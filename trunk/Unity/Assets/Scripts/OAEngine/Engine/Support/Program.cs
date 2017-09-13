using System;
using System.Collections.Generic;
using Engine;
using Engine.Support;

namespace Engine.Support
{
   

    public sealed class Program
    {
        public static RunStatus Run(string[] args ,IPlatformImpl platformInfo = null)
        {
            Game.Initialize(new Arguments(args), platformInfo);
            GC.Collect();
            return Game.Run();
        }
    }
}

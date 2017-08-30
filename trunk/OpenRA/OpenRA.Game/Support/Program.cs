using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA
{
   

    static class Program
    {
        public static RunStatus Run(string[] args)
        {
            Game.Initialize(new Arguments(args));
            GC.Collect();
            return Game.Run();
        }
    }
}

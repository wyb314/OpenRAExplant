using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRA.Support;

namespace OpenRA
{
   

    public sealed class Program
    {
        public static RunStatus Run(string[] args ,IPlatformImpl platformInfo = null)
        {
            Game.Initialize(new Arguments(args), platformInfo);
            GC.Collect();
            //return RunStatus.Error;
            return Game.Run();
        }
    }
}

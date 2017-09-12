

using System;

namespace Engine.Network.Interfaces
{
    public interface IGameInformation
    {
        string Mod { set; get; }

        string Version { set; get; }
        
        string MapUid { set; get; }

        string MapTitle { set; get; }

        DateTime StartTimeUtc { set; get; }
        
        DateTime EndTimeUtc { set; get; }



    }
}

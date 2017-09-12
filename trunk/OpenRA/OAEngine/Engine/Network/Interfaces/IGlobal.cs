

namespace Engine.Network.Interfaces
{
    public interface IGlobal
    {
        string ServerName { set; get; }

        string Map { set; get; }

        int Timestep { set; get; }

        int OrderLatency { set; get; }
        
        int RandomSeed { set; get; }
        
        bool AllowSpectators { set; get; }

        bool AllowVersionMismatch { set; get; }

        string GameUid { set; get; }
        
        bool EnableSingleplayer { set; get; }
    }
}

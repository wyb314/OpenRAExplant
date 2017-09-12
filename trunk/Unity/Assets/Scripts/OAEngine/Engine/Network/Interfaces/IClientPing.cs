

namespace Engine.Network.Interfaces
{
    public interface IClientPing
    {
        int Index { set; get; }

        long Latency { set; get; }

        long LatencyJitter { set; get; }
        
        long[] LatencyHistory { set; get; }
    }
}

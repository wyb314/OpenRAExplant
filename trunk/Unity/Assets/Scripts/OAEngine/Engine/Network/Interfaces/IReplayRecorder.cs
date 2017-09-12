
using System;

namespace Engine.Network.Interfaces
{
    public interface IReplayRecorder : IDisposable
    {
        Func<string> chooseFilename { set; get; }

        void Receive(int clientID, byte[] data);
    }
}

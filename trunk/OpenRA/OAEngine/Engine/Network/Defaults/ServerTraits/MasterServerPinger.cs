using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;
using Engine.Network.Server;
using Engine.Support;

namespace Engine.Network.Defaults.ServerTraits
{
    public class MasterServerPinger : ServerTrait,
        ITick<ClientDefault>,
        INotifyServerStart<ClientDefault>,
        INotifySyncLobbyInfo<ClientDefault>,
        IStartGame<ClientDefault>,
        IEndGame<ClientDefault>
    {
        // 3 minutes. Server has a 5 minute TTL for games, so give ourselves a bit of leeway.
        const int MasterPingInterval = 60 * 3;
        //static readonly Beacon LanGameBeacon;
        static readonly Dictionary<int, string> MasterServerErrors = new Dictionary<int, string>()
        {
            { 1, "Server port is not accessible from the internet." },
            { 2, "Server name contains a blacklisted word." }
        };

        public int TickTimeout { get { return MasterPingInterval * 10000; } }

        long lastPing = 0;
        bool isInitialPing = true;

        volatile bool isBusy;
        Queue<string> masterServerMessages = new Queue<string>();

        public MasterServerPinger()
        {
        }


        public void Tick(IServer<ClientDefault> server)
        {
        }
        
        
        public void GameEnded(IServer<ClientDefault> server)
        {
        }

        public void GameStarted(IServer<ClientDefault> server)
        {
        }

        public void LobbyInfoSynced(IServer<ClientDefault> server)
        {
        }

        public void ServerStarted(IServer<ClientDefault> server)
        {
        }

       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class FrameDataDefault : IFrameData<ClientDefault>
    {
        public Dictionary<int, int> clientQuitTimes { private set; get; }

        public Dictionary<int, Dictionary<int, byte[]>> framePackets { private set; get; }

        public FrameDataDefault()
        {
            this.clientQuitTimes = new Dictionary<int, int>();
            this.framePackets = new Dictionary<int, Dictionary<int, byte[]>>();
        }


        public void AddFrameOrders(int clientId, int frame, byte[] orders)
        {
            var frameData = framePackets.GetOrAdd(frame);
            frameData.Add(clientId, orders);
        }

        public void ClientQuit(int clientId, int lastClientFrame)
        {
            if (lastClientFrame == -1)
                lastClientFrame = framePackets
                    .Where(x => x.Value.ContainsKey(clientId))
                    .Select(x => x.Key).OrderBy(x => x).LastOrDefault();

            clientQuitTimes[clientId] = lastClientFrame;
        }

        public bool IsReadyForFrame(int frame)
        {
            bool result = ClientsNotReadyForFrame(frame).Any();
            return !result;
        }

        public IEnumerable<int> ClientsNotReadyForFrame(int frame)
        {
            var frameData = framePackets.GetOrAdd(frame);

            IEnumerable<int> result = ClientsPlayingInFrame(frame)
                .Where(client => !frameData.ContainsKey(client));
            int count = result.Count();
            return result;
        }

        public IEnumerable<int> ClientsPlayingInFrame(int frame)
        {
            IEnumerable<int> result = clientQuitTimes
                .Where(x => frame <= x.Value)
                .Select(x => x.Key)
                .OrderBy(x => x);

            int count = result.Count();

            return result;
        }

        public IEnumerable<ClientOrder> OrdersForFrame(IOrderManager<ClientDefault> orderManager, INetWorld world, int frame)
        {
            var frameData = framePackets[frame];
            var clientData = ClientsPlayingInFrame(frame)
                .ToDictionary(k => k, v => frameData[v]);

            return clientData
                .SelectMany(x => orderManager.orderSerializer.Deserialize(world,x.Value)
                    .Select(o => new ClientOrder { Client = x.Key, Order = o }));
        }
        
    }
}

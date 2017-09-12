using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;
using Engine.Primitives;

namespace Engine.Network.Defaults
{
    public class OrderManagerDefault: IOrderManager
    {
        public string Host { private set;get; }

        public int Port { private set;get; }

        public string Password { private set;get; }

        public int NetFrameNumber { private set;get; }

        public int LocalFrameNumber { set;get; }

        public int FramesAhead { private set ; get; }

        public IConnection Connection { private set;get; }

        public List<IOrder> localOrders { private set;get; }

        public IOrderSerializer orderSerializer { private set;get; }

        public IOrderProcessor orderProcessor { private set;get; }

        public IFrameData frameData { private set; get; }

        public ISyncReport syncReport { private set;get; }

        public INetWorld World {set;get; }

        public bool IsReadyForNextFrame
        {
            get
            {
                bool result = frameData.IsReadyForFrame(NetFrameNumber);

                return NetFrameNumber >= 1 && result;
            }
        }

        public bool GameStarted { get { return NetFrameNumber != 0; } }

        public OrderManagerDefault(string host, int port, string password, IConnection conn)
        {
            Host = host;
            Port = port;
            Password = password;
            Connection = conn;
            syncReport = new SyncReportDefault();
        }


        public void StartGame()
        {
            if (GameStarted) return;

            NetFrameNumber = 1;
            for (var i = NetFrameNumber; i <= FramesAhead; i++)
                Connection.Send(i, new List<byte[]>());
        }


        public void IssueOrders(IOrder[] orders)
        {
            foreach (var order in orders)
                IssueOrder(order);
        }


        public void IssueOrder(IOrder order)
        {
            this.localOrders.Add(order);
        }

        Dictionary<int, byte[]> syncForFrame = new Dictionary<int, byte[]>();

        void CheckSync(byte[] packet)
        {
            var frame = BitConverter.ToInt32(packet, 0);
            byte[] existingSync;
            if (syncForFrame.TryGetValue(frame, out existingSync))
            {
                if (packet.Length != existingSync.Length)
                    OutOfSync(frame);
                else
                    for (var i = 0; i < packet.Length; i++)
                        if (packet[i] != existingSync[i])
                            OutOfSync(frame);
            }
            else
                syncForFrame.Add(frame, packet);
        }

        void OutOfSync(int frame)
        {
            syncReport.DumpSyncReport(frame, frameData.OrdersForFrame(this,World, frame));
            throw new InvalidOperationException("Out of sync in frame {0}.\n Compare syncreport.log with other players.".F(frame));
        }

        public void TickImmediate()
        {
            var immediateOrders = localOrders.Where(o => o.IsImmediate).ToList();
            if (immediateOrders.Count != 0)
                Connection.SendImmediate(immediateOrders.Select(o => o.Serialize()).ToList());
            localOrders.RemoveAll(o => o.IsImmediate);

            var immediatePackets = new List<Pair<int, byte[]>>();

            Connection.Receive(
                (clientId, packet) =>
                {
                    var frame = BitConverter.ToInt32(packet, 0);
                    if (packet.Length == 5 && packet[4] == 0xBF)
                        frameData.ClientQuit(clientId, frame);
                    else if (packet.Length >= 5 && packet[4] == 0x65)
                        CheckSync(packet);
                    else if (frame == 0)
                        immediatePackets.Add(Pair.New(clientId, packet));
                    else
                        frameData.AddFrameOrders(clientId, frame, packet);
                });

            foreach (var p in immediatePackets)
            {
                foreach (var o in this.orderSerializer.Deserialize(World,p.Second))
                {
                    this.orderProcessor.ProcessOrder(this,this.World,p.First,o);
                    // A mod switch or other event has pulled the ground from beneath us
                    if (disposed)
                        return;
                }
            }
        }

        public void Tick()
        {
            if (!IsReadyForNextFrame)
                throw new InvalidOperationException();

            Connection.Send(NetFrameNumber + FramesAhead, localOrders.Select(o => o.Serialize()).ToList());
            localOrders.Clear();

            foreach (var order in frameData.OrdersForFrame(this,World, NetFrameNumber))
            {
                //Log.Write("wyb", "process order Client->{0} orderStr->{1} netFrameNum->{2}".F(order.Client, order.Order.OrderString, NetFrameNumber));
                this.orderProcessor.ProcessOrder(this, World, order.Client, order.Order);
            }
            
            Connection.SendSync(NetFrameNumber, OrderIO.SerializeSync(World.SyncHash()));

            syncReport.UpdateSyncReport();

            ++NetFrameNumber;
        }

        bool disposed;

        public void Dispose()
        {
            disposed = true;
            if (Connection != null)
                Connection.Dispose();
        }
    }
}

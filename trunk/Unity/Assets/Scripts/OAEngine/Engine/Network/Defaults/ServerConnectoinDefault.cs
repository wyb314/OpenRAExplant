using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Engine.Network.Interfaces;
using Engine.Support;

namespace Engine.Network.Defaults
{
    public enum ReceiveState { Header, Data }
    public class ServerConnectoinDefault : IServerConnectoin<ClientDefault>
    {
        public const int MaxOrderLength = 131072;

        public int PlayerIndex { set; get; }

        public int MostRecentFrame { private set;get; }

        public Socket Socket { set; get; }

        public List<byte> Data = new List<byte>();

        public ReceiveState State = ReceiveState.Header;
        public int ExpectLength = 8;
        public int Frame = 0;

        public long TimeSinceLastResponse { get { return Game.RunTime - lastReceivedTime; } }
        public bool TimeoutMessageShown { set; get; }
        long lastReceivedTime = 0;

        public byte[] PopBytes(int n)
        {
            var result = Data.GetRange(0, n);
            Data.RemoveRange(0, n);
            return result.ToArray();
        }

        bool ReadDataInner(IServer<ClientDefault> server)
        {
            var rx = new byte[1024];
            var len = 0;

            for (;;)
            {
                try
                {
                    // Poll the socket first to see if there's anything there.
                    // This avoids the exception with SocketErrorCode == `SocketError.WouldBlock` thrown
                    // from `socket.Receive(rx)`.
                    if (!Socket.Poll(0, SelectMode.SelectRead)) break;

                    if (0 < (len = Socket.Receive(rx)))
                        Data.AddRange(rx.Take(len));
                    else
                    {
                        if (len == 0)
                            server.DropClient(this);
                        break;
                    }
                }
                catch (SocketException e)
                {
                    // This should no longer be needed with the socket.Poll call above.
                    if (e.SocketErrorCode == SocketError.WouldBlock) break;

                    server.DropClient(this);
                    Log.Write("server", "Dropping client {0} because reading the data failed: {1}", PlayerIndex, e);
                    return false;
                }
            }

            lastReceivedTime = Game.RunTime;
            TimeoutMessageShown = false;

            return true;
        }

        public void ReadData(IServer<ClientDefault> server)
        {
            if (ReadDataInner(server))
                while (Data.Count >= ExpectLength)
                {
                    var bytes = PopBytes(ExpectLength);
                    switch (State)
                    {
                        case ReceiveState.Header:
                            {
                                ExpectLength = BitConverter.ToInt32(bytes, 0) - 4;
                                Frame = BitConverter.ToInt32(bytes, 4);
                                State = ReceiveState.Data;

                                if (ExpectLength < 0 || ExpectLength > MaxOrderLength)
                                {
                                    server.DropClient(this);
                                    Log.Write("server", "Dropping client {0} for excessive order length = {1}", PlayerIndex, ExpectLength);
                                    return;
                                }
                            }
                            break;

                        case ReceiveState.Data:
                            {
                                if (MostRecentFrame < Frame)
                                    MostRecentFrame = Frame;

                                server.DispatchOrders(this, Frame, bytes);
                                ExpectLength = 8;
                                State = ReceiveState.Header;
                            }
                            break;
                    }
                }
        }
    }
}

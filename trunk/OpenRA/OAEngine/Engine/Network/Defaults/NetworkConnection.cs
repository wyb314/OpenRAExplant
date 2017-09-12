using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Engine.Network.Server;

namespace Engine.Network.Defaults
{
    public class NetworkConnection : EchoConnection
    {
        readonly TcpClient tcp;
        readonly List<byte[]> queuedSyncPackets = new List<byte[]>();
        volatile ConnectionState connectionState = ConnectionState.Connecting;
        volatile int clientId;
        bool disposed;

        public NetworkConnection(string host, int port)
        {
            try
            {
                tcp = new TcpClient(host, port) { NoDelay = true };
                new Thread(NetworkConnectionReceive)
                {
                    Name = GetType().Name + " " + host + ":" + port,
                    IsBackground = true
                }.Start(tcp.GetStream());
            }
            catch
            {
                connectionState = ConnectionState.NotConnected;
            }
        }

        void NetworkConnectionReceive(object networkStreamObject)
        {
            try
            {
                var networkStream = (NetworkStream)networkStreamObject;
                var reader = new BinaryReader(networkStream);
                var serverProtocol = reader.ReadInt32();

                if (ProtocolVersion.Version != serverProtocol)
                    throw new InvalidOperationException(
                        "Protocol version mismatch. Server={0} Client={1}"
                            .F(serverProtocol, ProtocolVersion.Version));

                clientId = reader.ReadInt32();
                connectionState = ConnectionState.Connected;

                for (;;)
                {
                    var len = reader.ReadInt32();
                    var client = reader.ReadInt32();
                    var buf = reader.ReadBytes(len);
                    if (len == 0)
                        throw new NotImplementedException();
                    AddPacket(new ReceivedPacket { FromClient = client, Data = buf });
                }
            }
            catch { }
            finally
            {
                connectionState = ConnectionState.NotConnected;
            }
        }

        public override int LocalClientId { get { return clientId; } }
        public override ConnectionState ConnectionState { get { return connectionState; } }

        public override void SendSync(int frame, byte[] syncData)
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(frame));
            ms.Write(syncData);
            queuedSyncPackets.Add(ms.ToArray());
        }

        protected override void Send(byte[] packet)
        {
            base.Send(packet);

            try
            {
                var ms = new MemoryStream();
                ms.Write(BitConverter.GetBytes(packet.Length));
                ms.Write(packet);

                foreach (var q in queuedSyncPackets)
                {
                    ms.Write(BitConverter.GetBytes(q.Length));
                    ms.Write(q);
                    base.Send(q);
                }

                queuedSyncPackets.Clear();
                ms.WriteTo(tcp.GetStream());
            }
            catch (SocketException) { /* drop this on the floor; we'll pick up the disconnect from the reader thread */ }
            catch (ObjectDisposedException) { /* ditto */ }
            catch (InvalidOperationException) { /* ditto */ }
            catch (IOException) { /* ditto */ }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;

            // Closing the stream will cause any reads on the receiving thread to throw.
            // This will mark the connection as no longer connected and the thread will terminate cleanly.
            if (tcp != null)
                tcp.Close();

            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;
using System.IO;


namespace Engine.Network.Defaults
{
    public class EchoConnection : IConnection
    {
        protected struct ReceivedPacket
        {
            public int FromClient;
            public byte[] Data;
        }

        readonly List<ReceivedPacket> receivedPackets = new List<ReceivedPacket>();
        public IReplayRecorder Recorder { get; private set; }

        public virtual int LocalClientId
        {
            get { return 1; }
        }

        public virtual ConnectionState ConnectionState
        {
            get { return ConnectionState.PreConnecting; }
        }

        public virtual void Send(int frame, List<byte[]> orders)
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(frame));
            foreach (var o in orders)
                ms.Write(o);
            Send(ms.ToArray());
        }

        public virtual void SendImmediate(List<byte[]> orders)
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(0));
            foreach (var o in orders)
                ms.Write(o);
            Send(ms.ToArray());
        }

        public virtual void SendSync(int frame, byte[] syncData)
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(frame));
            ms.Write(syncData);
            Send(ms.ToArray());
        }

        protected virtual void Send(byte[] packet)
        {
            if (packet.Length == 0)
                throw new NotImplementedException();
            AddPacket(new ReceivedPacket { FromClient = LocalClientId, Data = packet });
        }

        protected void AddPacket(ReceivedPacket packet)
        {
            lock (receivedPackets)
                receivedPackets.Add(packet);
        }

        public virtual void Receive(Action<int, byte[]> packetFn)
        {
            ReceivedPacket[] packets;
            lock (receivedPackets)
            {
                packets = receivedPackets.ToArray();
                receivedPackets.Clear();
            }

            foreach (var p in packets)
            {
                packetFn(p.FromClient, p.Data);
                if (Recorder != null)
                    Recorder.Receive(p.FromClient, p.Data);
            }
        }

        public void StartRecording<T>(Func<string> chooseFilename) where T : IReplayRecorder , new ()
        {
            // If we have a previous recording then save/dispose it and start a new one.
            if (Recorder != null)
                Recorder.Dispose();
            Recorder = new T();
            Recorder.chooseFilename = chooseFilename;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Recorder != null)
                Recorder.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

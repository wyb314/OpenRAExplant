using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ClientDefault : IClient
    {
        public int Index { set; get; }

        private string name = "";

        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        private string ipAddress = "";

        public string IpAddress
        {
            set { this.ipAddress = value; }
            get { return this.ipAddress; }
        }

        public ClientState State { set; get; }

        private string bot = "";

        public string Bot
        {
            set { this.bot = value; }
            get { return this.bot; }
        }
        
        public bool IsAdmin { set; get; }

        private string slot = "";

        public string Slot
        {
            set
            {
                if (value == null)
                {
                    this.slot = string.Empty;
                }
                else
                {
                    this.slot = value;
                }
            }
            get { return this.slot; }
        }
        
        public int BotControllerClientIndex { set; get; }

        public bool IsReady { get { return State == ClientState.Ready; } }
        public bool IsInvalid { get { return State == ClientState.Invalid; } }

        public bool IsObserver { get { return string.IsNullOrEmpty(this.slot); } }

        public byte[] Serialize()
        {
            byte[] bytes = null;
            using (var ret = new MemoryStream())
            {
                var w = new BinaryWriter(ret);
                w.Write(this.Index);
                w.Write(this.Name);
                w.Write(this.IpAddress);
                w.Write((int)this.State);
                w.Write(this.Bot);
                w.Write(this.IsAdmin);
                w.Write(this.Slot);
                w.Write(this.BotControllerClientIndex);
                bytes = ret.ToArray();
            }
            return bytes;
        }

        public static ClientDefault Deserialize(byte[] data)
        {
            ClientDefault client = new ClientDefault();
            using (var ret = new MemoryStream(data))
            {
                var r = new BinaryReader(ret);
                client.Index = r.ReadInt32();
                client.Name = r.ReadString();
                client.IpAddress = r.ReadString();
                client.State = (ClientState) r.ReadInt32();
                client.Bot = r.ReadString();
                client.IsAdmin = r.ReadBoolean();
                client.Slot = r.ReadString();
                client.BotControllerClientIndex = r.ReadInt32();
            }
            return client;
        }

    }

    public static class ClientDefaultExts
    {
        public static byte[] WriteToBytes(this List<ClientDefault> y)
        {
            byte[] bytes = null;
            using (var mr = new MemoryStream())
            {
                var w = new BinaryWriter(mr);
                int count = y.Count;
                w.Write(count);
                foreach (var client in y)
                {
                    byte[] clientBytes = client.Serialize();
                    w.Write(clientBytes.Length);
                    w.Write(clientBytes);
                }
                bytes = mr.ToArray();
            }

            return bytes;
        }

        public static List<ClientDefault> ReadToClients(this byte[] bytes)
        {
            List<ClientDefault> clients = null;
            using (var mr = new MemoryStream(bytes))
            {
                var br = new BinaryReader(mr);
                int count = br.ReadInt32();
                clients = new List<ClientDefault>(count);
                
                for (int i = 0; i < count; i++)
                {
                    int clientBytesLength = br.ReadInt32();
                    ClientDefault client = ClientDefault.Deserialize(br.ReadBytes(clientBytesLength));
                    clients.Add(client);
                }
            }

            return clients;
        }
    }

    
}

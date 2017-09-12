using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.FileFormats;
using Engine.Network.Defaults;

namespace Engine.Network
{
    sealed class ReplayRecorder
    {
        public ReplayMetadata Metadata;
        BinaryWriter writer;
        Func<string> chooseFilename;
        MemoryStream preStartBuffer = new MemoryStream();


        public ReplayRecorder(Func<string> chooseFilename)
        {
            this.chooseFilename = chooseFilename;

            writer = new BinaryWriter(preStartBuffer);
        }
        
        static bool IsGameStart(byte[] data)
        {
            if (data.Length == 5 && data[4] == 0xbf)
                return false;
            if (data.Length >= 5 && data[4] == 0x65)
                return false;

            var frame = BitConverter.ToInt32(data, 0);
            return frame == 0 && data.ToOrderList(null).Any(o => o.OrderString == "StartGame");
        }


        bool disposed;

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            if (Metadata != null)
            {
                if (Metadata.GameInfo != null)
                    Metadata.GameInfo.EndTimeUtc = DateTime.UtcNow;
                Metadata.Write(writer);
            }

            if (preStartBuffer != null)
                preStartBuffer.Dispose();
            writer.Close();
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.FileFormats
{
    public class ReplayMetadata
    {

        public const int MetaStartMarker = -1;
        public const int MetaEndMarker = -2;
        public const int MetaVersion = 0x00000001;

        public readonly IGameInformation GameInfo;

        public void Write(BinaryWriter writer)
        {
            // Write start marker & version
            writer.Write(MetaStartMarker);
            writer.Write(MetaVersion);

            // Write data
            var dataLength = 0;
            {
                // Write lobby info data
                writer.Flush();
                //dataLength += writer.BaseStream.WriteString(Encoding.UTF8, GameInfo.Serialize());
            }

            // Write total length & end marker
            writer.Write(dataLength);
            writer.Write(MetaEndMarker);
        }
    }
}

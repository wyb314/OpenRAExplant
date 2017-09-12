using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine.Network
{
    public static class StreamExts
    {
        public static int ReadInt32(this Stream s)
        {
            return BitConverter.ToInt32(s.ReadBytes(4), 0);
        }

        public static byte[] ReadBytes(this Stream s, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            var buffer = new byte[count];
            s.ReadBytes(buffer, 0, count);
            return buffer;
        }

        public static void ReadBytes(this Stream s, byte[] buffer, int offset, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            while (count > 0)
            {
                int bytesRead;
                if ((bytesRead = s.Read(buffer, offset, count)) == 0)
                    throw new EndOfStreamException();
                offset += bytesRead;
                count -= bytesRead;
            }
        }

        public static void Write(this Stream s, byte[] buf)
        {
            s.Write(buf, 0, buf.Length);
        }
    }
}

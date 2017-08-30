using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OpenRA
{
    public static class StreamExts
    {
        public static byte ReadUInt8(this Stream s)
        {
            var b = s.ReadByte();
            if (b == -1)
                throw new EndOfStreamException();
            return (byte)b;
        }

        public static ushort ReadUInt16(this Stream s)
        {
            return BitConverter.ToUInt16(s.ReadBytes(2), 0);
        }

        public static uint ReadUInt32(this Stream s)
        {
            return BitConverter.ToUInt32(s.ReadBytes(4), 0);
        }

        public static int ReadInt32(this Stream s)
		{
			return BitConverter.ToInt32(s.ReadBytes(4), 0);
		}

        public static string ReadString(this Stream s, Encoding encoding, int maxLength)
        {
            var length = s.ReadInt32();
            if (length > maxLength)
                throw new InvalidOperationException("The length of the string ({0}) is longer than the maximum allowed ({1}).".F(length, maxLength));

            return encoding.GetString(s.ReadBytes(length));
        }


        // Writes a length-prefixed string using the specified encoding and returns
        // the number of bytes written.
        public static int WriteString(this Stream s, Encoding encoding, string text)
        {
            byte[] bytes;

            if (!string.IsNullOrEmpty(text))
                bytes = encoding.GetBytes(text);
            else
                bytes = new byte[0];

            s.Write(bytes.Length);
            s.Write(bytes);

            return 4 + bytes.Length;
        }

        public static void Write(this Stream s, int value)
        {
            s.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream s, byte[] data)
        {
            s.Write(data, 0, data.Length);
        }

        public static byte[] ReadAllBytes(this Stream s)
        {
            using (s)
            {
                if (s.CanSeek)
                    return s.ReadBytes((int)(s.Length - s.Position));

                var bytes = new List<byte>();
                var buffer = new byte[1024];
                int count;
                while ((count = s.Read(buffer, 0, buffer.Length)) > 0)
                    bytes.AddRange(buffer.Take(count));
                return bytes.ToArray();
            }
        }
    }
}

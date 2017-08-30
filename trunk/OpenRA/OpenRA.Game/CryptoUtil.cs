using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenRA
{
    public static class CryptoUtil
    {
        public static string SHA1Hash(Stream data)
        {
            using (var csp = SHA1.Create())
                return new string(csp.ComputeHash(data).SelectMany(a => a.ToString("x2")).ToArray());
        }

        public static string SHA1Hash(byte[] data)
        {
            using (var csp = SHA1.Create())
                return new string(csp.ComputeHash(data).SelectMany(a => a.ToString("x2")).ToArray());
        }

        public static string SHA1Hash(string data)
        {
            return SHA1Hash(Encoding.UTF8.GetBytes(data));
        }
    }
}

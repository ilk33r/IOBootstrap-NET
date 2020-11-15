using System;
using System.IO;
using System.Text;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IOHexUtilities
    {
        public static byte[] HexStringToByteArray(string hexString)
        {
            MemoryStream stream = new MemoryStream(hexString.Length / 2);
            for (int i = default(int); i < hexString.Length; i += 2)
            {
                stream.WriteByte(byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            return stream.ToArray();
        }

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte value in byteArray)
            {
                hex.AppendFormat("{0:x2}", value);
            }
            return hex.ToString();
        }
    }
}

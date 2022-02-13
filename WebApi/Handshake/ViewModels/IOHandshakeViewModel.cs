using System;
using IOBootstrap.NET.Common.Encryption;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using Org.BouncyCastle.Crypto.Parameters;

namespace IOBootstrap.NET.WebApi.Handshake.ViewModels
{
    public class IOHandshakeViewModel : IOViewModel
    {
        public Tuple<string, string> GetPuplicKey()
        {
            RsaPrivateCrtKeyParameters privateKey = IOEncryptionUtilities.GenerateRSAKeyPair();
            byte[] modulusBytes = privateKey.Modulus.ToByteArray();
            byte[] exponentBytes = privateKey.PublicExponent.ToByteArray();

            string modulus = IOHexUtilities.ByteArrayToHexString(modulusBytes);
            string exponent = IOHexUtilities.ByteArrayToHexString(exponentBytes);

            return new Tuple<string, string>(modulus, exponent);
        }
    }
}

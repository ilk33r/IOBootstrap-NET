using System;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Encryption;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Org.BouncyCastle.Crypto.Parameters;

namespace IOBootstrap.NET.WebApi.Handshake.ViewModels;

public class IOHandshakeViewModel<TDBContext> : IOViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    public async Task<Tuple<string, string>> GetPuplicKey()
    {
        RsaPrivateCrtKeyParameters privateKey = await IOEncryptionUtilities.GenerateRSAKeyPair();
        byte[] modulusBytes = privateKey.Modulus.ToByteArray();
        byte[] exponentBytes = privateKey.PublicExponent.ToByteArray();

        string modulus = IOHexUtilities.ByteArrayToHexString(modulusBytes);
        string exponent = IOHexUtilities.ByteArrayToHexString(exponentBytes);

        return new Tuple<string, string>(modulus, exponent);
    }
}

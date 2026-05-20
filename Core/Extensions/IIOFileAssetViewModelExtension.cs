using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOFileAssetViewModelExtension
{
    public static string CreateFilePublicId(this IIOFileAssetViewModel input, string fileName)
    {
        byte[] key = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionKey) ?? string.Empty
        );
        byte[] iv = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionIV) ?? string.Empty
        );
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        return IOHexUtilities.ByteArrayToHexString(aesUtilities.Encrypt(fileName));
    }

    public static string? GetFileName(this IIOFileAssetViewModel input, string publicId)
    {
        byte[] key = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionKey) ?? string.Empty
        );
        byte[] iv = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionIV) ?? string.Empty
        );
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        if (!String.IsNullOrEmpty(publicId))
        {
            return aesUtilities.Decrypt(IOHexUtilities.HexStringToByteArray(publicId));
        }

        return null;
    }
}

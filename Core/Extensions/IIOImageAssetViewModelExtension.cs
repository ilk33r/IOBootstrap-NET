using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOImageAssetViewModelExtension
{
    public static string CreateImagePublicId(this IIOImageAssetViewModel input, string fileName)
    {
		byte[] key = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
		byte[] iv = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        return aesUtilities.Encrypt(fileName);
    }

    public static string GetImageFileName(this IIOImageAssetViewModel input, string publicId)
    {
		byte[] key = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
		byte[] iv = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        if (!String.IsNullOrEmpty(publicId)) 
        {
            return aesUtilities.Decrypt(publicId);
        }

        return null;
    }
}

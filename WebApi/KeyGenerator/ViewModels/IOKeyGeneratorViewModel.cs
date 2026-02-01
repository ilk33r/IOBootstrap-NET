using System;
using System.Text;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Encryption;
using IOBootstrap.NET.Common.Messages.KeyGenerator;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace IOBootstrap.NET.WebApi.KeyGenerator.ViewModels;

public class IOKeyGeneratorViewModel<TDBContext> : IOViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{

    public override void CheckAuthorizationHeader()
    {
#if DEBUG
        return;
#else
            base.CheckAuthorizationHeader();
#endif
    }

    public async Task<IOEncryptResponseModel> Encrypt(IOEncryptRequestModel requestModel)
    {
        IOCacheObject? encryptedKey = IOCache.GetCachedObject(IOCacheKeys.SwaggerSymmetricKey);
        IOCacheObject? encryptedIV = IOCache.GetCachedObject(IOCacheKeys.SwaggerSymmetricIV);

        string aesKey;
        string aesIV;
        string symmetricKey = "";
        string symmetricIV = "";

        // Create aes key and iv
        byte[] aesKeyBytes;
        byte[] aesIVBytes;

        if (encryptedKey == null || encryptedIV == null)
        {
            aesKey = IORandomUtilities.GenerateRandomAlphaNumericString(32);
            aesIV = IORandomUtilities.GenerateRandomAlphaNumericString(16);
            aesKeyBytes = Encoding.UTF8.GetBytes(aesKey);
            aesIVBytes = Encoding.UTF8.GetBytes(aesIV);
        }
        else
        {
            string encryptedKeyString = (string)encryptedKey.Value;
            string encryptedIVString = (string)encryptedIV.Value;

            byte[] encryptedSymmetricIV = Convert.FromBase64String(encryptedIVString);
            aesIVBytes = await IOEncryptionUtilities.DecryptString(encryptedSymmetricIV);

            byte[] encryptedSymmetricKey = Convert.FromBase64String(encryptedKeyString);
            aesKeyBytes = await IOEncryptionUtilities.DecryptString(encryptedSymmetricKey);

            symmetricKey = encryptedKeyString;
            symmetricIV = encryptedIVString;
        }

        IOAESUtilities aesUtilities = new IOAESUtilities(aesKeyBytes, aesIVBytes);

        byte[] exponent = Convert.FromHexString(requestModel.PublicKeyExponent ?? "");
        byte[] modulus = Convert.FromHexString(requestModel.PublicKeyModulus ?? "");

        RsaKeyParameters publicKey = new RsaKeyParameters(false, new BigInteger(modulus), new BigInteger(exponent));
        IAsymmetricBlockCipher rsaEngine = new OaepEncoding(new RsaEngine(), new Sha256Digest());
        rsaEngine.Init(true, publicKey);

        if (encryptedKey == null || encryptedIV == null)
        {
            byte[] encryptedSymmetricKey = rsaEngine.ProcessBlock(aesKeyBytes, 0, aesKeyBytes.Length);
            byte[] encryptedSymmetricIV = rsaEngine.ProcessBlock(aesIVBytes, 0, aesIVBytes.Length);
            symmetricKey = Convert.ToBase64String(encryptedSymmetricKey);
            symmetricIV = Convert.ToBase64String(encryptedSymmetricIV);

            encryptedKey = new IOCacheObject(IOCacheKeys.SwaggerSymmetricKey, symmetricKey, 0);
            encryptedIV = new IOCacheObject(IOCacheKeys.SwaggerSymmetricIV, symmetricIV, 0);

            IOCache.CacheObject(encryptedKey);
            IOCache.CacheObject(encryptedIV);
        }
        
        string encryptedString = Convert.ToBase64String(aesUtilities.Encrypt(requestModel.PlainText ?? ""));

        IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
        {
            SymmetricKey = symmetricKey,
            SymmetricIV = symmetricIV,
            EncryptedValue = encryptedString
        };

        return responseModel;
    }

    public async Task<IOEncryptResponseModel> Decrypt(IOEncryptRequestModel requestModel)
    {
        string decryptedString = await DecryptString(requestModel.PlainText ?? "");
        IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
        {
            SymmetricKey = "",
            SymmetricIV = "",
            EncryptedValue = decryptedString
        };

        return responseModel;
    }

    public IOEncryptResponseModel EncryptAES(IOEncryptRequestModel requestModel)
    {
        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        string encrypted = Convert.ToBase64String(aesUtilities.Encrypt(requestModel.PlainText ?? ""));

        IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
        {
            SymmetricKey = null,
            SymmetricIV = null,
            EncryptedValue = encrypted
        };

        return responseModel;
    }

    public IOEncryptResponseModel DecryptAES(IOEncryptRequestModel requestModel)
    {
        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        try
        {
            // Obtain decrypted token value
            string decryptedToken = aesUtilities.Decrypt(Convert.FromBase64String(requestModel.PlainText ?? ""));

            IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
            {
                SymmetricKey = null,
                SymmetricIV = null,
                EncryptedValue = decryptedToken
            };

            return responseModel;
        }
        catch (Exception e)
        {
            Logger.LogDebug(e.StackTrace);
            
            IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
            {
                SymmetricKey = null,
                SymmetricIV = null,
                EncryptedValue = null
            };

            return responseModel;
        }
    }
}

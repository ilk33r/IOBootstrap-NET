using System;
using System.Text;
using IOBootstrap.NET.Common.Constants;
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
where TDBContext : IODatabaseContext<TDBContext>
{

    public override void CheckAuthorizationHeader()
    {
#if DEBUG
        return;
#else
            base.CheckAuthorizationHeader();
#endif
    }

    public IOEncryptResponseModel Encrypt(IOEncryptRequestModel requestModel)
    {
        // Create aes key and iv
        string aesKey = IORandomUtilities.GenerateRandomAlphaNumericString(32);
        string aesIV = IORandomUtilities.GenerateRandomAlphaNumericString(16);
        byte[] aesKeyBytes = Encoding.UTF8.GetBytes(aesKey);
        byte[] aesIVBytes = Encoding.UTF8.GetBytes(aesIV);
        IOAESUtilities aesUtilities = new IOAESUtilities(aesKeyBytes, aesIVBytes);

        byte[] exponent = Convert.FromHexString(requestModel.PublicKeyExponent ?? "");
        byte[] modulus = Convert.FromHexString(requestModel.PublicKeyModulus ?? "");

        RsaKeyParameters publicKey = new RsaKeyParameters(false, new BigInteger(modulus), new BigInteger(exponent));
        IAsymmetricBlockCipher rsaEngine = new OaepEncoding(new RsaEngine(), new Sha256Digest());
        rsaEngine.Init(true, publicKey);
        byte[] encryptedSymmetricKey = rsaEngine.ProcessBlock(aesKeyBytes, 0, aesKeyBytes.Length);
        byte[] encryptedSymmetricIV = rsaEngine.ProcessBlock(aesIVBytes, 0, aesIVBytes.Length);

        string encryptedString = Convert.ToBase64String(aesUtilities.Encrypt(requestModel.PlainText ?? ""));

        IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
        {
            SymmetricKey = Convert.ToBase64String(encryptedSymmetricKey),
            SymmetricIV = Convert.ToBase64String(encryptedSymmetricIV),
            EncryptedValue = encryptedString
        };

        return responseModel;
    }

    public IOEncryptResponseModel Decrypt(IOEncryptRequestModel requestModel)
    {
        string decryptedString = DecryptString(requestModel.PlainText ?? "");
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

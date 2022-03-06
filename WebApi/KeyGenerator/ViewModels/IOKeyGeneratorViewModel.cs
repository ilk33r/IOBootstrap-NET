using System;
using System.Text;
using IOBootstrap.Net.Common.Messages.KeyGenerator;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace IOBootstrap.NET.WebApi.KeyGenerator.ViewModels
{
    public class IOKeyGeneratorViewModel : IOViewModel
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

            byte[] exponent = Convert.FromHexString(requestModel.PublicKeyExponent);
            byte[] modulus = Convert.FromHexString(requestModel.PublicKeyModulus);

            RsaKeyParameters publicKey = new RsaKeyParameters(false, new BigInteger(modulus), new BigInteger(exponent));            
            IAsymmetricBlockCipher rsaEngine = new Pkcs1Encoding(new RsaEngine());
            rsaEngine.Init(true, publicKey);
            byte[] encryptedSymmetricKey = rsaEngine.ProcessBlock(aesKeyBytes, 0, aesKeyBytes.Length);
            byte[] encryptedSymmetricIV = rsaEngine.ProcessBlock(aesIVBytes, 0, aesIVBytes.Length);

            string encryptedString = aesUtilities.Encrypt(requestModel.PlainText);

            IOEncryptResponseModel responseModel = new IOEncryptResponseModel()
            {
                SymmetricKey = Convert.ToBase64String(encryptedSymmetricKey),
                SymmetricIV = Convert.ToBase64String(encryptedSymmetricIV),
                EncryptedValue = encryptedString
            };

            return responseModel;
        }
    }
}

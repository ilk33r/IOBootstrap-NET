using System;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace IOBootstrap.NET.Common.Encryption
{
    public static class IOEncryptionUtilities
    {

        public static RsaPrivateCrtKeyParameters GenerateRSAKeyPair() 
        {
            IOCacheObject cachedKey = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyCacheKey);
            if (cachedKey != null)
            {
                RsaPrivateCrtKeyParameters cachedPrivateKey = (RsaPrivateCrtKeyParameters)cachedKey.Value;
                return cachedPrivateKey;
            }

            // Create key id
            string keyID = IORandomUtilities.GenerateGUIDString();
            IOCacheObject keyIDCacheObject = new IOCacheObject(IOCacheKeys.RSAPrivateKeyIDCacheKey, keyID, IOCommonConstants.KeyPairCacheTimeInterval);

            // Create key pair generator
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));

            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
            cachedKey = new IOCacheObject(IOCacheKeys.RSAPrivateKeyCacheKey, privateKey, IOCommonConstants.KeyPairCacheTimeInterval);
            IOCache.CacheObject(cachedKey);
            IOCache.CacheObject(keyIDCacheObject);

            return privateKey;
        }

        public static byte[] DecryptString(byte[] encryptedData) {
            RsaPrivateCrtKeyParameters privateKey = GenerateRSAKeyPair();
            
            IAsymmetricBlockCipher rsaEngine = new Pkcs1Encoding(new RsaEngine());
            rsaEngine.Init(false, privateKey);
            byte[] decryptedData = rsaEngine.ProcessBlock(encryptedData, 0, encryptedData.Length);
            return decryptedData;
        }

        public static bool VerifyECSignature(byte[] publicKey, byte[] plainData, byte[] signedData)
        {
            var curve = SecNamedCurves.GetByName("secp256r1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            var point = curve.Curve.DecodePoint(publicKey);
            ECPublicKeyParameters PublicKey = new ECPublicKeyParameters(point, domain);

            ISigner signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(false, PublicKey);
            signer.BlockUpdate(plainData, 0, plainData.Length);

            return signer.VerifySignature(signedData);
        }
    }
}

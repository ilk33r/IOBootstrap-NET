using System;
using System.IO;
using System.Security.Cryptography;

namespace IOBootstrap.NET.Common.Utilities
{
    public class IOAESUtilities
    {
        #region Privates

        private Aes Encryptor;

        #endregion

        #region Initialization Methods

        public IOAESUtilities(byte[] key, byte[] iv)
        {
            // Create encryptor
            Encryptor = Aes.Create();
            Encryptor.Key = key;
            Encryptor.IV = iv;
        }

        #endregion

        #region Helper Methods

        public string Decrypt(string encryptedString)
        {
            // Decode encrypted string
            byte[] decodedBytes = Convert.FromBase64String(encryptedString);

            // Create a encryptor to perform the stream transform.
            ICryptoTransform crypto = Encryptor.CreateDecryptor(Encryptor.Key, Encryptor.IV);

            // Create the streams used for decryption.
            MemoryStream msDecrypt = new MemoryStream(decodedBytes);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, crypto, CryptoStreamMode.Read);
            StreamReader swDecrypt = new StreamReader(csDecrypt);

            string decryptedString = swDecrypt.ReadToEnd();
            swDecrypt.Dispose();

            csDecrypt.Flush();
            csDecrypt.Dispose();

            msDecrypt.Flush();
            msDecrypt.Dispose();

            return decryptedString;
        }

        public string Encrypt(string plainString)
        {
            // Create a encryptor to perform the stream transform.
            ICryptoTransform crypto = Encryptor.CreateEncryptor(Encryptor.Key, Encryptor.IV);

            // Create the streams used for encryption.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, crypto, CryptoStreamMode.Write);
            StreamWriter swEncrypt = new StreamWriter(csEncrypt);

            //Write all data to the stream.
            swEncrypt.Write(plainString);

            swEncrypt.Flush();
            swEncrypt.Dispose();

            csEncrypt.Flush();
            csEncrypt.Dispose();

            msEncrypt.Flush();
            msEncrypt.Dispose();

            // Create encrypted bytes
            byte[] encryptedBytes = msEncrypt.ToArray();

            return Convert.ToBase64String(encryptedBytes);
        }

        #endregion
    }
}

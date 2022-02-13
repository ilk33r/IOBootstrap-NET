using System;
using System.IO;
using System.Security.Cryptography;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IOPasswordUtilities
    {
        #region Constants

        private const int HashSize = 64;
        private const int SaltSize = 32;

        #endregion

        #region Hash

        public static string HashPassword(string password, int iterations)
        {
            //create salt
            RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
            byte[] salt;
            numberGenerator.GetBytes(salt = new byte[SaltSize]);

            //create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            //combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            //convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            //format hash with extra information
            return string.Format("$IOPSSWD$V1${0}${1}", iterations, base64Hash);
        }

        public static string HashPassword(string password)
        {
            return HashPassword(password, 10000);
        }

        public static bool IsPasswordHashSupported(string hashString)
        {
            return hashString.Contains("$IOPSSWD$V1$");
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            //check hash
            if (!IsPasswordHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            //extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$IOPSSWD$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            //get hashbytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            //get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            //create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            //get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}

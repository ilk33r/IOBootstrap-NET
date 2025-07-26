using System;
using System.Security.Cryptography;

namespace IOBootstrap.NET.Common.Utilities;

public class IOPasswordUtilities : IDisposable
{

    public delegate void PasswordResponse(string hashed);
    public delegate void VerifyResponse(bool verified);

    #region Constants

    private const int HashSize = 64;
    private const int SaltSize = 32;

    #endregion

    #region Hash

    public async Task HashPassword(string password, int iterations, PasswordResponse callback)
    {
        await Task.Run(() =>
        {
            //create salt
            RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
            byte[] salt;
            numberGenerator.GetBytes(salt = new byte[SaltSize]);

            //create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            //combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            //convert to base64
            string? base64Hash = Convert.ToBase64String(hashBytes);

            //format hash with extra information
            callback(string.Format("$IOPSSWD$V1${0}${1}", iterations, base64Hash));

            // Null values
            base64Hash = null;
            hash = null;
        });
    }

    public async Task HashPassword(string password, PasswordResponse callback)
    {
        await HashPassword(password, 10000, callback);
    }

    public bool IsPasswordHashSupported(string hashString)
    {
        return hashString.Contains("$IOPSSWD$V1$");
    }

    public async Task VerifyPassword(string password, string hashedPassword, VerifyResponse callback)
    {
        await Task.Run(() =>
        {
            //check hash
            if (!IsPasswordHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            //extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$IOPSSWD$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            string? base64Hash = splittedHashString[1];

            //get hashbytes
            byte[]? hashBytes = Convert.FromBase64String(base64Hash);

            //get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            //create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            //get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    callback(false);
                    base64Hash = null;
                    hashBytes = null;
                    return;
                }
            }

            base64Hash = null;
            hashBytes = null;
            callback(true);
        });
    }

    #endregion

    #region Disposable

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
}

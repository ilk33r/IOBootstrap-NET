using System;
using System.Security.Cryptography;
using System.Text;

namespace IOBootstrap.NET.Common.Utilities;

public static class IORandomUtilities
{

    #region Random Helpers

    public static string GenerateGUIDString()
    {
        return Guid.NewGuid().ToString();
    }

    public static string GenerateRandomAlphaNumericString(int characterCount)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, characterCount)
                          .Select(s => s[GenerateRandomNumber(chars.Length)]).ToArray());
    }

    public static string GenerateRandomNumericString(int characterCount)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, characterCount)
                          .Select(s => s[GenerateRandomNumber(chars.Length)]).ToArray());
    }

    public static int GenerateRandomNumber(int max)
    {
        byte[] random = new byte[4];
        int value;

        using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
        {
            do
            {
                rnd.GetBytes(random);
                value = BitConverter.ToInt32(random, 0) & Int32.MaxValue;
            } while (value >= max * (Int32.MaxValue / max));
        }
        
        return value % max;
    }

    #endregion

}

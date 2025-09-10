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
                          .Select(s => s[GenerateRandomNumber(0, chars.Length)]).ToArray());
    }

    public static string GenerateRandomNumericString(int characterCount)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, characterCount)
                          .Select(s => s[GenerateRandomNumber(0, chars.Length)]).ToArray());
    }

    public static int GenerateRandomNumber(int min, int max)
    {
        byte[] random = new byte[4];
        int value;

        int range = max - min;
        using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
        {
            do
            {
                rnd.GetBytes(random);
                value = BitConverter.ToInt32(random, 0) & Int32.MaxValue;
            } while (value >= range * (Int32.MaxValue / range));
        }

        return min + (value % range);
    }
    
    public static double GenerateRandomNumber(double min, double max)
    {
        byte[] buffer = new byte[8];
        double result;

        double range = max - min;
        using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
        {
            rnd.GetBytes(buffer);
            ulong rand = BitConverter.ToUInt64(buffer, 0);
            result = rand / (double)(ulong.MaxValue + 1.0);
        }
        
        return min + (result * (max - min));
    }

    #endregion

}

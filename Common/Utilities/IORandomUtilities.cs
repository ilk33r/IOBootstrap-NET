using System;

namespace IOBootstrap.NET.Common.Utilities
{
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
                              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static string GenerateRandomNumericString(int characterCount)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, characterCount)
                              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        #endregion

    }
}

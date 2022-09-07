using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IOBootstrap.NET.Common.Utilities
{
    public class IORegexUtility
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static bool HasSpecialCharacter(string text)
        {
            return !text.All(c => 
            {
                int num = (int)c;
                if (num >= 97 && num <= 122)
                {
                    return true;
                }

                if (num >= 65 && num <= 90)
                {
                    return true;
                }

                if (num >= 48 && num <= 57)
                {
                    return true;
                }

                return false;
            });
        }
    }
}

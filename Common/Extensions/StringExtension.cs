using System.Text;

namespace IOBootstrap.NET.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns the input string with the first character converted to lowercase
    /// </summary>
    public static string ToLowerFirst(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("There is no first letter");
        }

        Span<char> a = stackalloc char[input.Length];
        input.AsSpan(1).CopyTo(a.Slice(1));
        a[0] = char.ToLower(input[0]);
        return new string(a);
    }

    public static string RemoveNonASCII(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            int character = (int)c;
            if (character >= 48 && character <= 58) // 0-9
            {
                sb.Append(c);
                continue;
            }

            if (character >= 65 && character <= 90) // a-z
            {
                sb.Append(c);
                continue;
            }

            if (character >= 97 && character <= 122) // A-Z
            {
                sb.Append(c);
                continue;
            }

            sb.Append('-');
        }

        return sb.ToString();
    }
}
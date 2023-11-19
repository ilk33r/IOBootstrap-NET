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
}
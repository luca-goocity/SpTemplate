using System.Text;
using System.Text.RegularExpressions;
using Common.Models.Models.Constants;

namespace Common.Libs.Extensions;

public static class StringExtensions
{
    public static string Sanitize(this string source)
    {
        return source.Trim().ToUpper();
    }
    
    public static string SanitizeLower(this string source)
    {
        return source.Trim().ToLower();
    }

    public static string RemoveHtmlTags(this string source)
    {
        return Regex.Replace(source, RegexConstants.HtmlTag, string.Empty);
    }

    public static string ToTitleCase(this string source)
    {
        return char.ToUpper(source[0]) + source.ToLower()[1..];
    }

    public static string Truncate(this string? source, int len)
    {
        return source is not null && source.Length > len
            ? source[..len]
            : source ?? string.Empty;
    }
    
    public static string RemoveSpecialCharacters(this string str)
    {
        str = str.ToLower();
        var sb = new StringBuilder();
        foreach (var c in str.Where(c => c is >= 'a' and <= 'z' or >= '0' and <= '9'))
        {
            sb.Append(c);
        }
        return sb.ToString();
    }
}
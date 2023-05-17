using System.Text.RegularExpressions;

namespace Common.Models.Models.Constants;

public static class RegexConstants
{
    public static readonly Regex Email = new(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
    public static readonly Regex Phone = new(@"^[0-9\b]*$");
    public const string HtmlTag ="<.*?>";
}
using Common.Models.Models.Constants;

namespace Common.Libs.Utils;

public static class ValidationUtils
{
    public static bool BeAValidEmail(string? source)
        => !string.IsNullOrWhiteSpace(source) && RegexConstants.Email.IsMatch(source);

    public static bool BeAValidPhone(string? source)
        => !string.IsNullOrWhiteSpace(source) && RegexConstants.Phone.IsMatch(source);

    public static bool HaveSpecialChars(string stringToCheck)
        => stringToCheck.Any(char.IsDigit);

    public static bool BeANumber(string? source)
        => source?.All(char.IsDigit) ?? false;
}
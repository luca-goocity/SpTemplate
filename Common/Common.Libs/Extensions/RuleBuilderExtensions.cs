using Common.Models.Models.Constants;
using FluentValidation;
using MongoDB.Bson;

namespace Common.Libs.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, TProperty?> MustBeAValidMongoId<T, TProperty>(
        this IRuleBuilder<T, TProperty?> source)
    {
        return source.Must(xx =>
            {
                if (xx is null) return false;
                if (xx is string data) return ObjectId.TryParse(data, out _);
                return false;
            })
            .WithMessage("ID non valido.");
    }

    public static IRuleBuilderOptions<T, TProperty?> MustBeAValidListOfMongoId<T, TProperty>(
        this IRuleBuilder<T, TProperty?> source)
    {
        return source.Must(xx =>
            {
                if (xx is null) return false;
                if (xx is IEnumerable<string> data) return data.All(item => ObjectId.TryParse(item, out _));
                return false;
            })
            .WithMessage("Trovato almeno un ID non valido.");
    }

    public static IRuleBuilderOptions<T, TProperty?> MustBeAValidEmail<T, TProperty>(
        this IRuleBuilder<T, TProperty?> source)
    {
        return source.Must(xx =>
            {
                if (xx is null) return false;
                if (xx is string data)
                    return !string.IsNullOrWhiteSpace(data) && RegexConstants.Email.IsMatch(data);
                return false;
            })
            .WithMessage("Indirizzo email non valido.");
    }

    public static IRuleBuilderOptions<T, TProperty?> MustBeAValidPhone<T, TProperty>(
        this IRuleBuilder<T, TProperty?> source)
    {
        return source.Must(xx =>
            {
                if (xx is null) return false;
                if (xx is string data)
                    return data.Length is >= 4 and <= 20
                           && data.All(ch => char.IsDigit(ch) || data.StartsWith("+"));
                return false;
            })
            .WithMessage("Numero di telefono non valido.");
    }

    public static IRuleBuilderOptions<T, TProperty?> MustNotHaveSpecialChars<T, TProperty>(
        this IRuleBuilder<T, TProperty?> source)
    {
        return source.Must(xx =>
            {
                if (xx is null) return false;
                if (xx is string data) return data.All(char.IsLetter);
                return false;
            })
            .WithMessage("Non può contenere caratteri speciali.");
    }
}
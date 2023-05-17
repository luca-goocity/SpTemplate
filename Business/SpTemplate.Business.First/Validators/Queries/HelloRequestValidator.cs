using FluentValidation;
using SpTemplate.Business.First.Models.Requests.Queries;

namespace SpTemplate.Business.First.Validators.Queries;

public sealed class HelloRequestValidator : AbstractValidator<HelloRequest>
{
    public HelloRequestValidator()
    {
        RuleFor(xx => xx.Name)
            .NotNull()
            .NotEmpty()
            .MinimumLength(8)
            ;
    }
}
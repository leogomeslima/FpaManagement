using FluentValidation;
using FpaManagement.Application.DTOs.Forecast;

namespace FpaManagement.Application.Validators.Forecast;

public class CreateForecastItemValidator : AbstractValidator<CreateForecastItemDto>
{
    public CreateForecastItemValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Categoria é obrigatória")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(500);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Valor não pode exceder 1 bilhão");
    }
}

using FluentValidation;
using FpaManagement.Application.DTOs.Forecast;

namespace FpaManagement.Application.Validators.Forecast;

public class CreateForecastValidator : AbstractValidator<CreateForecastDto>
{
    public CreateForecastValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.FiscalYear)
            .NotEmpty().WithMessage("Ano fiscal é obrigatório")
            .InclusiveBetween(DateTime.Now.Year - 1, DateTime.Now.Year + 5)
            .WithMessage($"Ano fiscal deve estar entre {DateTime.Now.Year - 1} e {DateTime.Now.Year + 5}");

        RuleFor(x => x.VersionName)
            .NotEmpty().WithMessage("Nome da versão é obrigatório")
            .MaximumLength(100);
    }
}

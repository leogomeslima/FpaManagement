using FluentValidation;
using FpaManagement.Application.DTOs.CashFlow;

namespace FpaManagement.Application.Validators.CashFlow;

public class CreateCashFlowEntryValidator : AbstractValidator<CreateCashFlowEntryDto>
{
    public CreateCashFlowEntryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Data é obrigatória");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Valor não pode exceder 1 bilhão");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Moeda é obrigatória")
            .Must(c => c == "BRL" || c == "USD" || c == "EUR")
            .WithMessage("Moeda deve ser BRL, USD ou EUR");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo inválido");
    }
}

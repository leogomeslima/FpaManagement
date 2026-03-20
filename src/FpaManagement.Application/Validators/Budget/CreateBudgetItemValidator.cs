using FluentValidation;
using FpaManagement.Application.DTOs.Budget;

namespace FpaManagement.Application.Validators.Budget;

public class CreateBudgetItemValidator : AbstractValidator<CreateBudgetItemDto>
{
    public CreateBudgetItemValidator()
    {
        RuleFor(x => x.BudgetVersionId)
            .NotEmpty().WithMessage("ID da versão do orçamento é obrigatório")
            .Must(id => id != Guid.Empty).WithMessage("ID da versão inválido");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Categoria é obrigatória")
            .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres");

        RuleFor(x => x.SubCategory)
            .MaximumLength(100).WithMessage("Subcategoria deve ter no máximo 100 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.PlannedAmount)
            .GreaterThan(0).WithMessage("Valor planejado deve ser maior que zero")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Valor planejado não pode exceder 1 bilhão");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Moeda é obrigatória")
            .Must(c => c == "BRL" || c == "USD" || c == "EUR")
            .WithMessage("Moeda deve ser BRL, USD ou EUR");

        RuleFor(x => x.CostCenterId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("ID do centro de custo inválido quando informado");

        RuleFor(x => x.AccountCode)
            .MaximumLength(50).WithMessage("Código da conta deve ter no máximo 50 caracteres")
            .Matches("^[A-Z0-9.-]*$").WithMessage("Código da conta deve conter apenas letras maiúsculas, números, ponto e hífen")
            .When(x => !string.IsNullOrEmpty(x.AccountCode));
    }
}

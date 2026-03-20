using FluentValidation;
using FpaManagement.Application.DTOs.CostCenter;

namespace FpaManagement.Application.Validators.CostCenter;

public class UpdateCostCenterValidator : AbstractValidator<UpdateCostCenterDto>
{
    public UpdateCostCenterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do centro de custo é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.ManagerId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("ID do gestor inválido quando informado");

        RuleFor(x => x.AnnualBudget)
            .GreaterThanOrEqualTo(0).WithMessage("Orçamento anual não pode ser negativo")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Orçamento anual não pode exceder 1 bilhão");
    }
}

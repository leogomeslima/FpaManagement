using FluentValidation;
using FpaManagement.Application.DTOs.Budget;

namespace FpaManagement.Application.Validators.Budget;

public class CreateBudgetValidator : AbstractValidator<CreateBudgetDto>
{
    public CreateBudgetValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do orçamento é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.FiscalYear)
            .NotEmpty().WithMessage("Ano fiscal é obrigatório")
            .InclusiveBetween(DateTime.Now.Year - 1, DateTime.Now.Year + 5)
            .WithMessage($"Ano fiscal deve estar entre {DateTime.Now.Year - 1} e {DateTime.Now.Year + 5}");

        RuleFor(x => x.VersionName)
            .NotEmpty().WithMessage("Nome da versão é obrigatório")
            .MaximumLength(100).WithMessage("Nome da versão deve ter no máximo 100 caracteres");

        RuleFor(x => x.VersionDescription)
            .MaximumLength(500).WithMessage("Descrição da versão deve ter no máximo 500 caracteres");

        // Pelo menos um dos IDs deve ser informado
        RuleFor(x => x)
            .Must(x => x.DepartmentId.HasValue || x.CostCenterId.HasValue)
            .WithMessage("É necessário informar pelo menos um departamento ou centro de custo");
    }
}

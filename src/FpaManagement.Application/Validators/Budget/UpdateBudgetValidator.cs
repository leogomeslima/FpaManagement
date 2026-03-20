using FluentValidation;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Application.Validators.Budget;

public class UpdateBudgetValidator : AbstractValidator<UpdateBudgetDto>
{
    public UpdateBudgetValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do orçamento é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status inválido")
            .Must(status => status != BudgetStatus.Archived)
            .WithMessage("Não é possível atualizar um orçamento arquivado");
    }
}

using FluentValidation;
using FpaManagement.Application.DTOs.Department;

namespace FpaManagement.Application.Validators.Department;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do departamento é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.ManagerId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("ID do gestor inválido quando informado");
    }
}

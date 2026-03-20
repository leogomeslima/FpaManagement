using FluentValidation;
using FpaManagement.Application.DTOs.Department;

namespace FpaManagement.Application.Validators.Department;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Código do departamento é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches("^[A-Z0-9_-]+$").WithMessage("Código deve conter apenas letras maiúsculas, números, underscore e hífen");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do departamento é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
    }
}

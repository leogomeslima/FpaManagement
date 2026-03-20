using FluentValidation;
using FpaManagement.Application.DTOs.User;

namespace FpaManagement.Application.Validators.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .MinimumLength(2).WithMessage("Nome deve ter no mínimo 2 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Sobrenome é obrigatório")
            .MaximumLength(100).WithMessage("Sobrenome deve ter no máximo 100 caracteres");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .Matches("^[0-9+() -]*$").WithMessage("Telefone inválido")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

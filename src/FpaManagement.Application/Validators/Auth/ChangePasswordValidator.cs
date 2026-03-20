using FluentValidation;
using FpaManagement.Application.DTOs.Auth;

namespace FpaManagement.Application.Validators.Auth;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(8).WithMessage("Nova senha deve ter no mínimo 8 caracteres")
            .MaximumLength(100).WithMessage("Nova senha deve ter no máximo 100 caracteres")
            .Matches("[A-Z]").WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches("[a-z]").WithMessage("Nova senha deve conter pelo menos uma letra minúscula")
            .Matches("[0-9]").WithMessage("Nova senha deve conter pelo menos um número")
            .Matches("[^a-zA-Z0-9]").WithMessage("Nova senha deve conter pelo menos um caractere especial")
            .NotEqual(x => x.CurrentPassword).WithMessage("Nova senha deve ser diferente da senha atual");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.NewPassword).WithMessage("Senhas não conferem");
    }
}

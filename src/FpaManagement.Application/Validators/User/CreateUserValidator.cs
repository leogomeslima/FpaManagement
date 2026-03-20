using FluentValidation;
using FpaManagement.Application.DTOs.User;

namespace FpaManagement.Application.Validators.User;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(256).WithMessage("Email deve ter no máximo 256 caracteres");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Nome de usuário é obrigatório")
            .MaximumLength(50).WithMessage("Nome de usuário deve ter no máximo 50 caracteres")
            .MinimumLength(3).WithMessage("Nome de usuário deve ter no mínimo 3 caracteres")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Nome de usuário deve conter apenas letras, números, ponto, underscore e hífen");

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

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número")
            .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.RoleIds)
            .NotEmpty().WithMessage("Pelo menos um perfil deve ser selecionado")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("IDs de perfil inválidos");
    }
}

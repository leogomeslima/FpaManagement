using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.User;

[Authorize(Permissions = "ManageUsers")]
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public CreateUserDto Data { get; set; } = null!;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(
        IIdentityService identityService,
        IMapper mapper,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar nomes das roles via IDs (usando o contexto de aplicação)
        var roleNames = await _context.Roles
            .Where(r => request.Data.RoleIds.Contains(r.Id) && !r.IsDeleted)
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        // 2. Delegar TODA a criação e atribuição de roles para o IdentityService
        // O IdentityService deve lidar internamente com UserManager e ApplicationUser
        var (result, userId) = await _identityService.CreateUserAsync(
            request.Data.UserName,
            request.Data.Email,
            request.Data.Password,
            roleNames);

        if (!result.Succeeded)
        {
            return Result<UserDto>.Failure(result.Errors);
        }

        // 3. Montar o DTO de retorno
        // Como não temos a entidade ApplicationUser aqui, montamos com os dados da request
        var userDto = new UserDto
        {
            Id = Guid.Parse(userId),
            UserName = request.Data.UserName,
            Email = request.Data.Email,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            PhoneNumber = request.Data.PhoneNumber,
            IsActive = true,
            Roles = roleNames
        };

        return Result<UserDto>.Success(userDto, "Usuário criado com sucesso.");
    }
}

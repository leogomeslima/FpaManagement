using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.User;

[Authorize(Permissions = "ManageUsers")]
public class ActivateUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ActivateUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id);

        if (user.IsActive)
            return Result.Success("Usuário já está ativo.");

        user.Activate();
        user.UpdatedBy = _currentUserService.UserEmail ?? "system";
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Usuário ativado com sucesso.");
    }
}

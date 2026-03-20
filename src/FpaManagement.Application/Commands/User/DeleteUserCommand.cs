using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.User;

[Authorize(Permissions = "ManageUsers")]
public class DeleteUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id);

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Usuário excluído com sucesso.");
    }
}

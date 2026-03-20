using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.User;

[Authorize(Permissions = "ManageUsers")]
public class AssignUserRolesCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}

public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AssignUserRolesCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

        // Remove perfis atuais (soft delete)
        var existingRoles = await _context.UserRoles
            .Where(ur => ur.UserId == request.UserId && !ur.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var ur in existingRoles)
            ur.Delete(); // marca como deletado

        // Adiciona novos perfis
        var roles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            var userRole = new UserRole(user.Id, role.Id);
            userRole.CreatedBy = _currentUserService.UserEmail ?? "system";
            _context.UserRoles.Add(userRole);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Perfis atribuídos com sucesso.");
    }
}

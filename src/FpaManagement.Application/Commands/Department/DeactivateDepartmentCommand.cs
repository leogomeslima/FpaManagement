using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Department;

[Authorize(Permissions = "EditDepartment")]
public class DeactivateDepartmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeactivateDepartmentCommandHandler : IRequestHandler<DeactivateDepartmentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateDepartmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeactivateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (department == null)
        {
            throw new NotFoundException(nameof(Department), request.Id);
        }

        department.Deactivate();
        department.UpdatedBy = _currentUserService.UserEmail ?? "system";
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Departamento desativado com sucesso");
    }
}

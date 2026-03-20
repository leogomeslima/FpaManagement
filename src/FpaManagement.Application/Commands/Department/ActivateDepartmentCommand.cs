using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Department;

[Authorize(Permissions = "EditDepartment")]
public class ActivateDepartmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class ActivateDepartmentCommandHandler : IRequestHandler<ActivateDepartmentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ActivateDepartmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ActivateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (department == null)
        {
            throw new NotFoundException(nameof(Department), request.Id);
        }

        department.Activate();
        department.UpdatedBy = _currentUserService.UserEmail ?? "system";
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Departamento ativado com sucesso");
    }
}

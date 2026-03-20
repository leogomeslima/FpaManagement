using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Department;

[Authorize(Permissions = "DeleteDepartment")]
public class DeleteDepartmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteDepartmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.CostCenters.Where(cc => !cc.IsDeleted))
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (department == null)
        {
            throw new NotFoundException(nameof(Department), request.Id);
        }

        // Verificar se há centros de custo vinculados
        if (department.CostCenters.Any())
        {
            return Result.Failure("Não é possível excluir um departamento que possui centros de custo vinculados");
        }

        // Soft delete
        department.IsDeleted = true;
        department.DeletedAt = DateTime.UtcNow;
        department.DeletedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Departamento excluído com sucesso");
    }
}

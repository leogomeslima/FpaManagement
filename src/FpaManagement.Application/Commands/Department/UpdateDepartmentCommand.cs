using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Department;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Department;

[Authorize(Permissions = "EditDepartment")]
public class UpdateDepartmentCommand : IRequest<Result<DepartmentDto>>
{
    public Guid Id { get; set; }
    public UpdateDepartmentDto Data { get; set; } = null!;
}

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateDepartmentCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.Manager)
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (department == null)
        {
            throw new NotFoundException(nameof(Department), request.Id);
        }

        // Atualizar dados
        department.Update(request.Data.Name, request.Data.Description);

        if (request.Data.ManagerId.HasValue)
        {
            department.SetManager(request.Data.ManagerId.Value);
        }

        department.UpdatedBy = _currentUserService.UserEmail ?? "system";
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<DepartmentDto>(department);
        return Result<DepartmentDto>.Success(result, "Departamento atualizado com sucesso");
    }
}

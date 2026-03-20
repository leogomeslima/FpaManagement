using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Department;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Department;

[Authorize(Permissions = "CreateDepartment")]
public class CreateDepartmentCommand : IRequest<Result<DepartmentDto>>
{
    public CreateDepartmentDto Data { get; set; } = null!;
}

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public CreateDepartmentCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        // Verificar se já existe departamento com o mesmo código
        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(d => d.Code == request.Data.Code && !d.IsDeleted, cancellationToken);

        if (existingDepartment != null)
        {
            return Result<DepartmentDto>.Failure($"Já existe um departamento com o código '{request.Data.Code}'");
        }

        // Criar departamento
        var department = new Domain.Entities.Department(
            request.Data.Code,
            request.Data.Name,
            request.Data.Description);

        department.CreatedBy = _currentUserService.UserEmail ?? "system";

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<DepartmentDto>(department);
        return Result<DepartmentDto>.Success(result, "Departamento criado com sucesso");
    }
}

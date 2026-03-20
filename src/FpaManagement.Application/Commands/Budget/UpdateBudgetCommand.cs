using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "EditBudget")]
public class UpdateBudgetCommand : IRequest<Result<BudgetDto>>
{
    public Guid Id { get; set; }
    public UpdateBudgetDto Data { get; set; } = null!;
}

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, Result<BudgetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBudgetCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BudgetDto>> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Versions)
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Domain.Entities.Budget), request.Id);

        budget.Update(request.Data.Name, request.Data.Description);
        budget.UpdatedBy = _currentUserService.UserEmail ?? "system";
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<BudgetDto>(budget);
        return Result<BudgetDto>.Success(dto, "Orçamento atualizado com sucesso.");
    }
}

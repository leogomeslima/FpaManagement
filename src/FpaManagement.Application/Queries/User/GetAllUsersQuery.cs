using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.User;

[Authorize(Permissions = "ManageUsers")]
public class GetAllUsersQuery : IRequest<PaginatedList<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "FirstName";
    public bool SortAscending { get; set; } = true;
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(u => u.FirstName.ToLower().Contains(term) ||
                                     u.LastName.ToLower().Contains(term) ||
                                     u.Email.ToLower().Contains(term) ||
                                     u.UserName.ToLower().Contains(term));
        }
        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        query = request.SortBy?.ToLower() switch
        {
            "firstname" => request.SortAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName),
            "lastname" => request.SortAscending ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName),
            "email" => request.SortAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "username" => request.SortAscending ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName),
            "createdat" => request.SortAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
            _ => request.SortAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName)
        };

        var projected = query.ProjectTo<UserDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<UserDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}

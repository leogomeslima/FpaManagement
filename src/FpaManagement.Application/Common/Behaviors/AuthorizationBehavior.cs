using System.Reflection;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Security;
using MediatR;

namespace FpaManagement.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated
            if (!_currentUserService.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        if (_currentUserService.IsInRole(role.Trim()))
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Permission-based authorization
            var authorizeAttributesWithPermissions = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Permissions));

            if (authorizeAttributesWithPermissions.Any())
            {
                var authorized = false;

                foreach (var permissions in authorizeAttributesWithPermissions.Select(a => a.Permissions.Split(',')))
                {
                    foreach (var permission in permissions)
                    {
                        if (_currentUserService.HasPermission(permission.Trim()))
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }

        return await next();
    }
}

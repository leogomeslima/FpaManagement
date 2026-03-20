using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.DTOs.Auth;
using FpaManagement.Application.DTOs.User;
using FpaManagement.Application.Validators.Auth;
using FpaManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var validator = new LoginRequestValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Usuário inativo. Contate o administrador." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (result.IsLockedOut)
        {
            return Unauthorized(new { message = "Usuário bloqueado por muitas tentativas. Tente novamente mais tarde." });
        }

        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }

        // Atualizar último login
        user.LastLoginAt = _dateTimeService.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtTokenAsync(user);
        var refreshToken = GenerateRefreshToken();

        // Salvar refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _dateTimeService.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email!,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToList()
        };

        return Ok(new LoginResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = _dateTimeService.UtcNow.AddMinutes(15),
            User = userDto
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= _dateTimeService.UtcNow)
        {
            return Unauthorized(new { message = "Refresh token inválido ou expirado" });
        }

        var newToken = await GenerateJwtTokenAsync(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = _dateTimeService.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new LoginResponseDto
        {
            AccessToken = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _dateTimeService.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (_currentUserService.UserId.HasValue)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.Value.ToString());
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }
        }

        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
    {
        var validator = new ChangePasswordValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId!.Value.ToString());
        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Senha alterada com sucesso" });
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "FpaManagementSecretKey2024!@#$%¨&*()_+";
        var issuer = jwtSettings["Issuer"] ?? "FpaManagement";
        var audience = jwtSettings["Audience"] ?? "FpaManagementClient";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", user.Id),
            new Claim("UserName", user.UserName!),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        // Adicionar roles como claims
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Adicionar permissões baseadas nas roles (aqui você pode carregar do banco)
        // Por enquanto, vamos adicionar algumas permissões de exemplo baseadas nas roles
        foreach (var role in roles)
        {
            switch (role.ToUpper())
            {
                case "ADMIN":
                    claims.Add(new Claim("Permission", "ViewDashboard"));
                    claims.Add(new Claim("Permission", "CreateBudget"));
                    claims.Add(new Claim("Permission", "ApproveBudget"));
                    claims.Add(new Claim("Permission", "ManageUsers"));
                    break;
                case "CONTROLLER":
                    claims.Add(new Claim("Permission", "ViewDashboard"));
                    claims.Add(new Claim("Permission", "ViewBudget"));
                    claims.Add(new Claim("Permission", "ApproveBudget"));
                    break;
                case "FINANCEIRO":
                    claims.Add(new Claim("Permission", "ViewDashboard"));
                    claims.Add(new Claim("Permission", "ViewBudget"));
                    claims.Add(new Claim("Permission", "CreateBudget"));
                    break;
                case "GESTOR":
                    claims.Add(new Claim("Permission", "ViewDashboard"));
                    claims.Add(new Claim("Permission", "ViewBudget"));
                    break;
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = _dateTimeService.UtcNow.AddMinutes(15);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}

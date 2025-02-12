using FPECS.ISTK.Business.Services.Authentication;
using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared;
using FPECS.ISTK.Shared.Options;
using FPECS.ISTK.Shared.Requests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FPECS.ISTK.Business.Services;

public interface IAuthService
{
    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    public Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHelper _passwordHelper;
    private readonly ITokenHelper _tokenHelper;
    public AuthService(ApplicationDbContext context, IPasswordHelper? passwordHelper = null, ITokenHelper? tokenHelper = null, IOptions<JwtOptions>? jwtOptions = null)
    {
        var _jwtOptions = jwtOptions?.Value ?? new JwtOptions
        {
            Issuer = nameof(JwtOptions.Issuer),
            Audience = nameof(JwtOptions.Audience),
            Secret = nameof(JwtOptions.Secret),
            ExpirationMinutes = 10
        };
        _dbContext = context;
        _passwordHelper = passwordHelper ?? new PasswordHelper();
        _tokenHelper = tokenHelper ?? new TokenHelper(_jwtOptions);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .Include(x => x.Roles)
            .FirstAsync(x => x.Username == request.Username, cancellationToken);

        var isPasswordValid = _passwordHelper.VerifyPassword(user.PasswordHash, request.Password);
        if (!isPasswordValid)
        {
            return null;
        }

        var claims = GetClaimsByUser(user);
        var token = _tokenHelper.GenerateToken(claims);

        var response = new LoginResponse
        {
            UserId = user.Id,
            AccessToken = token
        };

        return response;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var isUserAlreadyExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Username == request.Username, cancellationToken);
        if (isUserAlreadyExists)
        {
            return false;
        }

        var hashedPassword = _passwordHelper.HashPassword(request.Password);
        var userToCreate = new UserEntity
        {
            Username = request.Username,
            PasswordHash = hashedPassword,
            FirstName = string.Empty,
            LastName = string.Empty,
            Roles = [new UserRoleEntity { Role = AvailableRole.User }]
        };

        await _dbContext.Users.AddAsync(userToCreate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static IEnumerable<Claim> GetClaimsByUser(UserEntity user)
    {
        if (user.Roles is { Count: > 0 })
        {
            foreach (var roleEntity in user.Roles)
            {
                if (roleEntity is null)
                {
                    continue;
                }
                yield return new Claim(ClaimTypes.Role, roleEntity.Role.ToString());
            }
        }
        yield return new Claim(ClaimTypes.Name, user.Username);
    }
}

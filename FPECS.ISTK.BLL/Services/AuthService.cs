using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared.Options;
using FPECS.ISTK.Shared.Requests;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FPECS.ISTK.Business.Services.Authentication;
using FPECS.ISTK.Shared;

namespace FPECS.ISTK.Business.Services;

public interface IAuthService
{
    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    public Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHelper _passwordHelper;
    public AuthService(ApplicationDbContext context, IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _dbContext = context;
        _passwordHelper = new PasswordHelper();
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
        var token = GenerateToken(claims);

        var response = new LoginResponse { AccessToken = token };

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
            Roles = [new UserRoleEntity { Role = AvailableRole.User}]
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

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

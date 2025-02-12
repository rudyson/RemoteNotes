using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Business.Services.Authentication;
using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared;
using FPECS.ISTK.Shared.Requests.Auth;
using MockQueryable.Moq;
using Moq;

namespace FPECS.ISTK.Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<ApplicationDbContext> _dbContextMock;
    private AuthService _authService;
    private Mock<IPasswordHelper> _passwordHelperMock;
    private Mock<ITokenHelper> _tokenHelperMock;

    [SetUp]
    public void SetUp()
    {
        var users = new List<UserEntity>
        {
            new() { Id = 1, Username = "user1", FirstName = "user", LastName = "system", PasswordHash = "$2a$12$TNLG5fJMr7Z0u7MB5VQrGOjH2D/bGpl.yVDHyvfCc8/w0YG36YGXK", Roles = new List<UserRoleEntity> { new() { Role = AvailableRole.User } } }
        }.AsQueryable().BuildMockDbSet();

        _dbContextMock = new Mock<ApplicationDbContext>();
        _dbContextMock.Setup(db => db.Users).Returns(users.Object);

        _passwordHelperMock = new Mock<IPasswordHelper>();

        _tokenHelperMock = new Mock<ITokenHelper>();

        _authService = new AuthService(context: _dbContextMock.Object, _passwordHelperMock.Object, _tokenHelperMock.Object);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var request = new LoginRequest { Username = "user1", Password = "password" };
        _tokenHelperMock.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>())).Returns("fakeToken");
        _passwordHelperMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var result = await _authService.LoginAsync(request, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserId, Is.EqualTo(1));
            Assert.That(result.AccessToken, Is.EqualTo("fakeToken"));
        });
    }

    [Test]
    public async Task RegisterAsync_NewUser_ReturnsTrue()
    {
        var request = new RegisterRequest { Username = "newUser", Password = "password123" };
        _dbContextMock.Setup(db => db.Users.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserEntity>)null!);
        _passwordHelperMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashedPassword123");

        var result = await _authService.RegisterAsync(request, CancellationToken.None);

        Assert.That(result, Is.True);
    }
}
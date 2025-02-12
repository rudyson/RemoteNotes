using FPECS.ISTK.Business.Services.Authentication;
using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Database;
using FPECS.ISTK.Shared.Options;
using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

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
                new() { Id = 1, Username = "user1", FirstName = "user", LastName = "system", PasswordHash = "hashedPassword123", Roles = new List<UserRoleEntity> { new() { Role = AvailableRole.User } } }
            }.AsQueryable();

        var mockSet = new Mock<DbSet<UserEntity>>();
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.Provider).Returns(users.Provider);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.Expression).Returns(users.Expression);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
        _dbContextMock.Setup(m => m.Users.FirstAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
    .ReturnsAsync(users.First());

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        _dbContextMock = new Mock<ApplicationDbContext>(options);
        _dbContextMock.Setup(db => db.Users).Returns(mockSet.Object);

        _passwordHelperMock = new Mock<IPasswordHelper>();
        _tokenHelperMock = new Mock<ITokenHelper>();

        _authService = new AuthService(_dbContextMock.Object);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "user1", Password = "password" };
        _passwordHelperMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenHelperMock.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>())).Returns("fakeToken");

        // Act
        var result = await _authService.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.UserId);
        Assert.AreEqual("fakeToken", result.AccessToken);
    }

    [Test]
    public async Task RegisterAsync_NewUser_ReturnsTrue()
    {
        // Arrange
        var request = new RegisterRequest { Username = "newUser", Password = "password123" };
        _passwordHelperMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashedPassword123");
        _dbContextMock
            .Setup(db => db.Users.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns((UserEntity user, CancellationToken _) =>
                ValueTask.FromResult((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserEntity>)null!));

        // Act
        var result = await _authService.RegisterAsync(request, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
    }
}
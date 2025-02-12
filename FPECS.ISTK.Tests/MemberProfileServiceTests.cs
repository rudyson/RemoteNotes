using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FPECS.ISTK.Tests;
[TestFixture]
public class MemberProfileServiceTests
{
    private Mock<ApplicationDbContext> _dbContextMock;
    private MemberProfileService _service;

    [SetUp]
    public void SetUp()
    {
        var users = new List<UserEntity>
            {
                new() { Id = 1, Username = "user1", FirstName = "John", LastName = "Doe", PasswordHash = string.Empty, DateOfBirth = new(2000, 1, 1), Sex = true },
                new() { Id = 2, Username = "user2", FirstName = "Jane", LastName = "Smith", PasswordHash = string.Empty, DateOfBirth = new(1995, 5, 10), Sex = false }
            }.AsQueryable();

        var mockSet = new Mock<DbSet<UserEntity>>();
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.Provider).Returns(users.Provider);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.Expression).Returns(users.Expression);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockSet.As<IQueryable<UserEntity>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        _dbContextMock = new Mock<ApplicationDbContext>(options);
        _dbContextMock.Setup(db => db.Users).Returns(mockSet.Object);

        _service = new MemberProfileService(_dbContextMock.Object);
    }

    [Test]
    public async Task GetMemberProfileAsync_ReturnsCorrectProfile()
    {
        // Act
        var result = await _service.GetMemberProfileAsync(1, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("John", result.FirstName);
    }

    [Test]
    public async Task UpdateMemberProfileAsync_UpdatesAndReturnsProfile()
    {
        // Arrange
        var request = new UpdateMemberProfileRequest { Id = 1, FirstName = "NewJohn" };

        // Act
        var result = await _service.UpdateMemberProfileAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("NewJohn", result.FirstName);
    }
}

using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using Moq;
using MockQueryable.Moq;

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
        }.AsQueryable().BuildMockDbSet();

        _dbContextMock = new Mock<ApplicationDbContext>();
        _dbContextMock.Setup(db => db.Users).Returns(users.Object);

        _service = new MemberProfileService(_dbContextMock.Object);
    }

    [Test]
    public async Task GetMemberProfileAsync_ReturnsCorrectProfile()
    {
        var result = await _service.GetMemberProfileAsync(1, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("John", result.FirstName);
    }

    [Test]
    public async Task UpdateMemberProfileAsync_UpdatesAndReturnsProfile()
    {
        var request = new UpdateMemberProfileRequest { Id = 1, FirstName = "NewJohn" };

        var result = await _service.UpdateMemberProfileAsync(request, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("NewJohn", result.FirstName);
    }
}

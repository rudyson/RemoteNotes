using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared.Requests.Notes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;

namespace FPECS.ISTK.Tests;

[TestFixture]
public class NotesServiceTests
{
    private Mock<ApplicationDbContext> _dbContextMock;
    private NotesService _notesService;

    [SetUp]
    public void SetUp()
    {
        var notes = new List<NoteEntity>
        {
            new NoteEntity { Id = 1, Title = "Test Note", Content = "Test Content", UserId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new NoteEntity { Id = 2, Title = "Another Test", Content = "Another Test Content", UserId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        }.AsQueryable().BuildMockDbSet();

        _dbContextMock = new Mock<ApplicationDbContext>();
        _dbContextMock.Setup(db => db.Notes).Returns(notes.Object);

        _notesService = new NotesService(_dbContextMock.Object);
    }

    [Test]
    public async Task CreateNoteAsync_ValidRequest_ReturnsNote()
    {
        var request = new CreateNoteRequest { Title = "New Note", Content = "New Content", MemberId = 1 };
        var mockNote = new NoteEntity
        {
            Id = 1,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = request.MemberId
        };

        var mockEntityEntry = new Mock<EntityEntry<NoteEntity>>(mockNote);
        mockEntityEntry.Setup(e => e.ReloadAsync(default)).Returns(Task.CompletedTask);

        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);


        var result = await _notesService.CreateNoteAsync(request, CancellationToken.None);


        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo("New Note"));
            Assert.That(result.Content, Is.EqualTo("New Content"));
        });
    }

    [Test]
    public void CreateNoteAsync_InvalidMemberId_ThrowsException()
    {
        var request = new CreateNoteRequest { Title = "New Note", Content = "New Content", MemberId = 9999 };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _notesService.CreateNoteAsync(request, CancellationToken.None));
    }

    [Test]
    public async Task GetNotesAsync_ValidMemberId_ReturnsNotes()
    {
        var result = await _notesService.GetNotesAsync(1, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetNoteAsync_ValidRequest_ReturnsNote()
    {
        var result = await _notesService.GetNoteAsync(1, 1, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo("Test Note"));
            Assert.That(result.Content, Is.EqualTo("Test Content"));
            Assert.That(result.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void GetNoteAsync_InvalidNoteId_ThrowsException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _notesService.GetNoteAsync(1, 9999, CancellationToken.None));
    }

    [Test]
    public async Task UpdateNoteAsync_ValidRequest_ReturnsUpdatedNote()
    {
        var request = new UpdateNoteRequest { Id = 1, MemberId = 1, Title = "Updated Title", Content = "Updated Content" };

        var result = await _notesService.UpdateNoteAsync(request, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
            Assert.That(result.Content, Is.EqualTo("Updated Content"));
            Assert.That(result.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void UpdateNoteAsync_InvalidNoteId_ThrowsException()
    {
        var request = new UpdateNoteRequest { Id = 9999, MemberId = 1, Title = "Updated Title", Content = "Updated Content" };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _notesService.UpdateNoteAsync(request, CancellationToken.None));
    }

    [Test]
    public async Task DeleteNoteAsync_ValidRequest_ReturnsTrue()
    {
        var result = await _notesService.DeleteNoteAsync(1, 1, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public void DeleteNoteAsync_InvalidNoteId_ThrowsException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _notesService.DeleteNoteAsync(1, 9999, CancellationToken.None));
    }

    [Test]
    public void DeleteNoteAsync_InvalidMemberId_ThrowsException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _notesService.DeleteNoteAsync(9999, 1, CancellationToken.None));
    }
}

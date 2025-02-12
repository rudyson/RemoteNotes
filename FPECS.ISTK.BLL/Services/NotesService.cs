using FPECS.ISTK.Database;
using FPECS.ISTK.Database.Entities;
using FPECS.ISTK.Shared.Requests.Notes;
using Microsoft.EntityFrameworkCore;

namespace FPECS.ISTK.Business.Services;

public interface INotesService
{
    Task<List<GetNoteInfoResponse>> GetNotesAsync(long memberId, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse> GetNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse> CreateNoteAsync(CreateNoteRequest request, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default);
}
public class NotesService : INotesService
{
    private readonly ApplicationDbContext _dbContext;
    public NotesService(ApplicationDbContext context)
    {
        _dbContext = context;
    }
    public async Task<GetNoteInfoResponse> CreateNoteAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var newNote = new NoteEntity
        {
            Content = request.Content,
            Title = request.Title,
            CreatedAt = now,
            UpdatedAt = now,
            UserId = request.MemberId,
        };
        var addedEntity = await _dbContext.AddAsync(newNote, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var note = addedEntity?.Entity ?? newNote;

        return new GetNoteInfoResponse
        {
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            Id = note.Id,
        };
    }

    public async Task<bool> DeleteNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default)
    {
        var note = await _dbContext.Notes.AsTracking().FirstAsync(x => x.Id == noteId && x.UserId == memberId, cancellationToken);
        _dbContext.Notes.Remove(note);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<GetNoteInfoResponse> GetNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default)
    {
        var note = await _dbContext.Notes.AsNoTracking().FirstAsync(x => x.Id == noteId && x.UserId == memberId, cancellationToken);
        return new GetNoteInfoResponse
        {
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            Id = note.Id,
        };
    }

    public Task<List<GetNoteInfoResponse>> GetNotesAsync(long memberId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Notes.AsNoTracking()
            .Where(x => x.UserId == memberId)
            .Select(note => new GetNoteInfoResponse
            {
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                Id = note.Id
            }).ToListAsync(cancellationToken);
    }

    public async Task<GetNoteInfoResponse> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var note = await _dbContext.Notes.AsTracking().FirstAsync(x => x.Id == request.Id && x.UserId == request.MemberId, cancellationToken);
        var now = DateTime.UtcNow;

        note.Title = request.Title ?? note.Title;
        note.Content = request.Content ?? note.Content;
        note.UpdatedAt = now;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GetNoteInfoResponse
        {
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            Id = note.Id,
        };
    }
}

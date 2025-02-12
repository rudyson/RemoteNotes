using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Shared.Requests.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPECS.ISTK.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly ILogger<NotesController> _logger;
    private readonly INotesService _notesService;

    public NotesController(ILogger<NotesController> logger, INotesService notesService)
    {
        _logger = logger;
        _notesService = notesService;
    }


    [HttpGet("{memberId:long}")]
    public async Task<IActionResult> GetNotes(long memberId, CancellationToken cancellationToken = default)
    {
        var notes = await _notesService.GetNotesAsync(memberId, cancellationToken);
        return Ok(notes);
    }

    [HttpGet("{memberId:long}/{noteId:long}")]
    public async Task<IActionResult> GetNote(long memberId, long noteId, CancellationToken cancellationToken = default)
    {
        var note = await _notesService.GetNoteAsync(memberId, noteId, cancellationToken);
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var note = await _notesService.CreateNoteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetNote), new { memberId = request.MemberId, noteId = note.Id }, note);
    }

    [HttpPut("{memberId:long}/{noteId:long}")]
    public async Task<IActionResult> UpdateNote([FromRoute] long memberId, [FromRoute] long noteId, [FromBody] UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        if (memberId != request.MemberId || noteId != request.Id)
        {
            return BadRequest();
        }

        var note = await _notesService.UpdateNoteAsync(request, cancellationToken);
        return Ok(note);
    }

    [HttpDelete("{memberId:long}/{noteId:long}")]
    public async Task<IActionResult> DeleteNote(long memberId, long noteId, CancellationToken cancellationToken = default)
    {
        var result = await _notesService.DeleteNoteAsync(memberId, noteId, cancellationToken);
        if (result)
        {
            return NoContent();
        }

        return BadRequest();
    }
}

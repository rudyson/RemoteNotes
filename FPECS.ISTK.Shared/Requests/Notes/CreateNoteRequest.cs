namespace FPECS.ISTK.Shared.Requests.Notes;

public class CreateNoteRequest
{
    public long MemberId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

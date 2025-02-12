namespace FPECS.ISTK.Shared.Requests.Notes;

public class UpdateNoteRequest
{
    public long MemberId { get; set; }
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}
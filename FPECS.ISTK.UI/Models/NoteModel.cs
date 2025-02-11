namespace FPECS.ISTK.UI.Models;
internal class NoteModel
{
    public long Id { get; set; }
    public required string Title { get; set; } = string.Empty;
    public required string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

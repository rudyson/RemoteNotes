namespace FPECS.ISTK.UI.Models;
internal class NoteModel
{
    public required long Id { get; set; }
    public required string Title { get; set; } = string.Empty;
    public required string Content { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
}

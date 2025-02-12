namespace FPECS.ISTK.Database.Entities;
public class NoteEntity
{
    public long Id { get; set; }

    public required string Title { get; set; }
    public required string Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public long UserId { get; set; }
    public virtual UserEntity? User { get; set; }
}

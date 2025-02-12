using FPECS.ISTK.Shared.Enums;

namespace FPECS.ISTK.Database.Entities;
public class UserEntity
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public UserStatus Status { get; set; }
    public bool? Sex { get; set; }

    public virtual List<UserRoleEntity>? Roles { get; set; }
    public virtual List<NoteEntity>? Notes { get; set; }
}
namespace FPECS.ISTK.Database.Entities;
public class UserEntity
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public virtual List<UserRoleEntity>? Roles { get; set; }
}
using FPECS.ISTK.Shared;

namespace FPECS.ISTK.Database.Entities;
public class UserRoleEntity
{
    public long UserId { get; set; }
    public UserEntity? User { get; set; }
    public required AvailableRole Role { get; set; }
}
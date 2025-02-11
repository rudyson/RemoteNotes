using FPECS.ISTK.Shared.Enums;

namespace FPECS.ISTK.UI.Models;
internal class UserModel
{
    public required long Id { get; set; }
    public required string AccessToken { get; set; }
    public required string Username { get; set; }
    public UserInfoModel? Info { get; set; }
}

internal class UserInfoModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public UserStatus Status { get; set; }
    /// <summary>
    /// True if Male, false if Female
    /// </summary>
    public required bool Sex { get; set; }
}
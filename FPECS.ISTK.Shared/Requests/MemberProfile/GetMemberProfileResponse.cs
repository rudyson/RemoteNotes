using FPECS.ISTK.Shared.Enums;

namespace FPECS.ISTK.Shared.Requests.MemberProfile;
public class GetMemberProfileResponse
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public UserStatus Status { get; set; }
    /// <summary>
    /// True if Male, false if Female
    /// </summary>
    public required bool Sex { get; set; }
}
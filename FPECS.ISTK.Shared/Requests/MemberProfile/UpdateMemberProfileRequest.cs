using FPECS.ISTK.Shared.Enums;

namespace FPECS.ISTK.Shared.Requests.MemberProfile;

public class UpdateMemberProfileRequest
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public UserStatus? Status { get; set; }
    public bool? Sex { get; set; }
}
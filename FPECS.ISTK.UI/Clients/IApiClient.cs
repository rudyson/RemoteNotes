using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared.Requests.MemberProfile;

namespace FPECS.ISTK.UI.Clients;

public interface IApiClient
{
    void SetAccessToken(string accessToken);
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<GetMemberProfileResponse?> GetMemberProfileAsync(long memberId, CancellationToken cancellationToken = default);
    Task<GetMemberProfileResponse?> UpdateMemberProfileAsync(UpdateMemberProfileRequest request, CancellationToken cancellationToken = default);
    // TODO: Update Note, Create note, delete note, get notes
}

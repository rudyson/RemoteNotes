using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using FPECS.ISTK.Shared.Requests.Notes;

namespace FPECS.ISTK.UI.Clients;

public interface IApiClient
{
    void SetAccessToken(string accessToken);
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<GetMemberProfileResponse?> GetMemberProfileAsync(long memberId, CancellationToken cancellationToken = default);
    Task<GetMemberProfileResponse?> UpdateMemberProfileAsync(UpdateMemberProfileRequest request, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse?> CreateNoteAsync(CreateNoteRequest request, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse?> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default);
    Task<GetNoteInfoResponse?> GetNoteAsync(long memberId, long noteId, CancellationToken cancellationToken = default);
    Task<List<GetNoteInfoResponse>?> GetNotesAsync(long memberId, CancellationToken cancellationToken = default);
}

using FPECS.ISTK.Shared.Requests;

namespace FPECS.ISTK.UI.Clients;

public interface IApiClient
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}

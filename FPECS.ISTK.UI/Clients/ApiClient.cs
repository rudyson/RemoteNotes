using FPECS.ISTK.Shared.Requests;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FPECS.ISTK.UI.Clients;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string? baseAddress = null)
    {
        baseAddress ??= "http://localhost:5000";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("auth/login", content, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<LoginResponse>(responseContent);
        }

        return null;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("auth/register", content, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
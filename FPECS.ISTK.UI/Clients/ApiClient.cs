using FPECS.ISTK.Shared.Requests;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FPECS.ISTK.UI.Clients;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiClient(string? baseAddress = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        baseAddress ??= "https://localhost:5000";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/auth/login", content, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonSerializerOptions);
        }

        return null;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/auth/register", content, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
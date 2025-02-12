using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FPECS.ISTK.UI.Clients;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiClient(string? baseAddress = null, string? accessToken = null,  JsonSerializerOptions? jsonSerializerOptions = null)
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
            Converters = { new JsonStringEnumConverter()}
        };
        if (!string.IsNullOrEmpty(accessToken))
        {
            SetAccessToken(accessToken);
        }
    }
    public void SetAccessToken(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

    public async Task<GetMemberProfileResponse?> GetMemberProfileAsync(long memberId, CancellationToken cancellationToken = default)
    {
        var url = $"api/member/{memberId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GetMemberProfileResponse>(responseContent, _jsonSerializerOptions);
    }
    public async Task<GetMemberProfileResponse?> UpdateMemberProfileAsync(UpdateMemberProfileRequest request, CancellationToken cancellationToken = default)
    {
        var url = $"api/member/{request.Id}";
        var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var deserializedResponse = JsonSerializer.Deserialize<GetMemberProfileResponse>(responseContent, _jsonSerializerOptions);
        return deserializedResponse;
    }
}
namespace FPECS.ISTK.Shared.Requests.Auth;

public class LoginResponse
{
    public required long UserId { get; set; }
    public required string AccessToken { get; set; }
}

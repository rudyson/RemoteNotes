namespace FPECS.ISTK.Shared.Requests;

public class LoginResponse
{
    public required long UserId { get; set; }
    public required string AccessToken { get; set; }
}

using FPECS.ISTK.Database.Entities;

namespace FPECS.ISTK.Business.Services.Authentication;

public interface IPasswordHelper
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string password);
}

public class PasswordHelper : IPasswordHelper
{
    private const int WorkFactor = 12;
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

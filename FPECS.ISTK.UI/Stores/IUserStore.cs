using FPECS.ISTK.UI.Models;

namespace FPECS.ISTK.UI.Stores;
internal interface IUserStore
{
    UserModel? CurrentUser { get; }
    bool IsLoggedIn { get; }
    string? GetAccessToken();
    long? GetId();
    void Login(UserModel user);
    void Logout();
}
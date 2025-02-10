using FPECS.ISTK.UI.Models;
using System.ComponentModel;

namespace FPECS.ISTK.UI.Stores;

internal class UserStore : INotifyPropertyChanged
{
    private UserModel? _currentUser;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsLoggedIn => _currentUser != null;
    public UserModel? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(IsLoggedIn));
        }
    }

    public void Login(UserModel user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public string? GetAccessToken() => CurrentUser?.AccessToken;

    public long? GetId() => CurrentUser?.Id;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.Windows;

namespace FPECS.ISTK.UI.ViewModels;
internal class LoginViewModel : BaseViewModel
{
    private readonly ApplicationStore _store;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IApiClient _apiClient;

    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand LoginButtonCommand => new(async _ => await ExecuteLoginAsync(), canExecute => CanExecuteLogin);
    public RelayCommand RegisterButtonCommand => new(async _ => await ExecuteRegisterAsync(), canExecute => CanExecuteRegister);

    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
            RefreshCanExecute();
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
            RefreshCanExecute();
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
            RefreshCanExecute();
        }
    }

    private void RefreshCanExecute()
    {
        LoginButtonCommand.RaiseCanExecuteChanged();
        RegisterButtonCommand.RaiseCanExecuteChanged();
    }

    private string _validationMessage;
    public string ValidationMessage
    {
        get => _validationMessage;
        set
        {
            _validationMessage = value;
            OnPropertyChanged(nameof(ValidationMessage));
            OnPropertyChanged(nameof(HasError));
        }
    }
    public bool HasError => !string.IsNullOrWhiteSpace(ValidationMessage);

    public bool CanExecuteLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !_store.IsLoggedIn && !IsLoading;
    public bool CanExecuteRegister => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !_store.IsLoggedIn && !IsLoading;

    public LoginViewModel(ApplicationStore store, RelayCommand updateViewCommand)
    {
        _store = store;
        _apiClient = new ApiClient();
        UpdateViewCommand = updateViewCommand;
    }

    private async Task ExecuteLoginAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        var cancellationToken = GetCancellationTokenAndCancelPreviousOperation();

        try
        {
            var request = new LoginRequest { Username = Username, Password = Password };
            var response = await _apiClient.LoginAsync(request, cancellationToken);

            if (response is null)
            {
                ValidationMessage = "Username or password wrong";
                return;
            }
            else
            {
                ValidationMessage = string.Empty;
            }

            _apiClient.SetAccessToken(response.AccessToken);
            var profile = await _apiClient.GetMemberProfileAsync(response.UserId, cancellationToken);

            var userModel = new UserModel
            {
                AccessToken = response.AccessToken,
                Id = response.UserId,
                Username = Username
            };

            if (profile is not null)
            {
                var info = new UserInfoModel
                {
                    Sex = profile.Sex,
                    DateOfBirth = profile.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                    FirstName = profile.FirstName,
                    LastName = profile.LastName
                };
                userModel.Info = info;
            }

            _store.Login(userModel);
            await _store.LoadNotesAsync(_apiClient, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            ValidationMessage = "Login was cancelled.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteRegisterAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        var cancellationToken = GetCancellationTokenAndCancelPreviousOperation();

        try
        {
            var request = new RegisterRequest { Username = Username, Password = Password };
            var isRegistered = await _apiClient.RegisterAsync(request, cancellationToken);

            if (isRegistered)
            {
                MessageBox.Show("Now please login again with provided credentials", "User registered successfully!", MessageBoxButton.OK, MessageBoxImage.Information);
                ValidationMessage = string.Empty;
            }
            else
            {
                ValidationMessage = "This user already exists. Try another username.";
            }            
        }
        catch (TaskCanceledException)
        {
            ValidationMessage = "Registration was cancelled.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private CancellationToken GetCancellationTokenAndCancelPreviousOperation()
    {
        _cancellationTokenSource?.Cancel();

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return cancellationToken;
    }
}
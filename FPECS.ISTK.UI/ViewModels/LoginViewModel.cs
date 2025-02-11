using FPECS.ISTK.Shared.Enums;
using FPECS.ISTK.Shared.Requests;
using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.Threading;

namespace FPECS.ISTK.UI.ViewModels;
internal class LoginViewModel : BaseViewModel
{
    private readonly NoteStore _noteStore;
    private readonly UserStore _userStore;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IApiClient _apiClient;

    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand LoginButtonCommand => new(async _ => await ExecuteLoginAsync(), canExecute => CanExecuteLogin);
    public RelayCommand RegisterButtonCommand => new(async _ => await ExecuteRegisterAsync(), canExecute => CanExecuteRegister);

    private string _validationMessage;
    private string _username;
    private string _password;
    private bool _isLoading;

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

    public string ValidationMessage
    {
        get => _validationMessage;
        set
        {
            _validationMessage = value;
            OnPropertyChanged(nameof(ValidationMessage));
        }
    }

    public bool CanExecuteLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !_userStore.IsLoggedIn && !IsLoading;
    public bool CanExecuteRegister => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !_userStore.IsLoggedIn && !IsLoading;

    public LoginViewModel(NoteStore noteStore, UserStore userStore, RelayCommand updateViewCommand)
    {
        _noteStore = noteStore;
        _userStore = userStore;
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
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            var request = new LoginRequest { Username = Username, Password = Password };
            var response = await _apiClient.LoginAsync(request, cancellationToken);
            if (response is null)
            {
                ValidationMessage = "Username or password wrong";
                return;
            }
            var userModel = new UserModel
            {
                AccessToken = response.AccessToken,
                Id = response.UserId,
                Username = Username
            };
            _userStore.Login(userModel);
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
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(2000, _cancellationTokenSource.Token);

            ValidationMessage = "User registered successfully!";
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
}
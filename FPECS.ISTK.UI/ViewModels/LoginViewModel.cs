using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;

namespace FPECS.ISTK.UI.ViewModels;
internal class LoginViewModel : BaseViewModel
{
    private readonly NoteStore _noteStore;
    private readonly UserStore _userStore;
    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand LoginButtonCommand => new(execute => ExecuteLogin(), canExecute => CanExecuteLogin);
    public RelayCommand RegisterButtonCommand => new(execute => ExecuteRegister(), canExecute => CanExecuteRegister);
    public string ValidationMessage { get; set; } = string.Empty;
    public bool CanExecuteLogin => !_userStore.IsLoggedIn;
    public bool CanExecuteRegister => _userStore.IsLoggedIn;

    public LoginViewModel(NoteStore noteStore, UserStore userStore, RelayCommand updateViewCommand)
    {
        _noteStore = noteStore;
        _userStore = userStore;
        UpdateViewCommand = updateViewCommand;
    }

    private void ExecuteLogin()
    {
        var user = new UserModel { AccessToken = "", Id = 1, Username = "string" };
        _userStore.Login(user);
    }
    private void ExecuteRegister()
    {
        _userStore.Logout();
    }
}
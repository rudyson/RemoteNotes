using FPECS.ISTK.Shared.Enums;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.Collections.ObjectModel;

namespace FPECS.ISTK.UI.ViewModels;
internal class MemberProfileViewModel : BaseViewModel
{
    private bool _isLoading;
    private readonly NoteStore _noteStore;
    private readonly UserStore _userStore;
    private UserInfoModel _model;

    public RelayCommand UpdateViewCommand { get; set; }

    public DateTime DateOfBirth { 
        get
        {
            return _model.DateOfBirth;
        }
        set
        {
            _model.DateOfBirth = value;
            OnPropertyChanged(nameof(DateOfBirth));
        }
    }
    public bool Sex
    {
        get
        {
            return _model.Sex;
        }
        set
        {
            _model.Sex = value;
            OnPropertyChanged(nameof(Sex));
        }
    }
    public string FirstName
    {
        get
        {
            return _model.FirstName;
        }
        set
        {
            _model.FirstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }
    public string LastName
    {
        get
        {
            return _model.LastName;
        }
        set
        {
            _model.LastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }
    public UserStatus Status
    {
        get
        {
            return _model.Status;
        }
        set
        {
            _model.Status = value;
            OnPropertyChanged(nameof(Status));
        }
    }
    public ObservableCollection<UserStatus> UserStatuses { get; } = new ObservableCollection<UserStatus>(Enum.GetValues(typeof(UserStatus)).Cast<UserStatus>());

    public MemberProfileViewModel(NoteStore noteStore, UserStore userStore, RelayCommand UpdateViewCommand)
    {
        _noteStore = noteStore;
        _userStore = userStore;
        this.UpdateViewCommand = UpdateViewCommand;

        if (_userStore.CurrentUser?.Info is not null)
        {
            var currentUserInfo = _userStore.CurrentUser.Info;
            _model = new UserInfoModel
            {
                DateOfBirth = currentUserInfo.DateOfBirth,
                Sex = currentUserInfo.Sex,
                FirstName = currentUserInfo.FirstName,
                LastName = currentUserInfo.LastName,
                Status = currentUserInfo.Status,
            };
        }
        else
        {
            _model = new UserInfoModel
            {
                DateOfBirth = DateTime.UtcNow,
                Sex = false,
                FirstName = string.Empty,
                LastName = string.Empty,
                Status = UserStatus.Student,
            };
        }
    }

    public RelayCommand UpdateProfileCommand => new(async execute => await UpdateProfileAsync(), canExecute => CanExecuteUpdateProfile);

    private async Task UpdateProfileAsync()
    {
        _userStore.CurrentUser!.Info = _model;
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }
    private bool CanExecuteUpdateProfile => true;
}

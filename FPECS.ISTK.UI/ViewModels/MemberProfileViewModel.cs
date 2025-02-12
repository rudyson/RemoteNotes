using FPECS.ISTK.Shared.Enums;
using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.Collections.ObjectModel;
using System.Windows;

namespace FPECS.ISTK.UI.ViewModels;
internal class MemberProfileViewModel : BaseViewModel
{
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly ApplicationStore _store;
    private UserInfoModel _model;
    private readonly IApiClient _apiClient;

    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand UpdateProfileCommand => new(async execute => await UpdateProfileAsync(), canExecute => CanExecuteUpdateProfile);
    public RelayCommand DiscardChangesCommand => new(execute => DiscardChanges(), canExecute => CanExecuteDiscardChanges);

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
            OnPropertyChanged(nameof(SexAsText));
        }
    }
    public string SexAsText => _model.Sex ? "Male" : "Female";

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
        UpdateProfileCommand.RaiseCanExecuteChanged();
        DiscardChangesCommand.RaiseCanExecuteChanged();
    }

    public ObservableCollection<UserStatus> UserStatuses { get; } = new ObservableCollection<UserStatus>(Enum.GetValues(typeof(UserStatus)).Cast<UserStatus>());

    public MemberProfileViewModel(ApplicationStore store, RelayCommand UpdateViewCommand)
    {
        _store = store;
        this.UpdateViewCommand = UpdateViewCommand;
        _apiClient = new ApiClient(accessToken: _store.GetAccessToken());

        if (_store.CurrentUser?.Info is not null)
        {
            var currentUserInfo = _store.CurrentUser.Info;
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

    private async Task UpdateProfileAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        var cancellationToken = GetCancellationTokenAndCancelPreviousOperation();

        try
        {
            var request = new UpdateMemberProfileRequest
            {
                Id = _store.GetId()!.Value,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                Sex = Sex,
                Status = Status
            };
            var getMemberProfileResponse = await _apiClient.UpdateMemberProfileAsync(request, cancellationToken);

            if (getMemberProfileResponse is null)
            {
                MessageBox.Show("Try again later", "Unable to update profile", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                _store.CurrentUser!.Info = _model;
                MessageBox.Show("Profile updated.", "Profile updated.", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateViewCommand.Execute(nameof(NotesViewModel));
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Try again later", "Unable to update profile", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }
    private bool CanExecuteUpdateProfile => !IsLoading;

    public bool CanExecuteDiscardChanges => !IsLoading;
    private void DiscardChanges()
    {
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }

    private CancellationToken GetCancellationTokenAndCancelPreviousOperation()
    {
        _cancellationTokenSource?.Cancel();

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return cancellationToken;
    }
}

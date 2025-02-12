using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.ComponentModel;
using System.Windows;

namespace FPECS.ISTK.UI.ViewModels;
internal class NotesViewModel : BaseViewModel
{
    public string UserInfo => $"Logged in as {_store.CurrentUser?.Username} (Id: {_store.CurrentUser?.Id})";

    public RelayCommand ResetFiltersCommand => new(execute => ResetFilters(), canExecute => CanExecuteResetFilters);
    public RelayCommand DeleteNoteCommand => new(async execute => await DeleteNoteAsync(), canExecute => CanExecuteDeleteNote);
    public RelayCommand EditNoteCommand => new(execute => EditNote(), canExecute => CanExecuteEditNote);
    public RelayCommand LogoutCommand => new(execute => Logout(), canExecute => CanExecuteLogoutCommand);
    public RelayCommand NavigateMemberProfileCommand => new(execute => NavigateMemberProfile(), canExecute => CanExecuteNavigateMemberProfile);
    public RelayCommand UpdateViewCommand { get; set; }

    private CancellationTokenSource _cancellationTokenSource;

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            FilteredNotes.Refresh();
        }
    }
    private DateTime? _selectedDate;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            _selectedDate = value;
            OnPropertyChanged(nameof(SelectedDate));
            FilteredNotes.Refresh();
        }
    }

    private NoteModel? _selectedNote;
    public NoteModel? SelectedNote
    {
        get => _selectedNote;
        set
        {
            _selectedNote = value;
            OnPropertyChanged(nameof(SelectedNote));
        }
    }
    private readonly ApplicationStore _store;
    public ICollectionView FilteredNotes => _store.FilteredNotes;
    public ICollection<NoteModel> Notes => _store.Notes;
    private readonly IApiClient _apiClient;

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
        DeleteNoteCommand.RaiseCanExecuteChanged();
        EditNoteCommand.RaiseCanExecuteChanged();
        LogoutCommand.RaiseCanExecuteChanged();
        NavigateMemberProfileCommand.RaiseCanExecuteChanged();
        ResetFiltersCommand.RaiseCanExecuteChanged();
    }

    public NotesViewModel(ApplicationStore store, RelayCommand updateViewCommand)
    {
        _store = store;
        _store.FilteredNotes.Filter = FilterNotes;
        _apiClient = new ApiClient(accessToken: _store.GetAccessToken());
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateViewCommand = updateViewCommand;
    }

    private bool FilterNotes(object item)
    {
        if (item is not NoteModel note)
        {
            return false;
        }

        var isTextMatches = string.IsNullOrEmpty(SearchText) || note.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var isDateMatches = !SelectedDate.HasValue || note.CreatedAt.Date == SelectedDate.Value.Date;

        return isTextMatches && isDateMatches;
    }

    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedDate = default;
        SelectedNote = null;
    }

    private async Task DeleteNoteAsync()
    {
        var noteId = SelectedNote!.Id;
        var memberId = _store.GetId() ?? default;
        var cancelationToken = GetCancellationTokenAndCancelPreviousOperation();

        var result = MessageBox.Show("Are you sure?", $"Confirmation (Note {noteId})", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            var isDeleted = await _apiClient.DeleteNoteAsync(memberId, noteId, cancelationToken);
            if (isDeleted)
            {
                _store.RemoveNote(SelectedNote!.Id);
            }
            else
            {
                MessageBox.Show("Unable to delete note", $"Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void EditNote()
    {
        UpdateViewCommand.Execute(new object[] { nameof(AddNoteViewModel), SelectedNote! });
    }

    private void Logout()
    {
        _store.Logout();
    }

    private void NavigateMemberProfile()
    {
        UpdateViewCommand.Execute(nameof(MemberProfileViewModel));
    }

    private bool CanExecuteResetFilters => !string.IsNullOrEmpty(SearchText) || SelectedDate.HasValue && !IsLoading;
    private bool CanExecuteDeleteNote => SelectedNote is { Id: > 0 } && !IsLoading;
    private bool CanExecuteEditNote => SelectedNote is { Id: > 0 } && !IsLoading;

    public bool CanExecuteLogoutCommand => _store.IsLoggedIn && !IsLoading;
    public bool CanExecuteNavigateMemberProfile => _store.IsLoggedIn && !IsLoading;

    private CancellationToken GetCancellationTokenAndCancelPreviousOperation()
    {
        _cancellationTokenSource?.Cancel();

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return cancellationToken;
    }
}

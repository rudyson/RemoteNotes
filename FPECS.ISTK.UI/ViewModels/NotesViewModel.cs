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
    public RelayCommand DeleteNoteCommand => new(execute => DeleteNote(), canExecute => CanExecuteDeleteNote);
    public RelayCommand EditNoteCommand => new(execute => EditNote(), canExecute => CanExecuteEditNote);

    public RelayCommand LogoutCommand => new(execute => Logout(), canExecute => _store.IsLoggedIn);
    public RelayCommand NavigateMemberProfileCommand => new(execute => NavigateMemberProfile(), canExecute => _store.IsLoggedIn);

    public RelayCommand UpdateViewCommand { get; set; }

    private string _searchText = string.Empty;
    private DateTime? _selectedDate;
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

    public NotesViewModel(ApplicationStore store, RelayCommand updateViewCommand)
    {
        _store = store;
        _store.FilteredNotes.Filter = FilterNotes;

        UpdateViewCommand = updateViewCommand;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _store.LoadNotesAsync();
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

    private void DeleteNote()
    {
        var result = MessageBox.Show("Are you sure?", $"Confirmation (Note {SelectedNote!.Id})", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _store.RemoveNote(SelectedNote!.Id);
        }
    }

    private void EditNote()
    {
        if (SelectedNote is null)
        {
            return;
        }
        UpdateViewCommand.Execute(new object[] { nameof(AddNoteViewModel), SelectedNote });
    }

    private void Logout()
    {
        _store.Logout();
    }

    private void NavigateMemberProfile()
    {
        UpdateViewCommand.Execute(nameof(MemberProfileViewModel));
    }

    private bool CanExecuteResetFilters => !string.IsNullOrEmpty(SearchText) || SelectedDate.HasValue;
    private bool CanExecuteDeleteNote => SelectedNote is { Id: > 0 };
    private bool CanExecuteEditNote => SelectedNote is { Id: > 0 };
}

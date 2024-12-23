using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.ComponentModel;

namespace FPECS.ISTK.UI.ViewModels;
internal class NotesViewModel : INotifyPropertyChanged
{
    public RelayCommand ResetFiltersCommand => new(execute => ResetFilters(), canExecute => CanExecuteResetFilters);
    public RelayCommand DeleteNoteCommand => new(execute => DeleteNote(), canExecute => CanExecuteDeleteNote);
    public RelayCommand CreateNoteCommand => new(execute => CreateNote(), canExecute => CanExecuteCreateNote);
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
    private readonly NoteStore _noteStore;
    public ICollectionView FilteredNotes => _noteStore.FilteredNotes;

    private string _newNoteTitle = string.Empty;
    public string NewNoteTitle
    {
        get => _newNoteTitle;
        set
        {
            _newNoteTitle = value;
            OnPropertyChanged(nameof(NewNoteTitle));
        }
    }

    private string _newNoteContent = string.Empty;
    public string NewNoteContent
    {
        get => _newNoteContent;
        set
        {
            _newNoteContent = value;
            OnPropertyChanged(nameof(NewNoteContent));
        }
    }

    public NotesViewModel(NoteStore noteStore)
    {
        _noteStore = noteStore;
        _noteStore.FilteredNotes.Filter = FilterNotes;
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
        _noteStore.RemoveNote(_selectedNote!.Id);
    }

    private void CreateNote()
    {
        var newNote = new NoteModel()
        {
            Id = default,
            Content = _newNoteContent!,
            CreatedAt = default,
            Title = _newNoteTitle!
        };

        _noteStore.AddNote(newNote);

        NewNoteTitle = string.Empty;
        NewNoteContent = string.Empty;

        OnPropertyChanged(nameof(NewNoteTitle));
        OnPropertyChanged(nameof(NewNoteContent));

        FilteredNotes.Refresh();
    }

    private bool CanExecuteResetFilters => !string.IsNullOrEmpty(SearchText) || SelectedDate.HasValue;
    private bool CanExecuteDeleteNote => _selectedNote is { Id: > 0 };
    private bool CanExecuteCreateNote => !string.IsNullOrEmpty(_newNoteTitle) && !string.IsNullOrEmpty(_newNoteContent);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

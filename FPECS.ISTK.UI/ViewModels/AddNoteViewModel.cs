using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;

namespace FPECS.ISTK.UI.ViewModels;
internal class AddNoteViewModel : BaseViewModel
{
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

    private readonly NoteStore _noteStore;
    public RelayCommand UpdateViewCommand { get; set; }
    public AddNoteViewModel(NoteStore noteStore, RelayCommand UpdateViewCommand)
    {
        _noteStore = noteStore;
        this.UpdateViewCommand = UpdateViewCommand;
    }

    public RelayCommand CreateNoteCommand => new(execute => CreateNote(), canExecute => CanExecuteCreateNote);

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

        _noteStore.FilteredNotes.Refresh();
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }
    private bool CanExecuteCreateNote => !string.IsNullOrEmpty(_newNoteTitle) && !string.IsNullOrEmpty(_newNoteContent);
}

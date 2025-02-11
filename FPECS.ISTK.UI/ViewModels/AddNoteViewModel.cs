using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;

namespace FPECS.ISTK.UI.ViewModels;
internal class AddNoteViewModel : BaseViewModel
{
    private readonly NoteModel _model;
    public bool IsEditMode => _model is { Id: > 0 };
    public string CreateNoteButtonText => _model.Id > 0 ? "Update note" : "Add note";
    public string CreateNoteTitleText => _model.Id > 0 ? "Update note" : "Create note";
    public string NewNoteTitle
    {
        get => _model.Title;
        set
        {
            _model.Title = value;
            OnPropertyChanged(nameof(NewNoteTitle));
        }
    }

    public string NewNoteContent
    {
        get => _model.Content;
        set
        {
            _model.Content = value;
            OnPropertyChanged(nameof(NewNoteContent));
        }
    }

    private readonly NoteStore _noteStore;
    private readonly UserStore _userStore;
    public RelayCommand UpdateViewCommand { get; set; }
    public AddNoteViewModel(NoteStore noteStore, UserStore userStore, RelayCommand UpdateViewCommand, object? model = null)
    {
        _noteStore = noteStore;
        _userStore = userStore;
        this.UpdateViewCommand = UpdateViewCommand;
        if (model is NoteModel noteModel) {
            _model = new NoteModel
            {
                Id = noteModel.Id,
                CreatedAt = noteModel.CreatedAt,
                Content = noteModel.Content,
                Title = noteModel.Title
            };
        }
        else
        {
            _model = new NoteModel
            {
                Id = default,
                CreatedAt = DateTime.MinValue,
                Content = string.Empty,
                Title = string.Empty
            };
        }
    }

    public RelayCommand CreateNoteCommand => new(execute => CreateNote(), canExecute => CanExecuteCreateNote);

    private void CreateNote()
    {
        if (IsEditMode)
        {
            _noteStore.UpdateNote(_model);
        }
        else
        {
            _noteStore.AddNote(_model);
        }

        _noteStore.FilteredNotes.Refresh();
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }
    private bool CanExecuteCreateNote => !string.IsNullOrEmpty(NewNoteTitle) && !string.IsNullOrEmpty(NewNoteContent);
}

using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Stores;

namespace FPECS.ISTK.UI.ViewModels;
internal class MainViewModel : BaseViewModel
{
    private readonly NoteStore _noteStore;
    public MainViewModel()
    {
        _noteStore = new NoteStore();
        _selectedViewModel = new NotesViewModel(_noteStore, UpdateViewCommand);
    }
    private BaseViewModel _selectedViewModel;
    public BaseViewModel SelectedViewModel
    {
        get { return _selectedViewModel; }
        set
        {
            _selectedViewModel = value;
            OnPropertyChanged(nameof(SelectedViewModel));
        }
    }

    public RelayCommand UpdateViewCommand => new(
        execute: (object? viewKey) => HandleNavigate(viewKey?.ToString() ?? string.Empty),
        canExecute: canExecute => true
        );

    public void HandleNavigate(string viewKey)
    {
        BaseViewModel nextView = viewKey switch
        {
            nameof(AddNoteViewModel) => new AddNoteViewModel(_noteStore, UpdateViewCommand),
            nameof(NotesViewModel) => new NotesViewModel(_noteStore, UpdateViewCommand),
            nameof(LoginViewModel) => new LoginViewModel(_noteStore, UpdateViewCommand),
            _ => throw new NotImplementedException()
        };

        SelectedViewModel = nextView;
    }
}

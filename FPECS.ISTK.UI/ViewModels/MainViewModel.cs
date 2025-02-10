using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Stores;
using System.ComponentModel;

namespace FPECS.ISTK.UI.ViewModels;
internal class MainViewModel : BaseViewModel, IDisposable
{
    private readonly NoteStore _noteStore;
    private readonly UserStore _userStore;
    private bool _disposed = false;
    public MainViewModel()
    {
        _noteStore = new NoteStore();
        _userStore = new UserStore();
        _userStore.PropertyChanged += OnUserStoreChanged;
        _selectedViewModel = GetViewModelByNavigationKey(nameof(LoginViewModel));
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
        SelectedViewModel = GetViewModelByNavigationKey(viewKey);
    }

    private BaseViewModel GetViewModelByNavigationKey(string navigationKey)
    {
        BaseViewModel viewModel = navigationKey switch
        {
            nameof(AddNoteViewModel) => new AddNoteViewModel(_noteStore, _userStore, UpdateViewCommand),
            nameof(NotesViewModel) => new NotesViewModel(_noteStore, _userStore, UpdateViewCommand),
            nameof(LoginViewModel) => new LoginViewModel(_noteStore, _userStore, UpdateViewCommand),
            _ => throw new NotImplementedException()
        };

        return viewModel;
    }

    private void OnUserStoreChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UserStore.IsLoggedIn))
        {
            UpdateView();
        }
    }

    private void UpdateView()
    {
        var viewModelNavigationKey = _userStore.IsLoggedIn ? nameof(NotesViewModel) : nameof(LoginViewModel);
        SelectedViewModel = GetViewModelByNavigationKey(viewModelNavigationKey);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _userStore.PropertyChanged -= OnUserStoreChanged;
        }

        _disposed = true;
    }

    ~MainViewModel()
    {
        Dispose(false);
    }
}

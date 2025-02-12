using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.ComponentModel;

namespace FPECS.ISTK.UI.ViewModels;
internal class MainViewModel : BaseViewModel, IDisposable
{
    private readonly ApplicationStore _store;
    private bool _disposed = false;
    public MainViewModel()
    {
        _store = new ApplicationStore();
        _store.PropertyChanged += OnUserStoreChanged;
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

    public RelayCommand UpdateViewCommand => new(execute: HandleNavigate, canExecute: canExecute => true);

    public void HandleNavigate(object? data = null)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data is string navigationKey)
        {
            SelectedViewModel = GetViewModelByNavigationKey(navigationKey);
        }
        else if (data is object[] paramArray && paramArray.Length > 0 && paramArray[0] is string navigationKeyAsParam)
        {
            var dataAsParam = paramArray.Length > 1 ? paramArray[1] : null;
            SelectedViewModel = GetViewModelByNavigationKey(navigationKeyAsParam, dataAsParam);
        }
        else
        {
            throw new ArgumentException($"Invalid navigation parameter: {data}");
        }
    }

    private BaseViewModel GetViewModelByNavigationKey(string navigationKey, object? data = null)
    {
        BaseViewModel viewModel = navigationKey switch
        {
            nameof(AddNoteViewModel) when data is NoteModel => new AddNoteViewModel(_store, UpdateViewCommand, data),
            nameof(AddNoteViewModel) when data is not NoteModel => new AddNoteViewModel(_store, UpdateViewCommand),
            nameof(NotesViewModel) => new NotesViewModel(_store, UpdateViewCommand),
            nameof(LoginViewModel) => new LoginViewModel(_store, UpdateViewCommand),
            nameof(MemberProfileViewModel) => new MemberProfileViewModel(_store, UpdateViewCommand),
            _ => throw new NotImplementedException()
        };

        return viewModel;
    }

    private void OnUserStoreChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ApplicationStore.IsLoggedIn))
        {
            UpdateView();
        }
    }

    private void UpdateView()
    {
        var viewModelNavigationKey = _store.IsLoggedIn ? nameof(NotesViewModel) : nameof(LoginViewModel);
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
            _store.PropertyChanged -= OnUserStoreChanged;
        }

        _disposed = true;
    }

    ~MainViewModel()
    {
        Dispose(false);
    }
}

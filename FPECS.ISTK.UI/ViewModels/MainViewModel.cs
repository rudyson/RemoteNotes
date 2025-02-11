using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection.Metadata;

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
        execute: (object? viewKey) => HandleNavigate(viewKey),
        canExecute: canExecute => true
        );

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
            nameof(AddNoteViewModel) when data is NoteModel => new AddNoteViewModel(_noteStore, _userStore, UpdateViewCommand, data),
            nameof(AddNoteViewModel) when data is not NoteModel => new AddNoteViewModel(_noteStore, _userStore, UpdateViewCommand),
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

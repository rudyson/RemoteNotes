using FPECS.ISTK.Shared.Requests.Notes;
using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using System.Threading;
using System.Windows;

namespace FPECS.ISTK.UI.ViewModels;
internal class AddNoteViewModel : BaseViewModel
{
    private readonly NoteModel _model;
    public bool IsEditMode => _model is { Id: > 0 };
    public string CreateNoteButtonText => IsEditMode ? "Update note" : "Add note";
    public string CreateNoteTitleText => IsEditMode ? "Update note" : "Create note";
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
        SaveNoteCommand.RaiseCanExecuteChanged();
        DiscardChangesCommand.RaiseCanExecuteChanged();
    }

    private readonly ApplicationStore _store;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly IApiClient _apiClient;
    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand SaveNoteCommand => new(async execute => await SaveNoteAsync(), canExecute => CanExecuteSaveNote);
    public RelayCommand DiscardChangesCommand => new(execute => DiscardChanges(), canExecute => CanExecuteDiscardChanges);
    public AddNoteViewModel(ApplicationStore store, RelayCommand UpdateViewCommand, object? model = null)
    {
        _store = store;
        _apiClient = new ApiClient(accessToken: _store.GetAccessToken());
        this.UpdateViewCommand = UpdateViewCommand;
        _cancellationTokenSource = new CancellationTokenSource();

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

    private void DiscardChanges()
    {
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }

    private async Task SaveNoteAsync()
    {
        var cancellationToken = GetCancellationTokenAndCancelPreviousOperation();
        IsLoading = true;
        try
        {
            if (IsEditMode)
            {
                await UpdateNoteAsync(cancellationToken);
            }
            else
            {
                await CreateNoteAsync(cancellationToken);
            }
        }
        catch
        {
            MessageBox.Show("Error", "Unable to save note", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateNoteAsync(CancellationToken cancellationToken)
    {
        var memberId = _store.GetId() ?? default;
        var request = new CreateNoteRequest
        {
            MemberId = memberId,
            Title = _model.Title,
            Content = _model.Content,
        };
        var note = await _apiClient.CreateNoteAsync(request, cancellationToken);
        if (note is null)
        {
            MessageBox.Show("Error", "Unable to create note", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            var noteToStore = new NoteModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
            };
            _store.AddNote(noteToStore);
            UpdateParentViewAndNavigate();
        }
    }

    private async Task UpdateNoteAsync(CancellationToken cancellationToken)
    {
        var memberId = _store.GetId() ?? default;
        var request = new UpdateNoteRequest
        {
            Id = _model.Id,
            MemberId = memberId,
            Title = _model.Title,
            Content = _model.Content,
        };
        var note = await _apiClient.UpdateNoteAsync(request, cancellationToken);
        if (note is null)
        {
            MessageBox.Show("Error", "Unable to update note", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            var noteToStore = new NoteModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
            };
            _store.UpdateNote(noteToStore);
            UpdateParentViewAndNavigate();
        }
    }

    private void UpdateParentViewAndNavigate()
    {
        _store.FilteredNotes.Refresh();
        UpdateViewCommand.Execute(nameof(NotesViewModel));
    }
    private bool CanExecuteSaveNote => !string.IsNullOrEmpty(NewNoteTitle) && !string.IsNullOrEmpty(NewNoteContent) && !IsLoading;
    private bool CanExecuteDiscardChanges => !IsLoading;
    private CancellationToken GetCancellationTokenAndCancelPreviousOperation()
    {
        _cancellationTokenSource?.Cancel();

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return cancellationToken;
    }
}

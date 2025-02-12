using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Stores;

internal class ApplicationStore : INotifyPropertyChanged, IUserStore
{
    public ApplicationStore()
    {
        Notes = new ObservableCollection<NoteModel>();
        FilteredNotes = CollectionViewSource.GetDefaultView(Notes);
    }
    private UserModel? _currentUser;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsLoggedIn => _currentUser != null;
    public UserModel? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(IsLoggedIn));
        }
    }

    public void Login(UserModel user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
        Notes.Clear();
    }

    public string? GetAccessToken() => CurrentUser?.AccessToken;

    public long? GetId() => CurrentUser?.Id;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<NoteModel> Notes { get; set; }
    public ICollectionView FilteredNotes { get; }

    public async Task LoadNotesAsync(IApiClient client, CancellationToken cancellationToken = default)
    {
        var userId = GetId();
        if (!userId.HasValue)
        {
            return;
        }

        var notes = await client.GetNotesAsync(userId.Value, cancellationToken);

        if (notes != null)
        {
            var notesToAdd = notes.Select(note => new NoteModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
            }).ToList();
            LoadNotes(notesToAdd);
        }
    }

    public void LoadNotes(List<NoteModel> notes)
    {
        Notes.Clear();
        foreach (var note in notes)
        {
            Notes.Add(note);
        }
    }

    public NoteModel AddNote(NoteModel note)
    {
        if (note is { Id: < 1 })
        {
            var maxIdNote = Notes.MaxBy(x => x.Id);
            note.Id = (maxIdNote?.Id ?? 0) + 1;
        }

        var now = DateTime.UtcNow;
        note.CreatedAt = now;
        note.UpdatedAt = now;

        Notes.Add(note);
        return note;
    }

    public NoteModel UpdateNote(NoteModel noteModel)
    {
        var note = Notes.First(x => x.Id == noteModel.Id);
        note.Title = noteModel.Title;
        note.Content = noteModel.Content;
        note.UpdatedAt = DateTime.UtcNow;
        return note;
    }

    public bool RemoveNote(long id)
    {
        var noteById = Notes.FirstOrDefault(x => x.Id == id);

        if (noteById is null)
        {
            return false;
        }

        Notes.Remove(noteById);
        return true;
    }
}

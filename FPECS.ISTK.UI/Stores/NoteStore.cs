using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Stores;
internal class NoteStore
{
    public NoteStore()
    {
        Notes = new ObservableCollection<NoteModel>();
        FilteredNotes = CollectionViewSource.GetDefaultView(Notes);
    }
    public ObservableCollection<NoteModel> Notes { get; set; }
    public ICollectionView FilteredNotes { get; }

    public async Task LoadNotesAsync(CancellationToken cancellationToken = default)
    {
        if (Notes is { Count: > 0 })
        {
            return;
        }

        await Task.Delay(200, cancellationToken);

        var now = DateTime.UtcNow;
        var notes = new List<NoteModel>()
            {
                new NoteModel { Id=1, Title = "Shopping", Content = "Buy groceries", CreatedAt = now.AddDays(-1) },
                new NoteModel { Id=2, Title = "Work", Content = "Finish project report", CreatedAt = now },
                new NoteModel { Id=3, Title = "Meeting", Content = "Team sync-up", CreatedAt = now.AddDays(1) }
            };

        Notes.Clear();
        foreach (var item in notes)
        {
            Notes.Add(item);
        }
    }

    public NoteModel AddNote(NoteModel note)
    {
        var maxIdNote = Notes.MaxBy(x => x.Id);
        if (maxIdNote != null)
        {
            note.Id = maxIdNote.Id + 1;
        }
        note.CreatedAt = DateTime.UtcNow;
        
        Notes.Add(note);
        return note;
    }
    public bool RemoveNote(long id)
    {
        var noteById = Notes.FirstOrDefault(x => x.Id == id);

        if (noteById is null) {
            return false;
        }

        Notes.Remove(noteById);
        return true;
    }
}

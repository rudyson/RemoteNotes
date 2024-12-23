using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Stores;
internal class NoteStore
{
    public NoteStore(List<NoteModel> notes)
    {
        Notes = new ObservableCollection<NoteModel>(notes);
        FilteredNotes = CollectionViewSource.GetDefaultView(Notes);
    }

    public ObservableCollection<NoteModel> Notes { get; set; }
    public ICollectionView FilteredNotes { get; }

    public NoteModel AddNote(NoteModel note)
    {
        var maxIdNote = Notes.MaxBy(x => x.Id);
        if (maxIdNote != null)
        {
            note.Id = maxIdNote.Id;
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

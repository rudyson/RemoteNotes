using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FPECS.ISTK.UI.Stores;
internal interface INoteStore
{
    ICollectionView FilteredNotes { get; }
    ObservableCollection<NoteModel> Notes { get; set; }

    NoteModel AddNote(NoteModel note);
    Task LoadNotesAsync(CancellationToken cancellationToken = default);
    bool RemoveNote(long id);
    NoteModel UpdateNote(NoteModel noteModel);
}
using FPECS.ISTK.UI.Clients;
using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FPECS.ISTK.UI.Stores;
internal interface INotesStore
{
    ICollectionView FilteredNotes { get; }
    ObservableCollection<NoteModel> Notes { get; set; }

    NoteModel AddNote(NoteModel note);
    void LoadNotes(List<NoteModel> notes);
    Task LoadNotesAsync(IApiClient client, CancellationToken cancellationToken = default);
    bool RemoveNote(long id);
    NoteModel UpdateNote(NoteModel noteModel);
}
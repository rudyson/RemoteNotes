using FPECS.ISTK.UI.Models;
using FPECS.ISTK.UI.Stores;
using FPECS.ISTK.UI.ViewModels;
using System.Windows;

namespace FPECS.ISTK.UI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly NoteStore _noteStore;

    public App()
    { 
        var now = DateTime.UtcNow;
        var notes = new List<NoteModel>()
        {
            new NoteModel { Id=1, Title = "Shopping", Content = "Buy groceries", CreatedAt = now.AddDays(-1) },
            new NoteModel { Id=2, Title = "Work", Content = "Finish project report", CreatedAt = now },
            new NoteModel { Id=3, Title = "Meeting", Content = "Team sync-up", CreatedAt = now.AddDays(1) }
        };
        _noteStore = new NoteStore(notes);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var defaultViewModel = new NotesViewModel(_noteStore);

        var mainWindow = new MainWindow()
        {
            DataContext = defaultViewModel
        };
        mainWindow.Show();

        base.OnStartup(e);
    }
}


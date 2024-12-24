using FPECS.ISTK.UI.Stores;
using FPECS.ISTK.UI.ViewModels;
using System.Windows;

namespace FPECS.ISTK.UI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly NoteStore _noteStore;
    public MainWindow()
    {
        _noteStore = new NoteStore();
        DataContext = new NotesViewModel(_noteStore);
        InitializeComponent();
    }
}
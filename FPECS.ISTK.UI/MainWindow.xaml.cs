using FPECS.ISTK.UI.Stores;
using FPECS.ISTK.UI.ViewModels;
using System.Windows;

namespace FPECS.ISTK.UI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainViewModel();
        InitializeComponent();
    }
}
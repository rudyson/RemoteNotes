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
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow();
        MainWindow.Show();

        base.OnStartup(e);
    }
}


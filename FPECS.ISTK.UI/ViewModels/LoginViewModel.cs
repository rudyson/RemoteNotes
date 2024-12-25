using FPECS.ISTK.UI.Commands;
using FPECS.ISTK.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPECS.ISTK.UI.ViewModels;
internal class LoginViewModel : BaseViewModel
{
    private readonly NoteStore _noteStore;
    public RelayCommand UpdateViewCommand { get; set; }
    public RelayCommand LoginButtonCommand { get; set; }
    public RelayCommand RegisterButtonCommand { get; set; }
    public string ValidationMessage { get; set; }
    public LoginViewModel(NoteStore noteStore, RelayCommand updateViewCommand)
    {
        _noteStore = noteStore;
        UpdateViewCommand = updateViewCommand;
    }
}

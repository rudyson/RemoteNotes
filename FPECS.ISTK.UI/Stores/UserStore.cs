using FPECS.ISTK.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Stores;
internal class UserStore
{
    private UserModel? _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    public UserModel? CurrentUser
    {
        get => _currentUser;
        set {
            _currentUser = value;
            
        }
    }
    public UserStore()
    {

    }

    // GetUser
    // GetAuthToken
    // GetId
    // IsLoggedIn
    // Login
    // Logout
}

namespace FPECS.ISTK.UI.Models;
internal class UserModel
{
    public required long Id { get; set; }
    public required string NickName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public required bool Sex { get; set; } // True - Male ; False - Female
}

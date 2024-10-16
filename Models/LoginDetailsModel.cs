using System.ComponentModel.DataAnnotations;

namespace VaxTrack_v1.Models;

public class LoginDetailsModel
{
    // Properties for login

    [Required(ErrorMessage = "Username is required")]
    public string Username {get; set;}="";

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password {get; set;}="";

    public bool RememberMe { get; set; }
}
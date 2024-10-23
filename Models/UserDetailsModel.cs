using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaxTrack_v1.Models;
public class UserDetailsModel
{
    // Properties for login

    [Required(ErrorMessage = "Username is required")]
    [Key]
    public string Username {get; set;} = "DefaultUsername"; // Default value

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password {get; set;}="";

    [NotMapped]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }="";

    // Properties for registration

    [Required(ErrorMessage ="Full name is required")]
    public string Name {get; set;}="";

    [Required(ErrorMessage ="Date of Birth is required")]
    [DataType(DataType.Date)]
    public DateTime Birthdate {get; set;}

    [Required(ErrorMessage ="UID is required")]
    [StringLength(12, MinimumLength =12, ErrorMessage = "UID must be 12 digits number")]
    public string Uid {get; set;}="";

    [Required(ErrorMessage ="Gender is required")]
    public string Gender {get; set;}="";

    [Required(ErrorMessage ="Phone no. is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(10, ErrorMessage = "Phone number cannot be longer than 10 digits")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
    public string Phone {get; set;}="";

    [Required(ErrorMessage = "Role is required")]
    public bool Role {get; set;} = false;

}


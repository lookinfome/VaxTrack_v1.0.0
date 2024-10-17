using System.ComponentModel.DataAnnotations;

namespace VaxTrack_v1.Models;

public class UserVaccinationDetailsModel
{
    [Required(ErrorMessage = "Username is required")]
    [Key]
    public string Username {get; set;} = ""; // Default value

    [Required(ErrorMessage = "Vaccination status is required")]
    public string VaccinationStatus {get; set;} = "Not Vaccinated"; // Default value
    
    [DataType(DataType.Date)]
    public DateTime? Dose1Date {get; set;} = DateTime.MinValue;

    [DataType(DataType.Date)]
    public DateTime? Dose2Date {get;set;} = DateTime.MinValue;
}


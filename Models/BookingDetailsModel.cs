using System.ComponentModel.DataAnnotations;

namespace VaxTrack_v1.Models;

public class BookingDetailsModel
{
    [Key]
    public string Username {get; set;}
    public string BookingId {get; set;}
    public DateTime Dose1Date {get; set;} = DateTime.MinValue;
    public DateTime Dose2Date {get; set;} = DateTime.MinValue;
    public string D1HospitalName {get; set;}
    public string D2HospitalName {get; set;}
    public int Slot1Number {get; set;}
    public int Slot2Number {get; set;}
}

public class BookingFormModel
{
    public string Username {get; set;}

    [Required(ErrorMessage = "Dose 1 date is required")]
    [DataType(DataType.Date)]
    public DateTime Dose1Date {get; set;} = DateTime.MinValue;
    
    // [Required(ErrorMessage = "Dose 2 date is required")]
    [DataType(DataType.Date)]
    public DateTime Dose2Date {get; set;} = DateTime.MinValue;

}
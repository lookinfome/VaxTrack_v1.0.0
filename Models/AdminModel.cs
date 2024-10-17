using System.ComponentModel.DataAnnotations;

namespace VaxTrack_v1.Models;

public class AdminModel
{

}

public class AdminViewUserVaccinationDetails
{
    public string? Username {get; set;}
    public string? Name {get; set;}
    public string? BookingId {get; set;}
    public DateTime? Dose1Date {get; set;}
    public string? D1HospitalName {get; set;}
    public DateTime? Dose2Date {get;set;}
    public string? D2HospitalName {get; set;}
    public string? VaccinationStatus {get; set;}
}

public class AdminViewUserWithoutBooking
{
    public string? Username {get; set;}
    public string? Name {get; set;}
}
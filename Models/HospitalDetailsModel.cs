using System.ComponentModel.DataAnnotations;

namespace VaxTrack_v1.Models;

public class HospitalDetailsModel
{
    [Key]
    public string HospitalId {get; set;}
    public string HospitalName {get; set;}
    public int SlotsAvailable {get; set;}
}

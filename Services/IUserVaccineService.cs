using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    public interface IUserVaccineService
    {
        public List<AdminViewUserVaccinationDetails> FetchUserVaccinationDetails();
        public int UpdateUserVaccinationDetails(UserVaccinationDetailsModel userVaccinationDetails);
    }

    public class UserVaccineService:IUserVaccineService
    {
        private readonly AppDbContext _vaxTrackDBContext;

        public UserVaccineService(AppDbContext vaxTrackDBContext)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
        }

        // method: fetch user vaccination details
        public List<AdminViewUserVaccinationDetails> FetchUserVaccinationDetails()
        {
            try
            {
                var _userVaccinationDetails =  from user in _vaxTrackDBContext.UserVaccinationDetails
                                    join booking in _vaxTrackDBContext.BookingDetails
                                    on user.Username equals booking.Username
                                    select new AdminViewUserVaccinationDetails
                                    {
                                        Username = user.Username,
                                        BookingId = booking.BookingId,
                                        Dose1Date = user.Dose1Date != DateTime.MinValue ? user.Dose1Date : (DateTime?)null,
                                        D1HospitalName = booking.D1HospitalName,
                                        Dose2Date = user.Dose2Date != DateTime.MinValue ? user.Dose2Date : (DateTime?)null,
                                        D2HospitalName = booking.D2HospitalName,
                                        VaccinationStatus = user.VaccinationStatus
                                    };

                return _userVaccinationDetails.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error while fetching user vaccination details: {ex.Message}");
                return new List<AdminViewUserVaccinationDetails>();
            }
        }
    
        // method: update user vaccination details record
        public int UpdateUserVaccinationDetails(UserVaccinationDetailsModel userVaccinationDetails)
        {
            try
            {
                _vaxTrackDBContext.UserVaccinationDetails.Update(userVaccinationDetails);
                int _isUserVaccinaionDetailsUpdated = _vaxTrackDBContext.SaveChanges();

                return _isUserVaccinaionDetailsUpdated;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error while updating user vaccination details: {ex.Message}");
                return 0;
            }
        }
    
    }
}
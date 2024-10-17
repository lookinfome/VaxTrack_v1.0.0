using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    // interface: user vaccine service | to serve as service and allowed as injectable
    public interface IUserVaccineService
    {
        public List<AdminViewUserVaccinationDetails> FetchUserVaccinationDetails();
        public int UpdateUserVaccinationDetails(UserVaccinationDetailsModel userVaccinationDetails);
        public int FetchTotalVaccinationCompletedCount();
        public int FetchTotalUserCount();
    }

    // class: user vaccine service | implementing service methods and handeling utility methods
    public class UserVaccineService:IUserVaccineService
    {
        // variable: sqlite DB | to access DB tables
        private readonly AppDbContext _vaxTrackDBContext;

        // contructor: user vaccine service | to initialize account service class variables
        public UserVaccineService(AppDbContext vaxTrackDBContext)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
        }

        /*
        *   service method: FetchUserVaccinationDetails()
        *   purpose: to fetch list of user details who completed vaccination
        *   return: list of user details who completed vaccination
        */

        public List<AdminViewUserVaccinationDetails> FetchUserVaccinationDetails()
        {
            try
            {
                // fetch details of user who have completed vaccination
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
    
        /*
        *   service method: UpdateUserVaccinationDetails()
        *   purpose: to update vaccination status of user in user vaccination details
        *   parameter: user vaccination details model as object
        *   return: return int, <=0 for failure, 1 for succss 
        */
        public int UpdateUserVaccinationDetails(UserVaccinationDetailsModel userVaccinationDetails)
        {
            try
            {
                // update vaccination status in user vaccination details table
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

        /*
        *   service method: FetchTotalVaccinationCompletedCount()
        *   purpose: to fetch total count of completed vaccination
        *   return: total vaccination completed count
        */

        public int FetchTotalVaccinationCompletedCount()
        {
            // fetch total count of users who completed vaccination
            int _vaccinatedCount = _vaxTrackDBContext.UserVaccinationDetails
                    .Where(record => record.VaccinationStatus == "Vaccinated")
                    .Count();
            
            return _vaccinatedCount>0?_vaccinatedCount:0;

        }

        /*
        *   service method: FetchTotalUserCount()
        *   purpose: to fetch total count of completed vaccination
        *   return: total vaccination completed count
        */
        public int FetchTotalUserCount()
        {
            // fetch total count of user booked slots
            int _totalUserCount = _vaxTrackDBContext.UserVaccinationDetails.Count();

            return _totalUserCount>0?_totalUserCount:0;
        }




    }
}
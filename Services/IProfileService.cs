using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;


namespace VaxTrack_v1.Services
{
    // interface: profile service | to serve as service and allowed as injectable
    public interface IProfileService
    {
        public UserDetailsModel GetUserDetails(string username);
        public UserVaccinationDetailsModel GetUserVaccinationDetails(string username);
    }

    // class: profile service | implementing service methods and handeling utility methods
    public class ProfileService : IProfileService
    {
        // variable: sqlite DB | to access DB tables
        private AppDbContext _vaxTrackDBContext;

        // contructor: profile service | to initialize profile service class variables
        public ProfileService(AppDbContext vaxTrackDBContext)
        {
            this._vaxTrackDBContext = vaxTrackDBContext;
        }

        /*
        *   service method: GetUserDetails()
        *   purpose: to fetch user details for provided username
        *   parameter: username as string
        *   return: return user details model as object with records
        */
        public UserDetailsModel GetUserDetails(string username)
        {
            try
            {
                // fetch user details for username
                var _userDetails = this._vaxTrackDBContext.UserDetails.FirstOrDefault(record=>record.Username == username);
                
                if(_userDetails != null)
                {
                    // if user details found
                    return _userDetails;
                }

                // else
                return new UserDetailsModel();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occured while fetching user details: {ex.Message}");
                return new UserDetailsModel();
            }
        }

        /*
        *   service method: GetUserVaccinationDetails()
        *   purpose: to fetch user vaccination details for provided username
        *   parameter: username as string
        *   return: return user vaccination details model as object with records
        */
        public UserVaccinationDetailsModel GetUserVaccinationDetails(string username)
        {

            try
            {
                var _userVaccinationDetails = this._vaxTrackDBContext.UserVaccinationDetails.FirstOrDefault(record=>record.Username == username);
                if(_userVaccinationDetails != null)
                {
                    return _userVaccinationDetails;
                }
                return new UserVaccinationDetailsModel();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occured while fetching user vaccination details: {ex.Message}");
                return new UserVaccinationDetailsModel();
            }

            
        }

    }
}
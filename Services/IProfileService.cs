using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;


namespace VaxTrack_v1.Services
{
    public interface IProfileService
    {
        public UserDetailsModel GetUserDetails(string username);
        public UserVaccinationDetailsModel GetUserVaccinationDetails(string username);
    }

    public class ProfileService : IProfileService
    {
        // variable: sqlite DB
        private AppDbContext _vaxTrackDBContext;

        // constructor
        public ProfileService(AppDbContext vaxTrackDBContext)
        {
            this._vaxTrackDBContext = vaxTrackDBContext;
        }

        // method: get user records from user details table
        public UserDetailsModel GetUserDetails(string username)
        {
            try
            {
                var _userDetails = this._vaxTrackDBContext.UserDetails.FirstOrDefault(record=>record.Username == username);
                if(_userDetails != null)
                {
                    return _userDetails;
                }
                return new UserDetailsModel();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occured while fetching user details: {ex.Message}");
                return new UserDetailsModel();
            }
        }

        // method: get user vaccination record from user vaccinaion details table
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
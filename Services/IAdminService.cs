using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    public interface IAdminService
    {
        public List<AdminViewUserVaccinationDetails> FetchAdminViewUserVaccinationDetails();
        public List<AdminViewUserVaccinationDetails> FilterAdminViewUsersVaccinationDetails(string filter);
        public bool ApproveUserVaccination(string username, string bookingId);
        public List<HospitalDetailsModel> FetchAdminViewHospitalDetails();
        public List<HospitalDetailsModel> FilterAdminViewHospitalDetails(string filter);
    }

    public class AdminService : IAdminService
    {
        private AppDbContext _vaxTrackDBContext;
        private readonly IHospitalService _hospitalService;
        private readonly IUserVaccineService _userVaccineService;
        private string _vaccinationStatus = "Vaccinated"; 

        // constructor
        public AdminService(AppDbContext vaxTrackDBContext, IHospitalService hospitalService, IUserVaccineService userVaccineService)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
            _hospitalService = hospitalService;
            _userVaccineService = userVaccineService;
        }

        // ===================================
        // user vaccination details - admin 
        // ===================================

        // method: fetch all user vaccination details
        public List<AdminViewUserVaccinationDetails> FetchAdminViewUserVaccinationDetails()
        {
            try
            {
                var _adminViewDetails = _userVaccineService.FetchUserVaccinationDetails();

                if (_adminViewDetails != null)
                {
                    return _adminViewDetails.ToList();
                }
                else
                {
                    return new List<AdminViewUserVaccinationDetails>();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while fetching admin view user vaccination details: {ex.Message}");
                return new List<AdminViewUserVaccinationDetails>();
            }
        }

        // method: filter user vaccination details
        public List<AdminViewUserVaccinationDetails> FilterAdminViewUsersVaccinationDetails(string filter)
        {
            try
            {
                var _userVaccinationDetails = FetchAdminViewUserVaccinationDetails().AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    _userVaccinationDetails = _userVaccinationDetails.Where(u => u.VaccinationStatus == filter);
                }

                return _userVaccinationDetails.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while filterning admin view user vaccination details: {ex.Message}");
                return new List<AdminViewUserVaccinationDetails>();
            }
        }

        // method: approve user vaccination
        public bool ApproveUserVaccination(string username, string bookingId)
        {
            try
            {
                // fetch user vaccination details
                var _userVaccinationDetails = _vaxTrackDBContext.UserVaccinationDetails.FirstOrDefault(record=>record.Username == username);

                if(_userVaccinationDetails?.Username != null)
                {
                    // update user vaccination status
                    _userVaccinationDetails.VaccinationStatus = _vaccinationStatus;

                    // update user vaccination record
                    int _isUserVaccinaionDetailsUpdated = _userVaccineService.UpdateUserVaccinationDetails(_userVaccinationDetails);

                    // update hospital details
                    if(_isUserVaccinaionDetailsUpdated >0 )
                    {
                        // fetch booking details
                        var _userBookingDetails = _vaxTrackDBContext.BookingDetails.FirstOrDefault(record=>record.Username == _userVaccinationDetails.Username);

                        if(_userBookingDetails?.Username != null)
                        {
                            // fetch hospital names
                            string _d1HospitalName = _userBookingDetails.D1HospitalName;
                            string _d2HospitalName = _userBookingDetails.D2HospitalName;

                            // update hospital details
                            return UpdateHospitalDetails(_d1HospitalName, _d2HospitalName);

                        }
                        else
                        {
                            return false; // booking details not found
                        }

                    }
                    else
                    {
                        return false; // user vaccination record not updated
                    }

                }
                else
                {
                    return false; // user vaccination record not found
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while approving user vaccination: {ex.Message}");
                return false;
            }
        }
        


        // ===================================
        // hospital details - admin
        // ===================================

        // method: fetch hospital details
        public List<HospitalDetailsModel> FetchAdminViewHospitalDetails()
        {
            try
            {
                var _hospitalDetails = _hospitalService.FetchHospitalDetails();

                if(_hospitalDetails != null)
                {
                    return _hospitalDetails;
                }

                return new List<HospitalDetailsModel>();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while fetching admin view hospital details: {ex.Message}");
                return new List<HospitalDetailsModel>();
            }
        }

        // method: filter hospital details by slots available
        public List<HospitalDetailsModel> FilterAdminViewHospitalDetails(string filter)
        {
            var _filteredHospitalDetails = _hospitalService.FilterHospitalDetails(filter);
            return _filteredHospitalDetails;
        }

        // method: update hospital details
        private bool UpdateHospitalDetails(string hospitalName1, string hospitalName2)
        {
            try
            {
                bool _isUserVaccinaionDetailsUpdated = _hospitalService.UpdateHospitalDetails(hospitalName1, hospitalName2);

                if(_isUserVaccinaionDetailsUpdated)
                {
                    return _isUserVaccinaionDetailsUpdated;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while updating hospital details: {ex.Message}");
                return false;
            }
        
        }

        

    }
}
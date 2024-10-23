using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    // interface: admin service | to serve as service and allowed as injectable
    public interface IAdminService
    {
        public List<AdminViewUserVaccinationDetails> FetchAdminViewUserVaccinationDetails();
        public List<AdminViewUserVaccinationDetails> FilterAdminViewUsersVaccinationDetails(string filter);
        public bool ApproveUserVaccination(string username, string bookingId);
        public int TotalVaccinationCompletedCount();
        public int TotalUserCount();
        public int UsersCountWithNoBooking();
        public List<AdminViewUserWithoutBooking> UsersDetailsWithNoBooking();
        public List<HospitalDetailsModel> FetchAdminViewHospitalDetails();
        public List<HospitalDetailsModel> FilterAdminViewHospitalDetails(string filter);
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy);
    }

    // class: admin service | implementing service methods and handeling utility methods
    public class AdminService : IAdminService
    {
        private AppDbContext _vaxTrackDBContext;
        private readonly IHospitalService _hospitalService;
        private readonly IUserVaccineService _userVaccineService;

        private readonly IBookingService _bookingService;
        private string _vaccinationStatus = "Vaccinated"; 

        // contructor: admin service | to initialize account service class variables
        public AdminService(AppDbContext vaxTrackDBContext, IHospitalService hospitalService, IUserVaccineService userVaccineService, IBookingService bookingService)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
            _hospitalService = hospitalService;
            _bookingService = bookingService;
            _userVaccineService = userVaccineService;
        }

        /*
        *   service method: FetchAdminViewUserVaccinationDetails()
        *   purpose: to fetch list of user's vaccination details with booking details
        *   return: return list of user's vaccination details with booking details
        */
        public List<AdminViewUserVaccinationDetails> FetchAdminViewUserVaccinationDetails()
        {
            try
            {
                var _adminViewDetails = _userVaccineService.FetchUserVaccinationDetails();

                if (_adminViewDetails != null)
                {

                    foreach(var rec in _adminViewDetails.ToList())
                    {
                        string _name = _vaxTrackDBContext.UserDetails.Where(record => record.Username == rec.Username).Select(record => record.Name).FirstOrDefault();

                        if(!string.IsNullOrEmpty(_name))
                        {
                            rec.Name = _name;
                        }

                    }

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

        /*
        *   service method: FilterAdminViewUsersVaccinationDetails()
        *   purpose: to filter list of user's vaccination details with booking details
        *   parameter: filter value as string
        *   return: return filtered list of user's vaccination details with booking details
        */
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

        /*
        *   service method: ApproveUserVaccination()
        *   purpose: to approve user's vaccination
        *   parameter: username and booking id as string
        *   return: return bool value, 1 for successful approval, else 0
        */
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
        
        /*
        *   service method: TotalVaccinationCompletedCount()
        *   purpose: to fetch user's count with vaccination complete
        *   return: return int value as user's count
        */
        public int TotalVaccinationCompletedCount()
        {
            int _vaccinatedCount = _userVaccineService.FetchTotalVaccinationCompletedCount();
            return _vaccinatedCount>0?_vaccinatedCount:0;
        }

        /*
        *   service method: UsersCountWithNoBooking()
        *   purpose: to fetch user's count without slot bookig
        *   return: return int value as user's count
        */
        public int UsersCountWithNoBooking()
        {
            int _usersCount = _bookingService.FetchUsersCountWithNoBooking();
            return _usersCount >0?_usersCount:0;
        }
   
        /*
        *   service method: TotalUserCount()
        *   purpose: to fetch registered user's count with booking 
        *   return: return int value as user's count
        */
        public int TotalUserCount()
        {
            int _totalUserCount = _userVaccineService.FetchTotalUserCount();
            return _totalUserCount>0?_totalUserCount:0;
        }

        /*
        *   service method: UsersDetailsWithNoBooking()
        *   purpose: to fetch list of users without slot bookig
        *   return: return list of users without slot bookig
        */
        public List<AdminViewUserWithoutBooking> UsersDetailsWithNoBooking()
        {
            List<AdminViewUserWithoutBooking> _usersWithoutSlot = _bookingService.FetchUsersDetailsWithNoBooking();
            return _usersWithoutSlot;
        }


        /*
        *   service method: FetchAdminViewHospitalDetails()
        *   purpose: to fetch list of hospital details
        *   return: return list of hospital details
        */
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

        /*
        *   service method: FilterAdminViewHospitalDetails()
        *   purpose: to filter list of hospital details
        *   parameter: filter value as string
        *   return: return filtered list of hospital details
        */
        public List<HospitalDetailsModel> FilterAdminViewHospitalDetails(string filter)
        {
            var _filteredHospitalDetails = _hospitalService.FilterHospitalDetails(filter);
            return _filteredHospitalDetails;
        }
     
        /*
        *   service method: UpdateHospitalDetails()
        *   purpose: to update available slots in hospital details table
        *   parameter: hospital names value as string
        *   return: return bool value, 1 if updated successfully, else 0
        */
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

        /*
        *   service method: UpdateAvailableSlotsById()
        *   purpose to increase slots available in hospitals by admin
        *   parameter: hospital id as string and increase by value as int
        *   return: return void
        */
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy)
        {
            HospitalDetailsModel _hospitalDetails = _hospitalService.FetchHospitalDetailsById(hospitalId);

            if(_hospitalDetails != null)
            {
                _hospitalDetails.SlotsAvailable = _hospitalDetails.SlotsAvailable+increaseBy;

                _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails);
                _vaxTrackDBContext.SaveChanges();
            }
            else
            {
                Console.WriteLine($"unexpected error occurred while updating slots");
            }
        }

    }
}
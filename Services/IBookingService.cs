using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace VaxTrack_v1.Services
{
    // interface: booking service | to serve as service and allowed as injectable
    public interface IBookingService
    {
        public BookingDetailsModel? FetchBookingDetails(string username);
        public string CreateNewBookingId(string username);
        public bool IsSlotsAvailable();
        public bool SaveBookingDetails(BookingFormModel submittedDetails, string newBookingId);
        public List<BookingDetailsModel> FetchBookingDetails();
        public int FetchUsersCountWithNoBooking();
        public List<AdminViewUserWithoutBooking> FetchUsersDetailsWithNoBooking();
    }

    // class: booking service | implementing service methods and handeling utility methods
    public class BookingService : IBookingService
    {
        // variable: sqlite DB | to access DB tables
        private readonly AppDbContext _vaxTrackDBContext;

        // contructor: booking service | to initialize account service class variables
        public BookingService(AppDbContext vaxTrackDBContext)
        {
            this._vaxTrackDBContext = vaxTrackDBContext;
        }


        /*
        *   service method: FetchBookingDetails()
        *   purpose: to fetch list of user's booking details
        *   return: return list of user's booking details
        */
        public List<BookingDetailsModel> FetchBookingDetails()
        {
            try
            {
                // fetch user's booking details
                var _userBookingDetails = _vaxTrackDBContext.BookingDetails.ToList();
                if(_userBookingDetails != null)
                {
                    return _userBookingDetails;
                }

                return new List<BookingDetailsModel>();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while fetching booking details: {ex.Message}");
                return new List<BookingDetailsModel>();
            }
        }

        /*
        *   service method: FetchBookingDetails()
        *   purpose: to fetch user's booking details for particular user
        *   parameter: username as string
        *   return: return user's booking details for particular user
        */
        public BookingDetailsModel FetchBookingDetails(string username)
        {
            try
            {
                // fetch user's booking details
                var _userBookingDetails = _vaxTrackDBContext.BookingDetails.FirstOrDefault(record=>record.Username == username);
                if(_userBookingDetails != null)
                {
                    return _userBookingDetails;
                }

                return new BookingDetailsModel();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while fetching booking details by username: {ex.Message}");
                return new BookingDetailsModel();
            }
        }

        /*
        *   service method: CreateNewBookingId()
        *   purpose: to create new booking id for username
        *   parameter: username as string
        *   return: return booking id as string
        */
        public string CreateNewBookingId(string username)
        {
            Random _randomNumGenerator = new Random();
            string _randomNum = _randomNumGenerator.Next(10,999).ToString();

            return $"{username}_{_randomNum}";
        } 

        /*
        *   service method: IsSlotsAvailable()
        *   purpose: to check if slots available in listed hospital from hospital details table
        *   return: return bool value, 1 if slots available, else 0
        */
        public bool IsSlotsAvailable()
        {
            try
            {
                // fetch two hospitals with 1 slot available
                var _hospitalDetails2Slots_1 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable > 0).Take(2).ToList();

                // fetch one hospital with 2 slots available
                var _hospitalDetails2Slots_2 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable >= 2).FirstOrDefault();

                if(_hospitalDetails2Slots_1.Count >= 2 || _hospitalDetails2Slots_2 != null)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while chceking slots availability: {ex.Message}");
                return false;
            }
        }

        /*
        *   service method: SaveBookingDetails()
        *   purpose: to fetch save slot booking details (dose 1 date, dose 2 date) in booking details table
        *   parameter: booking details model as object and booking id as string
        *   return: return bool value, 1 for successfully saving the details, else 0
        */
        public bool SaveBookingDetails(BookingFormModel submittedDetails, string newBookingId)
        {
            try
            {
                // fetch two hospitals having 1 slots available
                var _hospitalDetails2Slots_1 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable > 0).Take(2).ToList();

                // fetch one hospital having 2 slots available
                var _hospitalDetails2Slots_2 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable >= 2).FirstOrDefault();

                // fetch user vaccination details for username
                var _userVccinationDetails = _vaxTrackDBContext.UserVaccinationDetails.FirstOrDefault(record=>record.Username == submittedDetails.Username);

                // check if one hospital with 2 slots available
                if(_hospitalDetails2Slots_2 != null)
                {
                    // create new booking details
                    BookingDetailsModel _bookingDetails = new BookingDetailsModel {
                        Username = submittedDetails.Username,
                        BookingId = newBookingId,
                        Dose1Date = submittedDetails.Dose1Date,
                        D1HospitalName = _hospitalDetails2Slots_2.HospitalName,
                        Slot1Number = _hospitalDetails2Slots_2.SlotsAvailable,
                        Dose2Date = submittedDetails.Dose2Date,
                        D2HospitalName = _hospitalDetails2Slots_2.HospitalName,
                        Slot2Number = _hospitalDetails2Slots_2.SlotsAvailable-1
                        
                    };

                    // save new booking details
                    _vaxTrackDBContext.BookingDetails.Add(_bookingDetails);
                    int isNewBookingSuccess = _vaxTrackDBContext.SaveChanges();

                    if(isNewBookingSuccess > 0)
                    {
                        // update hospital details
                        _hospitalDetails2Slots_2.SlotsAvailable = _hospitalDetails2Slots_2.SlotsAvailable-2;
                        _vaxTrackDBContext.SaveChanges();

                        // update user vaccination details
                        if(_userVccinationDetails != null)
                        {
                            _userVccinationDetails.VaccinationStatus = _userVccinationDetails.VaccinationStatus;
                            _userVccinationDetails.Dose1Date = submittedDetails.Dose1Date;
                            _userVccinationDetails.Dose2Date = submittedDetails.Dose2Date;

                            // Save changes to the database
                            _vaxTrackDBContext.SaveChanges();
                        }
                        
                    }

                    return true;

                }

                // else two hospitals with 1 slot
                else if(_hospitalDetails2Slots_1 != null)
                {
                    // create new booking details
                    BookingDetailsModel _bookingDetails = new BookingDetailsModel {
                        Username = submittedDetails.Username,
                        BookingId = newBookingId,
                        Dose1Date = submittedDetails.Dose1Date,
                        D1HospitalName = _hospitalDetails2Slots_1[0].HospitalName,
                        Slot1Number = _hospitalDetails2Slots_1[0].SlotsAvailable,
                        Dose2Date = submittedDetails.Dose2Date,
                        D2HospitalName = _hospitalDetails2Slots_1[1].HospitalName,
                        Slot2Number = _hospitalDetails2Slots_1[1].SlotsAvailable
                        
                    };

                    // save new booking details
                    _vaxTrackDBContext.BookingDetails.Add(_bookingDetails);
                    int isNewBookingSuccess = _vaxTrackDBContext.SaveChanges();

                    if(isNewBookingSuccess > 0)
                    {
                        // update hospital details
                        _hospitalDetails2Slots_1[0].SlotsAvailable = _hospitalDetails2Slots_1[0].SlotsAvailable-1;
                        _hospitalDetails2Slots_1[1].SlotsAvailable = _hospitalDetails2Slots_1[1].SlotsAvailable-1;
                        _vaxTrackDBContext.SaveChanges();

                        // update user vaccination details
                        if(_userVccinationDetails != null)
                        {
                            _userVccinationDetails.VaccinationStatus = _userVccinationDetails.VaccinationStatus;
                            _userVccinationDetails.Dose1Date = submittedDetails.Dose1Date;
                            _userVccinationDetails.Dose2Date = submittedDetails.Dose2Date;

                            // Save changes to the database
                            _vaxTrackDBContext.SaveChanges();
                        }
                        
                    }

                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error while saving booking details: {ex.Message}");
                return false;
            }
        }


        /*
        *   service method: FetchUsersCountWithNoBooking()
        *   purpose: to fetch total count of registered users without slot booking 
        *   return: total count of registered users without slot booking
        */
        public int FetchUsersCountWithNoBooking()
        {
            // fetch total count of users without slot booking
            int _usersCount = _vaxTrackDBContext.UserVaccinationDetails
                                .GroupJoin(
                                    _vaxTrackDBContext.BookingDetails,
                                    user => user.Username,
                                    booking => booking.Username,
                                    (user, bookings) => new { user, bookings }
                                )
                                .Where(result => !result.bookings.Any())
                                .Count();


            return _usersCount >0?_usersCount:0;
        }

        /*
        *   service method: FetchUsersDetailsWithNoBooking()
        *   purpose: to fetch list of registered users without slot booking 
        *   return: list of registered users without slot booking
        */
        public List<AdminViewUserWithoutBooking> FetchUsersDetailsWithNoBooking()
        {
            // fetch registered user details without any slots booked
            var _usersWithoutSlot = from user in _vaxTrackDBContext.UserVaccinationDetails
                                    join details in _vaxTrackDBContext.UserDetails on user.Username equals details.Username
                                    join booking in _vaxTrackDBContext.BookingDetails on user.Username equals booking.Username into bookings
                                    from booking in bookings.DefaultIfEmpty()
                                    where booking == null
                                    select new AdminViewUserWithoutBooking
                                    {
                                        Username = user.Username,
                                        Name = details.Name
                                    };

            return _usersWithoutSlot.ToList();
        }


    } 
}

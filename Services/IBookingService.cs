using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace VaxTrack_v1.Services
{
    public interface IBookingService
    {
        public BookingDetailsModel? FetchBookingDetails(string username);
        public string CreateNewBookingId(string username);
        public bool IsSlotsAvailable();
        public bool SaveBookingDetails(BookingFormModel submittedDetails, string newBookingId);
        public List<BookingDetailsModel> FetchBookingDetails();
    }

    public class BookingService : IBookingService
    {
        public AppDbContext _vaxTrackDBContext;

        public BookingService(AppDbContext vaxTrackDBContext)
        {
            this._vaxTrackDBContext = vaxTrackDBContext;
        }

        // method: fetch booking details as list
        public List<BookingDetailsModel> FetchBookingDetails()
        {
            try
            {
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

        // method: fetch booking details based on username
        public BookingDetailsModel FetchBookingDetails(string username)
        {
            try
            {
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

        // method: create new booking Id
        public string CreateNewBookingId(string username)
        {
            Random _randomNumGenerator = new Random();
            string _randomNum = _randomNumGenerator.Next(10,999).ToString();

            return $"{username}_{_randomNum}";
        } 

        // method: check slots availability
        public bool IsSlotsAvailable()
        {
            try
            {
                var _hospitalDetails2Slots_1 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable > 0).Take(2).ToList();

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

        // method: save new booking details
        public bool SaveBookingDetails(BookingFormModel submittedDetails, string newBookingId)
        {
            try
            {
                var _hospitalDetails2Slots_1 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable > 0).Take(2).ToList();

                var _hospitalDetails2Slots_2 = _vaxTrackDBContext.HospitalDetails.Where(record => record.SlotsAvailable >= 2).FirstOrDefault();

                var _userVccinationDetails = _vaxTrackDBContext.UserVaccinationDetails.FirstOrDefault(record=>record.Username == submittedDetails.Username);

                if(_hospitalDetails2Slots_2 != null)
                {
                    // new booking details
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
                else if(_hospitalDetails2Slots_1 != null)
                {
                    // new booking details
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

    } 
}

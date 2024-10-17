using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Authorization;


namespace VaxTrack_v1.Controllers;

// class: booking contoller | handle logic and slot booking requests

public class BookingController:Controller
{
    // vriable: profile service | for accessing profile service methods
    private readonly IProfileService _profileService;

    // vriable: booking service | for accessing booking service methods
    private readonly IBookingService _bookingService;

    // constructor: booking controller | to initialize controller class variables
    public BookingController(IProfileService profileService, IBookingService bookingService)
    {
        _profileService = profileService;
        _bookingService = bookingService;
    }


    /*
    *   action method: SlotBook()
    *   http request: GET
    *   purpose: to get slot booking form for user
    *   return: slot booking form view
    *   authorization: required
    */
    [Authorize]
    [HttpGet("Account/UserProfile/{username}/SlotBook")]
    public IActionResult SlotBook(string username)
    {
        try 
        {
            // fetch booking details for user
            var _userBookingDetails = _bookingService.FetchBookingDetails(username);

            // fetch booking id for user
            if(_userBookingDetails?.Username != null)
            {

                // return message page
                ViewBag.SlotBookingMsg = $"{username} - already booked both slots, booking Id: {_userBookingDetails.BookingId}";
                return View("~/Views/Booking/SlotBookError.cshtml");
            }

            // if booking id not exists
            else
            {
                // fetch username
                @ViewBag.Username = username;

                // return new slot booking form
                return View();
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while getting slot book page: {ex.Message}");
            
            // fetch username
            @ViewBag.Username = username;
            return View();
        }
    }

    /*
    *   action method: SlotBook()
    *   http request: POST
    *   purpose: to submit slot booking form for user
    *   return: 
    *       if success, submit slot booking form, redirect to user profile page
    *       if failed, get back to slot booking form
    *   authorization: required
    */
    [Authorize]
    [HttpPost("Account/UserProfile/{username}/SlotBook")]
    public IActionResult SlotBook(BookingFormModel submittedDetails)
    {
        try
        {
            // check slots availability
            bool _isSlotsAvailable = _bookingService.IsSlotsAvailable();

            // if slots available
            if(_isSlotsAvailable)
            {
                if(ModelState.IsValid)
                {
                    // fetch booking Id
                    var _userBookingDetails = _bookingService.FetchBookingDetails(submittedDetails.Username);
                        
                    // if no record found
                    if(_userBookingDetails?.Username == null)
                    {
                        // create new booking id
                        string _newBookingId = _bookingService.CreateNewBookingId(submittedDetails.Username);

                        // save new booking details
                        bool isSlotBookingSuccess = _bookingService.SaveBookingDetails(submittedDetails, _newBookingId);
                        
                        if(isSlotBookingSuccess)
                        {
                            // return View - user profile page
                            return RedirectToAction("UserProfile", "Profile", new {username = submittedDetails.Username});

                        }
                        else
                        {
                            // return View - booking page
                            return SlotBook(submittedDetails.Username);
                        }
                        
                    }

                    // if record found
                    else
                    {
                        // return - message page
                        return Ok($"{_userBookingDetails.Username} - already booked both slots, booking Id: {_userBookingDetails.BookingId}");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid booking attempt");
                    
                    // return View - booking page
                    return SlotBook(submittedDetails.Username);
                }

            }

            // if slot not available
            else
            {
                // return - message page
                @ViewBag.SlotBookingMsg = $"No slots are available right now";
                return View("~/Views/Booking/SlotBookError.cshtml");
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while booking slot: {ex.Message}");
            @ViewBag.SlotBookingMsg = $"Unexpected error occurred while booking slot: {ex.Message}";
            return View("~/Views/Booking/SlotBookError.cshtml");
        }
    
    }

}
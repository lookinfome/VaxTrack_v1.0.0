using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Metadata.Ecma335;


namespace VaxTrack_v1.Controllers;

public class BookingController:Controller
{
    // Variables: profile and booking services
    private readonly IProfileService _profileService;
    private readonly IBookingService _bookingService;

    // Constructor: copy of in-house DB
    public BookingController(IProfileService profileService, IBookingService bookingService)
    {
        _profileService = profileService;
        _bookingService = bookingService;
    }


    // get booking page

    // action method: get slot booking form
    [HttpGet("Account/UserProfile/{username}/SlotBook")]
    public IActionResult SlotBook(string username)
    {
        try 
        {
            // user vaccination details

            // fetch booking Id
            var _userBookingDetails = _bookingService.FetchBookingDetails(username);

            // if booking Id exists
            if(_userBookingDetails?.Username != null)
            {

                // return View - message page
                ViewBag.SlotBookingMsg = $"{username} - already booked both slots, booking Id: {_userBookingDetails.BookingId}";
                return View("~/Views/Booking/SlotBookError.cshtml");
            }

            // if booking Id not exists
            else
            {
                // fetch username
                @ViewBag.Username = username;

                // return View - new slot booking form
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

    // action method: submit slot booking form
    [HttpPost("Account/UserProfile/{username}/SlotBook")]
    public IActionResult SlotBook(BookingFormModel submittedDetails)
    {
        try
        {
            // check slots availability
            bool _isSlotsAvailable = _bookingService.IsSlotsAvailable();

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

                        Console.WriteLine($"--------------{_newBookingId}---------------");

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
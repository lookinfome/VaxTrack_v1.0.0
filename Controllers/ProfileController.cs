using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VaxTrack_v1.Controllers;

// class: profile contoller | handle logic and profile requests
public class ProfileController:Controller
{
    // variable: user manager | for authentication
    private readonly UserManager<AppUserModel> _userManager;

    // vriable: profile service | for accessing profile service methods
    private readonly IProfileService _profileService;

    // vriable: booking service | for booking booking service methods
    private readonly IBookingService _bookingService;

    // constructor: profile controller | to initialize controller class variables
    public ProfileController(IProfileService profileService, IBookingService bookingService, UserManager<AppUserModel> userManager)
    {
        _profileService = profileService;
        _bookingService = bookingService;
        _userManager = userManager;
    }

    /*
    *   action method: UserProfile()
    *   http request: GET
    *   purpose: to get user profile page
    *   return: user profile view
    *   authorization: required
    */

    [Authorize]
    [HttpGet("Account/UserProfile/{username}")]
    public async Task<IActionResult> UserProfile(string username)
    {
        try
        {
            // fetch user details from asp-net-user table for authentication
            var loggedInUser = await _userManager.GetUserAsync(User);

            // if user's recor not found
            if (loggedInUser == null || loggedInUser.UserName != username)
            {
                return RedirectToAction("Login", "Account");
            }

            // fetch user's details for username
            var _userDetails = _profileService.GetUserDetails(username);
            
            // fetch user's vaccination details for username
            var _userVaccinationDetails = _profileService.GetUserVaccinationDetails(username);
            
            // fetch booking details for username
            var _userBookingDetails = _bookingService.FetchBookingDetails(username);

            // login message on success
            ViewBag.LoginMessage = TempData["LoginMessage"];

            // registration message on success
            ViewBag.RegistrationMessage = TempData["RegistrationMessage"];

            // if user's and user's vaccination details exists
            if(_userDetails != null)
            {
                if(_userVaccinationDetails != null)
                {
                    // fetch vaccination status, dose 1 and dose 2 details
                    ViewData["VaccinationStatus"] = _userVaccinationDetails.VaccinationStatus;
                    ViewData["Dose1Date"] = _userVaccinationDetails.Dose1Date;
                    ViewData["Dose2Date"] = _userVaccinationDetails.Dose2Date;

                    ViewBag.SlotBookButtonEnable = _userBookingDetails?.BookingId == null ? true : false;

                    return View(_userDetails);
                }
                else
                {
                    Console.WriteLine($"User vaccination details not found for user - {username}");
                    return View(_userDetails);
                }
            }
            else
            {
                Console.WriteLine($"User details not found for user - {username}");
                return RedirectToAction("Login", "Action");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while getting user profile page: {ex.Message}");
            Console.WriteLine($"User details not found for user - {username}");
            return RedirectToAction("Login", "Action");
        }
    }


}
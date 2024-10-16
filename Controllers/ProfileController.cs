using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace VaxTrack_v1.Controllers;

public class ProfileController:Controller
{
    // variable: profile and booking service
    private readonly IProfileService _profileService;
    private readonly IBookingService _bookingService;
    private readonly UserManager<AppUserModel> _userManager;

    // constructor: copy of in-house DB
    public ProfileController(IProfileService profileService, IBookingService bookingService, UserManager<AppUserModel> userManager)
    {
        _profileService = profileService;
        _bookingService = bookingService;
        _userManager = userManager;
    }

    // user profile page

    // action method: get user profile page
    [Authorize]
    [HttpGet("Account/UserProfile/{username}")]
    public async Task<IActionResult> UserProfile(string username)
    {
        try
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            if (loggedInUser == null || loggedInUser.UserName != username)
            {
                return RedirectToAction("Login", "Account");
            }

            var _userDetails = _profileService.GetUserDetails(username);
            var _userVaccinationDetails = _profileService.GetUserVaccinationDetails(username);
            var _userBookingDetails = _bookingService.FetchBookingDetails(username);

            if(_userDetails != null)
            {
                if(_userVaccinationDetails != null)
                {
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
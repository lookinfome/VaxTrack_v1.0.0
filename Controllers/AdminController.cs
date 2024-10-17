using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VaxTrack_v1.Controllers;

// class: admin contoller | handle admin page requests

[Authorize]
public class AdminController:Controller
{
    
    // variable: user manager | for authentication
    private readonly UserManager<AppUserModel> _userManager;

    // vriable: admin service | for accessing admin service methods
    private readonly IAdminService _adminService;

    // constructor: admin controller | to initialize controller class variables
    public AdminController(IAdminService adminService, UserManager<AppUserModel> userManager)
    {
        _adminService = adminService;
        _userManager = userManager;
    }

    /*
    *   action method: AdminPage()
    *   http request: GET
    *   purpose: to get admin page
    *   return: admin page view
    *   authorization: required
    */
    [HttpGet("/Admin/{username}")]
    public async Task<IActionResult> AdminPage()
    {
        // fetch user details from asp-net-user table
        var loggedInUser = await _userManager.GetUserAsync(User);

        // check user autheticity and role
        if (loggedInUser == null || !await _userManager.IsInRoleAsync(loggedInUser, "Admin"))
        {
            Console.WriteLine($"{loggedInUser?.UserName} is not Admin Role");
            return RedirectToAction("Login", "Account");
        }

        // default user vaccination details
        List<AdminViewUserVaccinationDetails> _adminViewDetails = _adminService.FetchAdminViewUserVaccinationDetails();
        ViewBag.AdminViewDetails = _adminViewDetails;

        // default hospital details
        List<HospitalDetailsModel> _adminHospitalDetails = _adminService.FetchAdminViewHospitalDetails();
        ViewBag.HospitalDetails = _adminHospitalDetails;

        // total vaccination completed count
        int _totalVaccinationCount = _adminService.TotalVaccinationCompletedCount();
        ViewBag.TotalVaccinationCount = _totalVaccinationCount;

        // total registered users wit no slot booked
        int _usersCountWithNoSlot = _adminService.UsersCountWithNoBooking();
        ViewBag.UsersCountWithNoBooking = _usersCountWithNoSlot;
        
        // total registered users
        int _totalUserCount = _adminService.TotalUserCount();
        ViewBag.TotalUsersCount = _totalUserCount;

        // total registered users with pending approval
        int _userCountWithPendingApproval = _totalUserCount - (_totalVaccinationCount+_usersCountWithNoSlot);
        ViewBag.UserCountWithPendingApproval = _userCountWithPendingApproval;

        // users with no booked slot details
        List<AdminViewUserWithoutBooking> _userListWothoutSlot  = _adminService.UsersDetailsWithNoBooking();
        ViewBag.UserListWithoutSlot = _userListWothoutSlot;

        // return view
        return View();
    }

    /*
    *   action method: ApproveSlotBook()
    *   http request: POST
    *   purpose: to approve pending vaccination of user
    *   return:
    *       if success, change vaccination status of user, update available slots in hospital
    *       if failed, redirect to error message page
    *   authorization: required
    */

    [HttpPost("/Admin/BookUserSlot")]
    public ActionResult ApproveSlotBook(string username, string bookingId)
    {
        try
        {
            // fetch approval status post approving the vaccination pending requests
            bool _approveSlotBooked = _adminService.ApproveUserVaccination(username, bookingId);

            // if approved
            if(_approveSlotBooked)
            {
                // fetch updated user's details post approval
                List<AdminViewUserVaccinationDetails> _adminViewDetails = _adminService.FetchAdminViewUserVaccinationDetails();
                return PartialView("_FilteredUsersTablePartial", _adminViewDetails.ToList());
            }

            //else
            else
            {
                // return error message
                return NotFound($"Approval failed due to unexpected error for user - {username}");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error while approving slot booking: {ex.Message}");
            return NotFound($"Approval failed due to unexpected error for user - {username}"); 
        }
    }

    /*
    *   action method: FilterUsers()
    *   http request: GET
    *   purpose: to filter table of user's with pending approvals based on vaccination status
    *   return: filtered list of user's with pending approvals based on vaccination status
    *   authorization: required
    */
    [HttpGet("/Admin/FilterUser")]
    public ActionResult FilterUsers(string filter)
    {
        try
        {
            // fetch filtered list of user's with vaccination status
            List<AdminViewUserVaccinationDetails> _adminViewUsersDetailsFiltered = _adminService.FilterAdminViewUsersVaccinationDetails(filter);

            if(_adminViewUsersDetailsFiltered.Count > 0)
            {
                // return partial view with filtered list
                return PartialView("_FilteredUsersTablePartial", _adminViewUsersDetailsFiltered.ToList());
            }

            // return error page
            return PartialView("_AdminErrorMsgPartial", _adminViewUsersDetailsFiltered.ToList());
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while displaying filtered users detials: {ex.Message}");
            return PartialView("_AdminErrorMsgPartial", new List<AdminViewUserVaccinationDetails>());
        }
    }

    /*
    *   action method: FilterHospitals()
    *   http request: GET
    *   purpose: to filter hospital table with available slots
    *   return: filtered list of hospital table with available slots
    *   authorization: required
    */
    [HttpGet("Admin/FilterHospital")]
    public ActionResult FilterHospitals(string filter)
    {
        try
        {
            // fetch filtered list of hospitals with available slots
            List<HospitalDetailsModel> _adminViewFilteredHospitalDtails = _adminService.FilterAdminViewHospitalDetails(filter);

            if (_adminViewFilteredHospitalDtails.Count > 0)
            {
                // return the partial view with filtered list
                return PartialView("_FilteredHospitalTablePartial", _adminViewFilteredHospitalDtails.ToList());
            }

            // return error page
            return PartialView("_AdminErrorMsgPartial", _adminViewFilteredHospitalDtails.ToList());
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while displaying filtered hospital results: {ex.Message}");
            return PartialView("_AdminErrorMsgPartial", new List<HospitalDetailsModel>());
        }
    }

}
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace VaxTrack_v1.Controllers;

[Authorize]
public class AdminController:Controller
{
    private readonly IAdminService _adminService;
    private readonly UserManager<AppUserModel> _userManager;

    public AdminController(IAdminService adminService, UserManager<AppUserModel> userManager)
    {
        _adminService = adminService;
        _userManager = userManager;
    }

    // method: get admin page
    [HttpGet("/Admin/{username}")]
    public async Task<IActionResult> AdminPage()
    {
        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null || !await _userManager.IsInRoleAsync(loggedInUser, "Admin"))
        {
            Console.WriteLine($"{loggedInUser?.UserName} is not Role");
            return RedirectToAction("Login", "Account");
        }

        // default user vaccination details
        List<AdminViewUserVaccinationDetails> _adminViewDetails = _adminService.FetchAdminViewUserVaccinationDetails();
        ViewBag.AdminViewDetails = _adminViewDetails;

        // default hospital details
        List<HospitalDetailsModel> _adminHospitalDetails = _adminService.FetchAdminViewHospitalDetails();
        ViewBag.HospitalDetails = _adminHospitalDetails;

        // total vaccination completed count
        int _totalVaccinationCount = _adminService.FetchTotalVaccinationCompletedCount();
        ViewBag.TotalVaccinationCount = _totalVaccinationCount;

        // total registered users wit no slot booked
        int _usersCountWithNoSlot = _adminService.FetchUsersWithNoBooking();
        ViewBag.UsersCountWithNoBooking = _usersCountWithNoSlot;
        
        // total registered users
        int _totalUserCount = _adminService.FetchTotalUserCount();
        ViewBag.TotalUsersCount = _totalUserCount;

        // total registered users with pending approval
        int _userCountWithPendingApproval = _totalUserCount - (_totalVaccinationCount+_usersCountWithNoSlot);
        ViewBag.UserCountWithPendingApproval = _userCountWithPendingApproval;

        //
        List<AdminViewUserWithoutBooking> _userListWothoutSlot  = _adminService.FetchUsersWithoutBooking();
        ViewBag.UserListWithoutSlot = _userListWothoutSlot;

        // return view
        return View();
    }

    [HttpPost("/Admin/BookUserSlot")]
    public ActionResult ApproveSlotBook(string username, string bookingId)
    {
        try
        {
            bool _approveSlotBooked = _adminService.ApproveUserVaccination(username, bookingId);
            if(_approveSlotBooked)
            {
                // fetch updated details post approval
                List<AdminViewUserVaccinationDetails> _adminViewDetails = _adminService.FetchAdminViewUserVaccinationDetails();
                return PartialView("_FilteredUsersTablePartial", _adminViewDetails.ToList());
            }
            else
            {
                return NotFound($"Approval failed due to unexpected error for user - {username}");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error while approving slot booking: {ex.Message}");
            return NotFound($"Approval failed due to unexpected error for user - {username}"); 
        }
    }

    [HttpGet("/Admin/FilterUser")]
    public ActionResult FilterUsers(string filter)
    {
        try
        {
            // fetch filtered results
            List<AdminViewUserVaccinationDetails> _adminViewUsersDetailsFiltered = _adminService.FilterAdminViewUsersVaccinationDetails(filter);

            if(_adminViewUsersDetailsFiltered.Count > 0)
            {
                return PartialView("_FilteredUsersTablePartial", _adminViewUsersDetailsFiltered.ToList());
            }

            return PartialView("_AdminErrorMsgPartial", _adminViewUsersDetailsFiltered.ToList());
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while displaying filtered users detials: {ex.Message}");
            return PartialView("_AdminErrorMsgPartial", new List<AdminViewUserVaccinationDetails>());
        }
    }

    [HttpGet("Admin/FilterHospital")]
    public ActionResult FilterHospitals(string filter)
    {
        try
        {
            List<HospitalDetailsModel> _adminViewFilteredHospitalDtails = _adminService.FilterAdminViewHospitalDetails(filter);

            if (_adminViewFilteredHospitalDtails.Count > 0)
            {
                return PartialView("_FilteredHospitalTablePartial", _adminViewFilteredHospitalDtails.ToList());
            }

            return PartialView("_AdminErrorMsgPartial", _adminViewFilteredHospitalDtails.ToList());
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred while displaying filtered hospital results: {ex.Message}");
            return PartialView("_AdminErrorMsgPartial", new List<HospitalDetailsModel>());
        }
    }

}
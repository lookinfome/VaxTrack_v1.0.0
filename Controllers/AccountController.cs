using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Identity;

namespace VaxTrack_v1.Controllers;

// class: account contoller | handle logic and registration requests
public class AccountController:Controller
{
    // variable: sign-in manager | for authentication
    private readonly SignInManager<AppUserModel> _signInManager;

    // vriable: account service | for accessing account service methods
    private readonly IAccountService _accountService;

    // constructor: account controller | to initialize controller class variables
    public AccountController(IAccountService accountService, SignInManager<AppUserModel> signInManager)
    {
        _accountService = accountService;
        _signInManager = signInManager;
    }


    /*
    *   action method: Login()
    *   http request: GET
    *   purpose: to get login form page
    *   return: login form view
    */

    [HttpGet("Account/Login")]
    public IActionResult Login()
    {
        return View();
    }

    // action method: submit login form

    /*
    *   action method: Login()
    *   http request: POST
    *   purpose: to submit login form
    *   parameter: login detail model as object
    *   return:
    *       if login success, get profile page for user, or admin dashboard for admin 
    *       if login failed, get back to login form
    */

    [HttpPost("Account/Login")]
    public async Task<IActionResult> Login(LoginDetailsModel submittedDetails)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Authenticate the user
                var result = await _signInManager.PasswordSignInAsync(submittedDetails.Username, submittedDetails.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["LoginMessage"] = $"Login Successfull for user - {submittedDetails.Username}";

                    if (_accountService.IsUserAdmin(submittedDetails.Username))
                    {
                        // Allow admin login
                        Console.WriteLine($"Login attempt success for admin - {submittedDetails.Username}");
                        return RedirectToAction("AdminPage", "Admin", new { username = submittedDetails.Username });
                    }

                    // Allow user login
                    Console.WriteLine($"Login attempt success for user - {submittedDetails.Username}");
                    return RedirectToAction("UserProfile", "Profile", new { username = submittedDetails.Username });
                }
                else
                {
                    Console.WriteLine($"Invalid username/password for user {submittedDetails.Username}");
                    return View(submittedDetails);
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid inputs for login form.");
                return View(submittedDetails);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during login: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            return View(submittedDetails);
        }
    }

    /*
    *   action method: Registration()
    *   http request: GET
    *   purpose: to get registration form page
    *   return: registration form view
    */

    [HttpGet("Account/Registration")]
    public IActionResult Registration()
    {
        return View();
    }

    /*
    *   action method: Registration()
    *   http request: POST
    *   purpose: to submit registration form
    *   parameter: user detail model as object
    *   return:
    *       if registration success, save new user details, as well default vaccination details for new user
    *       if registratio failed, get back to registration page
    */

    [HttpPost("Account/Registration")]
    public async Task<IActionResult> Registration(UserDetailsModel submittedDetails)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // save new user details
                var _newUserLoginDetails = await _accountService.SaveNewUser(submittedDetails);

                if (_newUserLoginDetails.Username != null)
                {
                    Console.WriteLine($"Registation successfull for user - {_newUserLoginDetails.Username}");

                    TempData["RegistrationMessage"]=$"Registation successfull for user - {_newUserLoginDetails.Username}";

                    // Redirect to Login action
                    return await Login(_newUserLoginDetails);
                }
                else
                {
                    Console.WriteLine("Invalid registration form inputs");
                    return View(submittedDetails);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid inputs for registration form.");
                return View(submittedDetails);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during registration: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            return View(submittedDetails);
        }
    }

    /*
    *   action method: Logout()
    *   http request: POST
    *   purpose: to logout user
    *   return:
    *       if logout success, redirect to home page
    */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // handling sign-out operation
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }



}
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Reflection;


namespace VaxTrack_v1.Controllers;

public class AccountController:Controller
{
    //
    private readonly SignInManager<AppUserModel> _signInManager;

    // vriable: account service
    private readonly IAccountService _accountService;

    // constructor: copy of in-house DB
    public AccountController(IAccountService accountService, SignInManager<AppUserModel> signInManager)
    {
        _accountService = accountService;
        _signInManager = signInManager;
    }

    // ===========
    // login page
    // ===========

    // action method:  get login page
    [HttpGet("Account/Login")]
    public IActionResult Login()
    {
        return View();
    }

    // action method: submit login page
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
                    if (_accountService.IsUserAdmin(submittedDetails.Username))
                    {
                        // Allow admin login
                        return RedirectToAction("AdminPage", "Admin", new { username = submittedDetails.Username });
                    }

                    // Allow user login
                    return RedirectToAction("UserProfile", "Profile", new { username = submittedDetails.Username });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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

    // =================
    // registration page
    // =================

    // action method: get registeration page
    [HttpGet("Account/Registration")]
    public IActionResult Registration()
    {
        return View();
    }

    // action method: submit registraion page
    [HttpPost("Account/Registration")]
    public async Task<IActionResult> Registration(UserDetailsModel submittedDetails)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var _newUserLoginDetails = await _accountService.SaveNewUser(submittedDetails);



                if (_newUserLoginDetails.Username != null)
                {
                    Console.WriteLine($"Login: If condition - {_newUserLoginDetails.Username}");

                    // Redirect to Login action
                    return await Login(_newUserLoginDetails);
                }
                else
                {
                    Console.WriteLine("Invalid form inputs");
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


    // =======
    // logout
    // =======

    // action method: logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }



}
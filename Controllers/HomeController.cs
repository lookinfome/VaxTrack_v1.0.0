using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;

namespace VaxTrack_v1.Controllers;

// class: home contoller | handle home page requests
public class HomeController : Controller
{
    // constructor: home controller | to initialize controller class variables
    public HomeController()
    {
    }

    /*
    *   action method: Index()
    *   http request: GET
    *   purpose: to get home page
    *   return: home view
    */
    public IActionResult Index()
    {
        return View();
    }
    
    /*
    *   action method: Privacy()
    *   http request: GET
    *   purpose: to get privacy details page
    *   return: privacy view
    */
    public IActionResult Privacy()
    {
        return View();
    }

    /*
    *   action method: Error()
    *   http request: GET
    *   purpose: to get error page
    *   return: error view
    */
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

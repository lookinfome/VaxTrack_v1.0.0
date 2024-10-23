using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using VaxTrack_v1.Services;

namespace VaxTrack_v1.Controllers;

// class: home contoller | handle home page requests
public class HomeController : Controller
{
    // constructor: home controller | to initialize controller class variables
    string[] _picList = new string[]{
        "../assets/img-2.jpg",
        "../assets/img-3.jpg",
        "../assets/img-4.jpg",
        "../assets/img-5.jpg",
        "../assets/img-6.jpg",
        "../assets/img-7.jpg"
    };

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
        ViewBag.PicList = _picList;
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

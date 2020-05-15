using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BloodOnTheWeb.Models;

namespace BloodOnTheWeb.Controllers
{
    public class HomeController : Controller
    {
        private string _theme;

        public IActionResult Index()
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                _theme = Request.Cookies["theme"];
            }
            else
            {
                _theme = "light";
            }
            ViewBag.theme = _theme;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Version()
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                _theme = Request.Cookies["theme"];
            }
            else
            {
                _theme = "light";
            }
            ViewBag.theme = _theme;
            
            ViewData["Message"] = "Application Versions";

            return View();
        }

        public IActionResult Privacy()
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                _theme = Request.Cookies["theme"];
            }
            else
            {
                _theme = "light";
            }
            ViewBag.theme = _theme;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

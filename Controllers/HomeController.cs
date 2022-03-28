using AuthTask.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secured()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {

            // after post login, we need to return the user to the intended page i.e the return url
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        public IActionResult Validate(string username, string password, string returnUrl)
        //public async Task<IActionResult> Validate(string username, string password,string returnUrl)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            if (username != "bob" && password != "ok")
            {

                return BadRequest();
               
            }
            //TempData["Error"] = "eRROR. Username or Password is invalid";
           // var claims = new List<Claim>();//properties that describe a user: from actions to atrributes

            //claims.Add(new Claim("username", username));
           // claims.Add(new Claim(ClaimTypes.NameIdentifier, username));

            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
           // var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);//authentication ticket
           // await HttpContext.SignInAsync(claimsPrincipal);
           return Redirect(returnUrl);
           //return View("Secured");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

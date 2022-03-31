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
            //ViewData["ReturnUrl"] = returnUrl; 
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password,string returnUrl)
        {
           // Console.WriteLine(username, password);
            
            if (username == "bob" && password == "ok")
            {
                
                //ViewData["ReturnUrl"] = returnUrl;
                var claims = new List<Claim>
                {
                    new Claim("username", username),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, "Mr")
                };//properties that describe a user: from actions to atrributes

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);//authentication ticket
                await HttpContext.SignInAsync(claimsPrincipal);
                //return Redirect(returnUrl);
                return View("Secured");
            }

            TempData["Error"] = "ERROR. Username or Password is invalid";

            return View("login");
           // return BadRequest();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize] //you need to be logged in to log out
        public async Task<IActionResult> Logout()
        {
            //User.Identity.IsAuthenticated = false;
            await HttpContext.SignOutAsync();
            
            return View("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

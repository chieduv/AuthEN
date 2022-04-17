﻿using AuthTask.Models;
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
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        

        public IActionResult Index()
        {
            return View();
        }

       [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Secured()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {

            // after post login, we need to return the user to the intended page i.e the return url
            ViewData["ReturnUrl"] = returnUrl; 
            return View();
        }

        [HttpGet("login/{provider}")]
        public IActionResult LoginExternal([FromRoute]string provider, [FromQuery]string returnUrl)
        {
            if (User != null && User.Identities.Any(identity => identity.IsAuthenticated)) 
            {
                RedirectToAction("", "Home");
            }
            //By default the client will be redirected back yo the URL that issued the challenge (/login?authtype=foo)'
            //send them to the homepage instead (/)

            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl }; 
             
            return new ChallengeResult(provider, authenticationProperties);
        }

        [ValidateAntiForgeryToken()]
        [Route("validate")]
        [HttpPost]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            ViewData["ReturnUrl"] = returnUrl;
            if (_userService.TryValidateUser(username, password, out List<Claim> claims))
            {
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var items = new Dictionary<string, string>();
                items.Add(".AuthScheme", CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties(items);
                await HttpContext.SignInAsync(claimsPrincipal, properties);
                return Redirect(returnUrl);
            }
            else
            {
                TempData["Error"] = "Error. Username or Password is invalid";
                return View("login");
            }
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }

        [Authorize] //you need to be logged in to log out
        public async Task<IActionResult> Logout()
        {
            
            await HttpContext.SignOutAsync();
            //return Redirect("/"); //return to th home page
            
            // to signout of google on logout and return to local host google uses a unique way to logout
            return Redirect(@"https://google.com/accounts/logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:5001");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

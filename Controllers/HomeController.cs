using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using LoginRegistration.Models;
using Microsoft.AspNetCore.Http;

namespace LoginRegistration.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
    
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
    
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost("register")]
        public IActionResult Register(User user)
        {
          // Check initial ModelState
          if(ModelState.IsValid)
          {
              // If a User exists with provided email
              if(dbContext.Users.Any(u => u.Email == user.Email))
              {
                  // Manually add a ModelState error to the Email field, with provided
                  // error message
                  ModelState.AddModelError("Email", "Email already in use!");
                  // You may consider returning to the View at this point
                  return View("Index");
              }

              PasswordHasher<User> Hasher = new PasswordHasher<User>();
              user.Password = Hasher.HashPassword(user, user.Password);
              dbContext.Users.Add(user);
              dbContext.SaveChanges();
              HttpContext.Session.SetInt32("userId", user.UserId);

              return RedirectToAction("Success");

          }
          return View("Index");
        } 

        [HttpGet("Success")]
        public IActionResult Success()
        {
          int? userId = HttpContext.Session.GetInt32("userId");
          if (userId == null)
          {
            return RedirectToAction("Index");
          }
          User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
          return View(user);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
          if(ModelState.IsValid)
          {
            User userInDb = dbContext.Users.FirstOrDefault(u=> u.Email == userSubmission.LoginEmail);
            if(userInDb == null)
              {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
              }
                //These two lines will compare our hashed passwords.
                var hash = new PasswordHasher<LoginUser>();
                var result = hash.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                //Result will either be 0 or 1.
                if(result == 0)
                {
                  ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                  return View("Index");
                } 
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return Redirect("Success");
          }
          return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
          HttpContext.Session.Clear();
          return RedirectToAction("Index");

        }
        

    }
}
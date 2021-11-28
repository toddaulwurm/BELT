using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BELT.Models;
using System.Text.RegularExpressions;

namespace BELT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View("LoginReg");
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasLetter = new Regex(@"[A-z]");
                var hasSymbol = new Regex(@"[$@?.!%&#*]");
                var isValidated = hasNumber.IsMatch(newUser.Password) && hasLetter.IsMatch(newUser.Password) && hasSymbol.IsMatch(newUser.Password);
                if(_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in system!");
                    return View("LoginReg");
                }
                if(!isValidated)
                {
                    ModelState.AddModelError("Password", "Password must have a letter, number and special character!");
                    return View("LoginReg");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                _context.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("LoggedUserId", newUser.UserId);


                return RedirectToAction("dashboard");
            }
            return View("LoginReg");
        }


        [HttpPost("login")]
        public IActionResult Login(LoginUser checkMe)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(use => use.Email == checkMe.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login!");
                    return View("LoginReg");
                }

                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(checkMe, userInDb.Password, checkMe.LoginPassword);

                if(result ==0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login!");
                    return View("LoginReg");
                }

                HttpContext.Session.SetInt32("LoggedUserId", userInDb.UserId);
                return RedirectToAction("dashboard");
            }
            return View("LoginReg");
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");

            ViewBag.User = _context.Users
                .Include(use => use.RSVPs)
                .ThenInclude(rsvp => rsvp.Shindig)
                .FirstOrDefault(use => use.UserId == loggedUserId);

            ViewBag.AllShindigs = _context.Shindigs
                .Include(shin => shin.RSVPs)
                .ThenInclude(rsvp => rsvp.User)
                // .OrderBy(shin => shin.Date)
                .ToList();

            ViewBag.AllUsers = _context.Users.ToList();

            return View();
        }

        [HttpGet("/newshindig")]
        public IActionResult NewShindig()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");

            ViewBag.User = _context.Users.FirstOrDefault(use => use.UserId == loggedUserId);

            return View("NewShindig");
        }
        [HttpPost("CreateShindig")]
        public IActionResult CreateShindig(Shindig newShindig)
        {
            if(ModelState.IsValid)
            {
                _context.Add(newShindig);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
                if(newShindig.DurationType == "minutes")
                {
                    TimeSpan newduration = new TimeSpan (0, newShindig.Duration, 0);
                    newShindig.EndTime = newShindig.Date.Add(newduration);
                    _context.Add(newShindig);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                if(newShindig.DurationType == "hours")
                {
                    TimeSpan newduration = new TimeSpan (newShindig.Duration, 0, 0);
                    newShindig.EndTime = newShindig.Date.Add(newduration);
                    _context.Add(newShindig);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                if(newShindig.DurationType == "days")
                {
                    TimeSpan newduration = new TimeSpan (newShindig.Duration, 0, 0, 0);
                    newShindig.EndTime = newShindig.Date.Add(newduration);
                    _context.Add(newShindig);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");
            ViewBag.User = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            return View("NewShindig");
        }


        [HttpGet("rsvp/{ShindigId}")]
        public IActionResult RSVP(int shindigId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");
            bool rsvpExists = _context.RSVPs.Any(rsvp => rsvp.UserId == loggedUserId && rsvp.ShindigId == shindigId);
            if(!rsvpExists){
                    RSVP newRSVP = new RSVP();
                    newRSVP.UserId = (int)loggedUserId;
                    newRSVP.ShindigId = shindigId;
                    _context.Add(newRSVP);  
                    _context.SaveChanges();
            }
            else
            {
                RSVP delete = _context.RSVPs.FirstOrDefault(rsvp => rsvp.UserId == loggedUserId && rsvp.ShindigId == shindigId);
                _context.RSVPs.Remove(delete);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpGet("ReadShindig/{ShindigId}")]
        public IActionResult ReadShindig(int shindigId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");
            ViewBag.User = _context.Users
                .Include(use => use.RSVPs)
                .ThenInclude(rsvp => rsvp.Shindig)
                .FirstOrDefault(user => user.UserId == loggedUserId);

            ViewBag.OneModel = _context.Shindigs
                .Include(shin => shin.User)
                .Include(shin => shin.RSVPs)
                .ThenInclude(rsvp => rsvp.User)
                .FirstOrDefault(shin => shin.ShindigId == shindigId);

            ViewBag.GuestList = _context.RSVPs
                .Include(rsvp => rsvp.User)
                .Where(rsvp => rsvp.ShindigId == shindigId)
                .ToList();

            return View();
        }

        [HttpGet("DeleteShindig/{ShindigId}")]
        public IActionResult DeleteShindig(int shindigId)
        {
            Shindig deletedShindig = _context.Shindigs
                .FirstOrDefault(shin => shin.ShindigId == shindigId);
            _context.Shindigs.Remove(deletedShindig);
            _context.SaveChanges();
            return RedirectToAction("Deleted");
        }
        [HttpGet("Deleted")]
        public IActionResult Deleted()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("LoggedUserId");
            if(loggedUserId==null) return RedirectToAction("Index");
            ViewBag.User = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            return View("Deleted");
        }
        

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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
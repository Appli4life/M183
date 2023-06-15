using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Session.Context;
using Session.Context.Entity;
using Session.Models;
using Session.Utilities;

namespace Session.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly SessionDBContext context;

        public HomeController(ILogger<HomeController> logger, SessionDBContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            this.logger.LogInformation("Login Seite wurde Aufgerufen");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserViewModel userViewModel)
        {
            this.logger.LogInformation("Login Request wurde gestartet");

            try
            {
                if (this.ModelState.IsValid)
                {
                    var user = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == userViewModel.UserName);
                    var hasher = new PasswordHasher<UserIdentity>();

                    if (user != null && hasher.VerifyHashedPassword(user, user.Password, userViewModel.UserPassword) == PasswordVerificationResult.Success)
                    {
                        this.HttpContext.Session.SetString(SessionConstants.UsernameProperty, user.UserName);
                        this.HttpContext.Session.SetString(SessionConstants.RoleProperty, user.Role);

                        this.logger.LogInformation("Login erfolgreich {@User}", user);

                        return this.RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        this.logger.LogWarning("Login Fehlgeschlagen {@UserViewModel}", userViewModel);
                        TempData["PasswordOrUsernameFalse"] = "Passwort oder Username stimmen nicht";
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogCritical(e, "Login Versuch hat eine Exception geworfen {@UserViewModel}", userViewModel);
                return StatusCode(500, "Server Fehler bei der Anmeldung");
            }

            this.logger.LogInformation("ModelState nicht Valid im Login {@UserViewModel}", userViewModel);
            return View(userViewModel);
        }

        [HttpGet]
        [SessionAuthorization]
        public async Task<IActionResult> Welcome()
        {
            return View();
        }

        [HttpGet]
        [SessionAuthorization(SessionConstants.AdminRole)]
        public async Task<IActionResult> Logging()
        {
            return View();
        }

        [HttpGet]
        [SessionAuthorization]
        public async Task<IActionResult> Logout()
        {
            var userName = this.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
            this.logger.LogInformation("User mit {Username} hat sich ausgeloggt", userName);
            this.HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
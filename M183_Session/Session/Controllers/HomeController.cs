using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Session.Context;
using Session.Context.Entity;
using Session.Models;
using Session.Utilities;

namespace Session.Controllers;

public class HomeController : Controller
{
    private readonly SessionDBContext context;

    public HomeController(SessionDBContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        Log.Information("Login Seite wurde Aufgerufen");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(UserViewModel userViewModel)
    {
        Log.Information("Login Request wurde gestartet");

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

                    Log.Information("Login erfolgreich {@User}", user);

                    return this.RedirectToAction("Welcome", "Home");
                }
                else
                {
                    Log.Warning("Login Fehlgeschlagen {@UserViewModel}", userViewModel);
                    TempData["PasswordOrUsernameFalse"] = "Passwort oder Username stimmen nicht";
                }
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Login Versuch hat eine Exception geworfen {@UserViewModel}", userViewModel);
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
            });
        }

        Log.Information("ModelState nicht Valid im Login {@UserViewModel}", userViewModel);
        return View(userViewModel);
    }

    [HttpGet]
    [SessionAuthorization]
    public async Task<IActionResult> Welcome()
    {
        var username = this.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
        Log.Information("Willkommensseite wurde von {Username} aufgerufen", username);

        try
        {
            var user = await this.context.Users.FirstAsync(x => x.UserName == username);
            return View(user);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Willkommmensseite hat eine Exception geworfen {Username}", username);
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
            });
        }
    }

    [HttpGet]
    [SessionAuthorization(SessionConstants.AdminRole)]
    public async Task<IActionResult> Logging()
    {
        var username = this.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
        Log.Information("Loggingseite wurde von {Username} aufgerufen", username);
        var fileName = @$"logs/logs-{DateTime.Today.AddDays(-1):yyyyMMdd}.txt";

        try
        {
            var vm = new LoggingViewModel()
            {
                Username = username,
            };

            if (System.IO.File.Exists(fileName))
            {
                Log.Information("Logfile mit {Filename} wird ausgelesen", fileName);
                vm.Loggs = await System.IO.File.ReadAllLinesAsync(fileName);
            }
            else
            {
                Log.Warning("Log File mit {FileName} nicht gefunden", fileName);
                vm.Loggs = new[] { "Keine Loggs" };
            }

            return View(vm);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Loggingseite hat eine Exception geworfen {Username} {Filename}", username, fileName);
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
            });
        }
    }

    [HttpGet]
    [SessionAuthorization]
    public async Task<IActionResult> Logout()
    {
        var userName = this.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
        Log.Information("User mit {Username} hat sich ausgeloggt", userName);
        this.HttpContext.Session.Clear();
        return this.RedirectToAction("Index", "Home");
    }
}
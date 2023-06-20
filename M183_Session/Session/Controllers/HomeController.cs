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

    [HttpGet]
    [SessionAuthorization(SessionConstants.AdminRole)]
    public async Task<IActionResult> AddUser()
    {
        Log.Information("Add User Webseite wird aufgerugen");
        return View();
    }

    [HttpPost]
    [SessionAuthorization(SessionConstants.AdminRole)]
    public async Task<IActionResult> AddUser(AddUserViewModel addUserViewModel)
    {
        try
        {
            if (this.ModelState.IsValid)
            {
                var hasher = new PasswordHasher<UserIdentity>();

                var newUser = new UserIdentity();
                newUser.UserName = addUserViewModel.UserName;
                newUser.Password = addUserViewModel.UserPassword;

                newUser.Password = hasher.HashPassword(newUser, newUser.Password);

                newUser.Role = SessionConstants.UserRole;

                await this.context.AddAsync(newUser);
                await this.context.SaveChangesAsync();
                TempData["UserCreated"] = "User wurde erfolgreich erstellt";

                var username = this.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
                Log.Information("{User} hat erstellt {@newUser}", username, newUser);

                return View();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, "User konnte nicht angelegt werden und hat eine Exception ausgelöst {@AddUserViewModel}", addUserViewModel);
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
            });
        }
        return View(addUserViewModel);
    }
}
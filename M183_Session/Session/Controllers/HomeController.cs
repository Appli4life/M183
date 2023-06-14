using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Session.Models;
using Session.Utilities;
using System.Diagnostics;

namespace Session.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userViewModel)
        {
            // TODO: Check Login
            return RedirectToAction("Welcome", "Home");
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
    }
}
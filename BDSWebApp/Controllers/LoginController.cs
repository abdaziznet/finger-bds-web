using BDSWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BDSWebApp.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Validate username and password

                if (LoginIsValid())
                {
                    // TODO: Create authentication token and set cookie

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        private bool LoginIsValid()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
    }
}

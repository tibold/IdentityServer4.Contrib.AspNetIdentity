using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;

namespace MVC.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
            return RedirectToAction("Index", "Home");
        }
    }
}